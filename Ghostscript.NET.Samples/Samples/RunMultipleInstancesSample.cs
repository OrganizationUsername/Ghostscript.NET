//
// RunMultipleInstancesSample.cs
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
using System.Runtime.InteropServices;
using System.Threading;

namespace Ghostscript.NET.Samples
{
    public class RunMultipleInstancesSample : ISample
    {
        private GhostscriptVersionInfo _gs_verssion_info = GhostscriptVersionInfo.GetLastInstalledVersion();

        public void Start()
        {
            // Wait until both workers finish; otherwise the process can exit before
            // Ghostscript writes output (thread-pool work is not foreground work).
            using (CountdownEvent done = new CountdownEvent(2))
            {
                ThreadPool.QueueUserWorkItem(Instance1, done);
                ThreadPool.QueueUserWorkItem(Instance2, done);
                done.Wait();
            }
        }

        private void Process(string input, string output, int startPage, int endPage)
        {
            // Load GS from memory on Windows so parallel instances do not share one native DLL mapping.
            bool fromMemory = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            using (Processor.GhostscriptProcessor processor = new Processor.GhostscriptProcessor(_gs_verssion_info, fromMemory))
            {
                processor.StartProcessing(CreateTestArgs(input, output, startPage, endPage), new ConsoleStdIO(true, true, true));
            }
        }

        private void Instance1(object state)
        {
            CountdownEvent done = (CountdownEvent)state;
            try
            {
                // Same test PDF as ProcessorSample1 (place any PDF under TestFiles\).
                Process(@"..\..\..\TestFiles\ProcessorSample1.pdf", @".\Output\RunMultipleInstancesSample-a%03d.png", 1, 2);
            }
            finally
            {
                done.Signal();
            }
        }

        private void Instance2(object state)
        {
            CountdownEvent done = (CountdownEvent)state;
            try
            {
                Process(@"..\..\..\TestFiles\ProcessorSample1.pdf", @".\Output\RunMultipleInstancesSample-b%03d.png", 1, 2);
            }
            finally
            {
                done.Signal();
            }
        }

        private string[] CreateTestArgs(string inputPath, string outputPath, int pageFrom, int pageTo)
        {
            List<string> gsArgs = new List<string>();

            gsArgs.Add("-q");
            gsArgs.Add("-dSAFER");
            gsArgs.Add("-dBATCH");
            gsArgs.Add("-dNOPAUSE");
            gsArgs.Add("-dNOPROMPT");
            gsArgs.Add(@"-sFONTPATH=" + System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts));
            gsArgs.Add("-dFirstPage=" + pageFrom.ToString());
            gsArgs.Add("-dLastPage=" + pageTo.ToString());
            gsArgs.Add("-sDEVICE=png16m");
            gsArgs.Add("-r72");
            gsArgs.Add("-sPAPERSIZE=a4");
            gsArgs.Add("-dNumRenderingThreads=" + Environment.ProcessorCount.ToString());
            gsArgs.Add("-dTextAlphaBits=4");
            gsArgs.Add("-dGraphicsAlphaBits=4");
            gsArgs.Add(@"-sOutputFile=" + outputPath);
            gsArgs.Add("-f");
            gsArgs.Add(inputPath);

            return gsArgs.ToArray();
        }
    }
}
