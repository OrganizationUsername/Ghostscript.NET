# Changelog

### [1.3.3-rc.2] - 2025-12-16
- **Unicode filenames:** Pass command arguments and file paths with proper native encodings (UTF-16LE on Windows, UTF-8 on non-Windows) using pointer-based GSAPI calls to preserve Unicode filenames. Added pointer-based delegates `gsapi_init_with_args_ptr` and `gsapi_run_file_ptr` and updated processor/interpreter to use them when supported.

### [1.3.2] - 2025-12-10
- Upgraded SkiaSharp version (2.88.8 -> 3.119.1)
- Included SkiaSharp.NativeAssets.Linux

### [1.3.2-rc.3] - 2025-12-05
- Converted from System.Drawing.Bitmap to SkiaSharp.SKBitmap for cross-platform support
- Replaced System.Drawing.Common package with SkiaSharp (2.88.8)
- Updated memory operations to work on both Windows and Linux
- Fixed color accuracy by correctly handling BGR format from Ghostscript
- Updated platform-specific structures for Linux compatibility