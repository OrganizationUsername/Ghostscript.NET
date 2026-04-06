//
// ImageMemoryHelper.cs
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
using System.Runtime.InteropServices;

namespace Ghostscript.NET
{
    internal class ImageMemoryHelper
    {
        #region Set24bppRgbImageColor

        public unsafe static void Set24bppRgbImageColor(IntPtr image, int width, int height, byte r, byte g, byte b)
        {
            byte* ptr = (byte*)image;
            int stride = (((width * 4) + 3) & ~3); // BGRA8888 is 4 bytes per pixel

            int padding = stride - (width * 4);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    *ptr++ = b; // B
                    *ptr++ = g; // G
                    *ptr++ = r; // R
                    *ptr++ = 255; // A (fully opaque)
                }

                ptr+=padding;
            }
        }

        #endregion

        #region ConvertRgb24ToBgra32

        public unsafe static void ConvertRgb24ToBgra32(IntPtr srcBgr24, IntPtr destBgra32, int width, int height, int srcStride, int destStride)
        {
            byte* srcPtr = (byte*)srcBgr24;
            byte* destPtr = (byte*)destBgra32;

            for (int y = 0; y < height; y++)
            {
                byte* srcRow = srcPtr + (y * srcStride);
                byte* destRow = destPtr + (y * destStride);

                for (int x = 0; x < width; x++)
                {
                    // Ghostscript provides BGR format (little-endian: Blue/Black first)
                    byte b = *srcRow++;
                    byte g = *srcRow++;
                    byte r = *srcRow++;

                    *destRow++ = b; // B
                    *destRow++ = g; // G
                    *destRow++ = r; // R
                    *destRow++ = 255; // A (fully opaque)
                }
            }
        }

        #endregion

        #region CopyImagePartFrom

        public static void CopyImagePartFrom(IntPtr src, IntPtr dest, int x, int y, int width, int height, int stride, int bytesPerPixel)
        {
            int destStride = (((width * bytesPerPixel) + 3) & ~3); 

            int srcTop = y;
            int destTop = 0;
            int srcBottom = y + height - 1;
            int posSrcTop = 0;
            int posDestTop = 0;
            int bytesToCopy = width * bytesPerPixel;

            while (srcTop <= srcBottom)
            {
                posSrcTop = (srcTop * (stride)) + (x * bytesPerPixel);
                posDestTop = (destTop * (destStride));

                // Cross-platform memory copy using Marshal.Copy
                byte[] buffer = new byte[bytesToCopy];
                Marshal.Copy(new IntPtr((long)src + posSrcTop), buffer, 0, bytesToCopy);
                Marshal.Copy(buffer, 0, new IntPtr((long)dest + posDestTop), bytesToCopy);

                srcTop++;
                destTop++;
            }
        }

        #endregion

        #region CopyImagePartTo

        public static void CopyImagePartTo(IntPtr dest, IntPtr src, int x, int y, int width, int height, int stride, int bytesPerPixel)
        {
            int partStride = (((width * bytesPerPixel) + 3) & ~3); 

            int destTop = y;
            int srcTop = 0;
            int destBottom = y + height - 1;
            int posDestTop = 0;
            int posSrcTop = 0;
            int bytesToCopy = width * bytesPerPixel;

            while (destTop <= destBottom)
            {
                posDestTop = (destTop * stride) + (x * bytesPerPixel);
                posSrcTop = (srcTop * partStride);

                // Cross-platform memory copy using Marshal.Copy
                byte[] buffer = new byte[bytesToCopy];
                Marshal.Copy(new IntPtr((long)src + posSrcTop), buffer, 0, bytesToCopy);
                Marshal.Copy(buffer, 0, new IntPtr((long)dest + posDestTop), bytesToCopy);

                destTop++;
                srcTop++;
            }
        }

        #endregion

        #region CopyImagePartToRgb24ToBgra32

        public unsafe static void CopyImagePartToRgb24ToBgra32(IntPtr destBgra32, IntPtr srcBgr24, int x, int y, int width, int height, int destStride, int srcStride)
        {
            byte* srcPtr = (byte*)srcBgr24;
            byte* destPtr = (byte*)destBgra32;

            for (int row = 0; row < height; row++)
            {
                int srcY = row;
                int destY = y + row;

                byte* srcRow = srcPtr + (srcY * srcStride);
                byte* destRow = destPtr + (destY * destStride) + (x * 4); // BGRA is 4 bytes per pixel

                for (int col = 0; col < width; col++)
                {
                    int srcX = col;
                    byte* srcPixel = srcRow + (srcX * 3); // BGR is 3 bytes per pixel

                    // Ghostscript provides BGR format (little-endian: Blue/Black first)
                    byte b = srcPixel[0];
                    byte g = srcPixel[1];
                    byte r = srcPixel[2];

                    destRow[0] = b; // B
                    destRow[1] = g; // G
                    destRow[2] = r; // R
                    destRow[3] = 255; // A (fully opaque)

                    destRow += 4;
                }
            }
        }

        #endregion

        #region FlipImageVertically

        public static void FlipImageVertically(IntPtr src, IntPtr dest, int height, int stride)
        {
            int size = height * stride;

            var buffer = new byte[size];
            Marshal.Copy(src, buffer, 0, size);

            byte[] row = new byte[stride];

            int top = 0;
            int bottom = height - 1;
            int posTop;
            int posBottom;

            while (top <= bottom)
            {
                posTop = top * stride;
                posBottom = bottom * stride;

                Array.Copy(buffer, posTop, row, 0, stride);
                Array.Copy(buffer, posBottom, buffer, posTop, stride);
                Array.Copy(row, 0, buffer, posBottom, stride);

                top++;
                bottom--;
            }

            Marshal.Copy(buffer, 0, dest, size);
        }

        #endregion
    }
}
