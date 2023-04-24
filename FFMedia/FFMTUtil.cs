using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace FFMediaProject
{
    internal unsafe static class FFMTUtil
    {

        public static unsafe BitmapSource ToBitmap(this ImageData bitmapData)
        {

            fixed (byte* ptr = bitmapData.Data)
            {
                return BitmapSource.Create(bitmapData.ImageSize.Width, bitmapData.ImageSize.Height, 96, 96, PixelFormats.Bgr24, null, new IntPtr(ptr), bitmapData.Data.Length, bitmapData.Stride);
            }


        }

        public static unsafe void WriteToBitmap(this ImageData bitmapData, WriteableBitmap wb)
        {
            wb.Lock();
            fixed (byte* ptr = bitmapData.Data)
            {
                byte* pBackBuffer = (byte*)wb.BackBuffer;
                *pBackBuffer = *ptr;
                //wb.WritePixels(new Int32Rect(0, 0, 1920, 1080), new IntPtr(ptr), bitmapData.Data.Length, bitmapData.Stride);

            }

            wb.AddDirtyRect(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight));
            wb.Unlock();

        }




        //private static ImageData FrameToImageData(Bitmap bitmap)
        //{
        //    Rectangle rect = new Rectangle(System.Drawing.Point.Empty, bitmap.Size);
        //    BitmapData bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        //    ImageData bitmapImageData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);
        //    bitmap.UnlockBits(bitLock);
        //    return bitmapImageData;
        //}
    }

    public class FFMTVideo : IDisposable
    {
        public FFMTVideo(string filePath)
        {


            MediaFile = MediaFile.Open(filePath);
            Duration = MediaFile.Info.Duration.TotalSeconds;
            Disposed = false;
            ZeroFrameBMP = CaptureForInfomation();
            Width = ZeroFrameBMP.Width;// source.Width;
            Height = ZeroFrameBMP.Height;// source.Height;
            FilePath = filePath;
        }

        public BitmapSource ZeroFrameBMP { get; }

        private MediaFile MediaFile { get; set; }
        public double Duration { get; }
        public double Width { get; }
        public double Height { get; }
        public bool Disposed { get; set; } = true;
        public string FilePath { get; }
        private bool Capturing { get; set; } = false;

        public BitmapSource CaptureForInfomation()
        {
            Capturing = true;
            var t = TimeSpan.FromSeconds(0);
            MediaFile.Video.TryGetFrame(t, out var sourceData);
            var bmp = sourceData.ToBitmap();
            bmp.Freeze();
            Capturing = false;
            return bmp;
        }

        public void Capture(double timeSec, out byte[] imageByte, out int stride)
        {
            Capturing = true;
            if (Disposed)
            {
                MediaFile = MediaFile.Open(FilePath);
                Disposed = false;
            }

            var t = TimeSpan.FromSeconds(timeSec);

            MediaFile.Video.TryGetFrame(t, out var sourceDataOut);
            int length = sourceDataOut.Data.Length;
            imageByte = sourceDataOut.Data.ToArray();
            stride = sourceDataOut.Stride;


            //var bmp = sourceData.ToBitmap();
            //sourceDataOut.WriteToBitmap(source);
            //bmp.Freeze();
            Capturing = false;
             
            //return bmp;
        }

        public void Dispose()
        {
            Task.Run(_Dispose);
        }

        private async Task _Dispose()
        {
            while (Capturing)
            {
                await Task.Delay(100);
            }

            MediaFile?.Dispose();
            Disposed = true;
        }

    }
}