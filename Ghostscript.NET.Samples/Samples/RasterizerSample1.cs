//
// RasterizerSample.cs
// This file is part of Ghostscript.NET.Samples project
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

// required Ghostscript.NET namespaces
using System;
using System.Drawing.Imaging;
using System.IO;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET.Samples.StdIOHandlers;
using SkiaSharp;

namespace Ghostscript.NET.Samples
{
    /// <summary>
    /// GhostscriptRasterizer allows you to rasterize pdf and postscript files into the 
    /// memory. If you want Ghostscript to store files on the disk use GhostscriptProcessor
    /// or one of the GhostscriptDevices (GhostscriptPngDevice, GhostscriptJpgDevice).
    /// </summary>
    public class RasterizerSample1 : ISample
    {
        public void Start()
        {
            Sample1();
            Sample2();
        }

        public void Sample1()
        {
            int desired_dpi = 96;

            string inputPdfPath = @"..\..\..\TestFiles\RasterizerSample1.pdf";
            string outputPath = @".\Output";

            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion();

            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(inputPdfPath, gvi, false);

                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    var pageFilePath = Path.Combine(outputPath, string.Format("RasterizerSample1-Sample1-{0}.png", pageNumber));

                    var img = rasterizer.GetPage(desired_dpi, pageNumber);
                    if (img != null)
                    {
                        using (var image = SKImage.FromBitmap(img))
                        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                        using (var stream = File.OpenWrite(pageFilePath))
                        {
                            data.SaveTo(stream);
                        }
                    }

                    Console.WriteLine(pageFilePath);
                }
            }
        }

        public void Sample2()
        {
            int desired_dpi = 96;

            string inputPdfPath = @"..\..\..\TestFiles\RasterizerSample1.pdf";
            string outputPath = @".\Output";

            var output = new DelegateStdIOHandler(
                stdOut: Console.WriteLine,
                stdErr: Console.WriteLine
                );

            using (var rasterizer = new GhostscriptRasterizer(output))
            {
                rasterizer.Open(inputPdfPath);

                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    var pageFilePath = Path.Combine(outputPath, string.Format("RasterizerSample1-Sample2-{0}.png", pageNumber));

                    var img = rasterizer.GetPage(desired_dpi, pageNumber);
                    if (img != null)
                    {
                        using (var image = SKImage.FromBitmap(img))
                        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                        using (var stream = File.OpenWrite(pageFilePath))
                        {
                            data.SaveTo(stream);
                        }
                    }

                    Console.WriteLine(pageFilePath);
                }
            }
        }
    }
}
