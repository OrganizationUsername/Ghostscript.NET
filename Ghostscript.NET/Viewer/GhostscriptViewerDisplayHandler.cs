//
// GhostscriptViewerDisplayHandler.cs
// This file is part of Ghostscript.NET library
//
// Author: Artifex Software Inc. 
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Text;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Ghostscript.NET.Viewer
{
    internal class GhostscriptViewerDisplayHandler : GhostscriptDisplayDeviceHandler
    {

        #region Private variables

        private GhostscriptViewer _viewer;
        private GhostscriptViewerImage _destImage;
        private IntPtr _srcImage;
        private uint _srcFormat;
        private int _srcStride;
        private long _lastUpdateTime = Environment.TickCount;
        private bool _synchTriggered = false;

        #endregion

        #region GhostscriptViewerDisplayHandler

        public GhostscriptViewerDisplayHandler(GhostscriptViewer viewer) : base(viewer.Interpreter.GhostscriptLibrary)
        {
            _viewer = viewer;
        }

        #endregion

        #region Open

        public override int Open(IntPtr handle, IntPtr device)
        {
            return 0;
        }

        #endregion

        #region Preclose

        public override int Preclose(IntPtr handle, IntPtr device)
        {
            return 0;
        }

        #endregion

        #region Close

        public override int Close(IntPtr handle, IntPtr device)
        {
            if (_destImage != null)
            {
                _destImage.Dispose(); _destImage = null;
            }

            return 0;
        }

        #endregion

        #region Presize

        public override int Presize(IntPtr handle, IntPtr device, int width, int height, int raster, uint format)
        {
            _srcFormat = format;

            if (_destImage != null)
            {
                _destImage.Dispose(); _destImage = null;
            }

            _destImage = GhostscriptViewerImage.Create(width, height, raster, SKColorType.Bgra8888);

            return 0;
        }

        #endregion

        #region Size

        public override int Size(IntPtr handle, IntPtr device, int width, int height, int raster, uint format, IntPtr pimage)
        {
            _srcStride = raster;
            _srcFormat = format;
            _srcImage = pimage;

            if (_destImage != null)
            {
                _destImage.Dispose(); _destImage = null;
            }

            _destImage = GhostscriptViewerImage.Create(width, height, raster, SKColorType.Bgra8888);

            _viewer.FormatHandler.ShowPagePostScriptCommandInvoked = false;

            _viewer.RaiseDisplaySize(new GhostscriptViewerViewEventArgs(_destImage, new SKRectI(0, 0, width, height)));

            return 0;
        }

        #endregion

        #region Sync

        public override int Sync(IntPtr handle, IntPtr device)
        {
            _synchTriggered = true;

            return 0;
        }

        #endregion

        #region Page

        public override int Page(IntPtr handle, IntPtr device, int copies, int flush)
        {
            _viewer.FormatHandler.ShowPagePostScriptCommandInvoked = true;

            if (!_viewer.ProgressiveUpdate || _viewer.Interpreter.GhostscriptLibrary.Revision > 950)
            {
                int srcBytesPerPixel = 3; // BGR from Ghostscript (little-endian: Blue/Black first)
                int destBytesPerPixel = 4; // BGRA for SkiaSharp

                _destImage.Lock();

                // Allocate temporary buffer for BGR24 data
                int srcStrideSize = (((_destImage.Width * srcBytesPerPixel) + 3) & ~3);
                IntPtr tempTile = Marshal.AllocHGlobal(srcStrideSize * _destImage.Height);

                // Copy BGR24 from source
                ImageMemoryHelper.CopyImagePartFrom(_srcImage, tempTile, 0, 0, _destImage.Width, _destImage.Height, _srcStride, srcBytesPerPixel);
                
                // Flip vertically
                ImageMemoryHelper.FlipImageVertically(tempTile, tempTile, _destImage.Height, srcStrideSize);

                // Convert BGR24 to BGRA32 and copy to destination
                ImageMemoryHelper.ConvertRgb24ToBgra32(tempTile, _destImage.Scan0, _destImage.Width, _destImage.Height, srcStrideSize, _destImage.Stride);

                Marshal.FreeHGlobal(tempTile);

                _destImage.Unlock();
            }

            _viewer.RaiseDisplayPage(new GhostscriptViewerViewEventArgs(_destImage, new SKRectI(0, 0, _destImage.Width, _destImage.Height)));

            return 0;
        }

        #endregion

        #region Update

        public override int Update(IntPtr handle, IntPtr device, int x, int y, int w, int h)
        {
            if (_viewer.ProgressiveUpdate)
            {
                int srcBytesPerPixel = 3; // BGR from Ghostscript (little-endian: Blue/Black first)
                int destBytesPerPixel = 4; // BGRA for SkiaSharp

                if (_srcImage != IntPtr.Zero)
                {
                    _destImage.Lock();

                    int srcTileStride = (((w * srcBytesPerPixel) + 3) & ~3);

                    if (_synchTriggered)
                    {
                        _synchTriggered = false;

                        ImageMemoryHelper.Set24bppRgbImageColor(_destImage.Scan0, _destImage.Width, _destImage.Height, 255, 255, 255);
                    }

                    if (w == _destImage.Width && h == _destImage.Height)
                    {
                        _destImage.Unlock();

                        return 0;
                    }

                    // Allocate temporary buffer for BGR24 tile
                    IntPtr tempTile = Marshal.AllocHGlobal(srcTileStride * h);

                    // Copy BGR24 tile from source
                    ImageMemoryHelper.CopyImagePartFrom(_srcImage, tempTile, x, y, w, h, _srcStride, srcBytesPerPixel);

                    // Flip vertically
                    ImageMemoryHelper.FlipImageVertically(tempTile, tempTile, h, srcTileStride);

                    int tileMirrorY = _destImage.Height - y - h;

                    // Convert BGR24 to BGRA32 and copy to destination
                    ImageMemoryHelper.CopyImagePartToRgb24ToBgra32(_destImage.Scan0, tempTile, x, tileMirrorY, w, h, _destImage.Stride, srcTileStride);

                    Marshal.FreeHGlobal(tempTile);

                    _destImage.Unlock();

                    if (Environment.TickCount - _lastUpdateTime > _viewer.ProgressiveUpdateInterval)
                    {
                        _lastUpdateTime = Environment.TickCount;

                        _viewer.RaiseDisplayUpdate(new GhostscriptViewerViewEventArgs(_destImage, new SKRectI(0, 0, _destImage.Width, _destImage.Height)));
                    }
                }
            }

            return 0;
        }

        #endregion

    }
}