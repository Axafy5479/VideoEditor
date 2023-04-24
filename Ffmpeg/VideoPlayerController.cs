using FFmpeg.AutoGen;
using FFmpegWraper;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;
using Ffmpeg;

public class VideoPlayController
{
    private static readonly AVPixelFormat ffPixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24;
    private static readonly PixelFormat wpfPixelFormat = PixelFormats.Bgr24;

    private Decoder decoder;
    private ImageWriter imageWriter;
    private FrameConveter frameConveter;

    public VideoPlayController()
    {

    }

    public void OpenFile(string path)
    {
        decoder = new Decoder();
        decoder.OpenFile(path);
    }

    public WriteableBitmap CreateBitmap(int dpiX, int dpiY)
    {
        if (decoder is null)
        {
            throw new InvalidOperationException("描画先を作成する前に動画を開く必要があります。");
        }
        var context = decoder.VideoCodecContext;
        int width = context.width;
        int height = context.height;

        WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, dpiX, dpiY, wpfPixelFormat, null);
        this.imageWriter = new ImageWriter(width, height, writeableBitmap);

        this.frameConveter = new FrameConveter();
        frameConveter.Configure(context.pix_fmt, context.width, context.height, ffPixelFormat, width, height);

        return writeableBitmap;
    }
}