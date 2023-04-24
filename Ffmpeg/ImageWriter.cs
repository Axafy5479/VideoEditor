using FFmpegWraper;
using System.Windows.Media.Imaging;
using System.Windows;
using System;

public class ImageWriter
{
    private readonly Int32Rect rect;
    private readonly WriteableBitmap writeableBitmap;

    public ImageWriter(int width, int height, WriteableBitmap writeableBitmap)
    {
        this.rect = new Int32Rect(0, 0, width, height);
        this.writeableBitmap = writeableBitmap;
    }

    public void WriteFrame(ManagedFrame frame, FrameConveter frameConveter)
    {
        var bitmap = writeableBitmap;
        bitmap.Lock();
        try
        {
            IntPtr ptr = bitmap.BackBuffer;
            frameConveter.ConvertFrameDirect(frame, ptr);
            bitmap.AddDirtyRect(rect);
        }
        finally
        {
            bitmap.Unlock();
        }
    }
}