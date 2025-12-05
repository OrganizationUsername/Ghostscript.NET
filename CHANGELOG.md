# Changelog

### [1.3.2-rc.3] - 2025-12-05
- Converted from System.Drawing.Bitmap to SkiaSharp.SKBitmap for cross-platform support
- Replaced System.Drawing.Common package with SkiaSharp (2.88.8)
- Updated memory operations to work on both Windows and Linux
- Fixed color accuracy by correctly handling BGR format from Ghostscript
- Updated platform-specific structures for Linux compatibility