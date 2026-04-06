//
// UnicodeTestSample.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;
using SkiaSharp;

namespace Ghostscript.NET.Samples
{
    /// <summary>
    /// UnicodeTestSample tests the ability of Ghostscript.NET to handle unicode characters
    /// in file paths and content. This includes:
    /// - Japanese characters (こんにちは)
    /// - Chinese characters (你好)
    /// - Arabic characters (مرحبا)
    /// - Russian characters (Привет)
    /// - And other unicode characters
    /// </summary>
    public class UnicodeTestSample : ISample
    {
        public void Start()
        {
            TestRasterizerWithUnicodePaths();
            TestProcessorWithUnicodePaths();
        }

        /// <summary>
        /// Tests GhostscriptRasterizer with unicode characters in output file paths
        /// </summary>
        private void TestRasterizerWithUnicodePaths()
        {
            int desired_dpi = 96;

            // Use an existing test file
            string inputPdfPath = @"..\..\..\TestFiles\こんにちは.pdf";
            string outputPath = @".\Output";

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            GhostscriptVersionInfo gvi = GhostscriptVersionInfo.GetLastInstalledVersion();

            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(inputPdfPath, gvi, false);

                // Test with Japanese characters
                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount && pageNumber <= 2; pageNumber++)
                {
                    var pageFilePath = Path.Combine(outputPath, string.Format("こんにちは-{0}.png", pageNumber));

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

                    Console.WriteLine($"Unicode test - Japanese output: {pageFilePath}");
                }
            }

            // Test with Chinese characters
            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(inputPdfPath, gvi, false);

                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount && pageNumber <= 2; pageNumber++)
                {
                    var pageFilePath = Path.Combine(outputPath, string.Format("你好-{0}.png", pageNumber));

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

                    Console.WriteLine($"Unicode test - Chinese output: {pageFilePath}");
                }
            }

            // Test with Arabic characters
            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(inputPdfPath, gvi, false);

                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount && pageNumber <= 1; pageNumber++)
                {
                    var pageFilePath = Path.Combine(outputPath, string.Format("مرحبا-{0}.png", pageNumber));

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

                    Console.WriteLine($"Unicode test - Arabic output: {pageFilePath}");
                }
            }

            // Test with Russian characters
            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(inputPdfPath, gvi, false);

                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount && pageNumber <= 1; pageNumber++)
                {
                    var pageFilePath = Path.Combine(outputPath, string.Format("Привет-{0}.png", pageNumber));

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

                    Console.WriteLine($"Unicode test - Russian output: {pageFilePath}");
                }
            }
        }

        /// <summary>
        /// Tests GhostscriptProcessor with unicode characters in output file paths
        /// </summary>
        private void TestProcessorWithUnicodePaths()
        {
            string inputFile = @"..\..\..\TestFiles\你好.pdf";
            string outputPath = @".\Output";

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // Test with Japanese characters in output filename
            string outputFile = Path.Combine(outputPath, "こんにちは-processor-%03d.png");

            using (GhostscriptProcessor ghostscript = new GhostscriptProcessor())
            {
                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                switches.Add("-dFirstPage=1");
                switches.Add("-dLastPage=1");
                switches.Add("-sDEVICE=png16m");
                switches.Add("-r96");
                switches.Add("-dTextAlphaBits=4");
                switches.Add("-dGraphicsAlphaBits=4");
                switches.Add(@"-sOutputFile=" + outputFile);
                switches.Add(@"-f");
                switches.Add(inputFile);

                ghostscript.Process(switches.ToArray());
            }

            Console.WriteLine($"Unicode test - Processor output: {outputFile}");

            // Test with Chinese characters in output filename
            outputFile = Path.Combine(outputPath, "你好-processor-%03d.png");

            using (GhostscriptProcessor ghostscript = new GhostscriptProcessor())
            {
                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                switches.Add("-dFirstPage=1");
                switches.Add("-dLastPage=1");
                switches.Add("-sDEVICE=png16m");
                switches.Add("-r96");
                switches.Add("-dTextAlphaBits=4");
                switches.Add("-dGraphicsAlphaBits=4");
                switches.Add(@"-sOutputFile=" + outputFile);
                switches.Add(@"-f");
                switches.Add(inputFile);

                ghostscript.Process(switches.ToArray());
            }

            Console.WriteLine($"Unicode test - Processor output: {outputFile}");
        }
    }
}

