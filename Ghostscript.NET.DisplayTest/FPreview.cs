using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Ghostscript.NET.DisplayTest
{
    public partial class FPreview : Form
    {
        private SKBitmap _currentBitmap = null;

        public FPreview()
        {
            InitializeComponent();

            this.Left = 0;
            this.Top = 0;

            pbDisplay.PaintSurface += PbDisplay_PaintSurface;
        }

        private void PbDisplay_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            if (_currentBitmap != null)
            {
                var destRect = SKRect.Create(pbDisplay.Width, pbDisplay.Height);
                var sourceRect = SKRect.Create(_currentBitmap.Width, _currentBitmap.Height);
                canvas.DrawBitmap(_currentBitmap, sourceRect, destRect);
            }
        }

        public void UpdateImage(SKBitmap bitmap)
        {
            _currentBitmap = bitmap;
            ResizeCanvas(bitmap);
            RefreshCanvas();
        }

        private void FPreview_Load(object sender, EventArgs e)
        {

        }

        private void ResizeCanvas(SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                return;
            }

            if (pbDisplay.Width == bitmap.Width && pbDisplay.Height == bitmap.Height)
            {
                return;
            }

            pbDisplay.SuspendLayout();
            pbDisplay.Width = bitmap.Width;
            pbDisplay.Height = bitmap.Height;
            pbDisplay.ResumeLayout();
        }

        private void RefreshCanvas()
        {
            if (!pbDisplay.IsHandleCreated)
            {
                return;
            }

            pbDisplay.Invalidate();
            pbDisplay.Update();
        }
    }
}
