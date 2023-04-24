using FFmpeg.AutoGen;
using System;

namespace Ffmpeg
{
    public unsafe class FFMpegInitializer
    {
        public FFMpegInitializer()
        {
            ffmpeg.RootPath = @"\ffmpeg-4.4.1-full_build-shared\bin";
        }

        string path = @"C:\Users\sunny\temp\piano.mp4";

        AVFormatContext* formatContext;

        public void OpenFile(string path)
        {
            AVFormatContext* _formatContext = null;
            ffmpeg.avformat_open_input(&_formatContext, path, null, null)
                .OnError(() => throw new InvalidOperationException("指定のファイルは開けませんでした。"));
            formatContext = _formatContext;

            ffmpeg.avformat_find_stream_info(formatContext, null)
                .OnError(() => throw new InvalidOperationException("ストリームを検出できませんでした。"));
        }

        private AVStream* GetFirstVideoStream()
        {
            for (int i = 0; i < (int)formatContext->nb_streams; ++i)
            {
                var stream = formatContext->streams[i];
                if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    return stream;
                }
            }
            return null;
        }

        private AVStream* GetFirstAudioStream()
        {
            for (int i = 0; i < (int)formatContext->nb_streams; i++)
            {
                var stream = formatContext->streams[i];
                if (stream->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                {
                    return stream;
                }
            }
            return null;
        }

    }
}
