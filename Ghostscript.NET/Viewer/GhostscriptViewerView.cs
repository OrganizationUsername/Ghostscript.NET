//
// GhostscriptViewerView.cs
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
using SkiaSharp;

namespace Ghostscript.NET.Viewer
{
    #region delegate GhostscriptViewerViewEventHandler

    public delegate void GhostscriptViewerViewEventHandler(object sender, GhostscriptViewerViewEventArgs e);

    #endregion

    public class GhostscriptViewerViewEventArgs : EventArgs
    {
        #region Private variables

        private GhostscriptViewerImage _image;
        private SKRect _mediaBox;

        #endregion

        #region Constructor

        internal GhostscriptViewerViewEventArgs(GhostscriptViewerImage image, SKRectI mediaBox)
        {
            _image = image;
            _mediaBox = new SKRect(mediaBox.Left, mediaBox.Top, mediaBox.Right, mediaBox.Bottom);
        }

        #endregion

        #region Image

        public SKBitmap Image
        {
            get { return _image.Bitmap; }
        }

        #endregion

        #region MediaBox


        public SKRect MediaBox
        {
            get { return _mediaBox; }
        }

        #endregion
    }
}
