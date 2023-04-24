using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;

namespace FFMediaProject
{
    internal unsafe static class FFMTUtil
    {
        public static void GetFrame(string path, double time)
        {
            int i = 0;
            var file = MediaFile.Open(path);
            while (file.Video.TryGetNextFrame(out var imageData))
            {
                imageData.ToBitmap().Save($@"C:\images\frame_{i++}.png");
                // See the #Usage details for example .ToBitmap() implementation
                // The .Save() method may be different depending on your graphics library
            }
        }

        public static unsafe BitmapSource ToBitmap(this ImageData bitmapData)
        {
            fixed (byte* ptr = bitmapData.Data)
            {
                return BitmapSource.Create(bitmapData.ImageSize.Width, bitmapData.ImageSize.Height, 96, 96, PixelFormats.Bgr32, null, new IntPtr(ptr), bitmapData.Data.Length, bitmapData.Stride);
            }
        }

    }
}