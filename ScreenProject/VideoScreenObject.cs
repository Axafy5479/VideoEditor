using ControlzEx.Standard;
using Data;
using FFMediaProject;
using FFMediaToolkit.Graphics;
using NAudioProj;
using OpenCvSharp;
using Reactive.Bindings;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WB.Pool;

namespace ScreenProject
{


    public class VideoScreenObject : Control, IScreenObject
    {
        static VideoScreenObject()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoScreenObject), new FrameworkPropertyMetadata(typeof(VideoScreenObject)));
        }

        public VideoScreenObject( string filePath, VideoObjViewModel vm)
        {
            TlItemObjVM = vm;
            Uri = filePath;
            audioCtrler = new(filePath);
            ViewModel = vm;
            DataContext = vm;
            FFMTVideo = new(filePath);

            Loaded += Initialize;
        }


        public VideoObjViewModel ViewModel { get; }
        public FFMTVideo FFMTVideo { get; }

        private ConcurrentQueue<PoolBMPSource> sourceQueue { get; } = new();

        private void Initialize(object sender, RoutedEventArgs e)
        {
            Width = FFMTVideo.Width;
            Height = FFMTVideo.Height;
        }


        private VideoCapturer? VideoCapturer { get; set; }
        public bool Disposed { get; set; } = false;

        public Control Control => this;

        public ITimelineObjectViewModel TlItemObjVM { get; }

        public string Uri { get; }

        private AudioController audioCtrler;
        public bool IsPlaying { get; private set; }


        public void Show(int globalFrame, bool isPlaying)
        {
            if(Disposed)
            {
                throw new Exception("Disposeされたインスタンスを操作することはできません");
            }

            VideoCapturer ??= new VideoCapturer(FFMTVideo, new PoolBMPSource(FFMTVideo.ZeroFrameBMP, ImageSource));

            var localFrame = globalFrame - TlItemObjVM.Frame.Value;
            var offsetedFrame = localFrame + TlItemObjVM.OffsetFrame.Value;

            VideoCapturer.Capture(offsetedFrame);

            if (isPlaying && !audioCtrler.IsPlaying)
            {
                audioCtrler.Play(offsetedFrame, ViewModel.GetCurrentVolumeRatio(localFrame));
                IsPlaying = isPlaying;
            }

            audioCtrler.ChangeVolume(ViewModel.GetCurrentVolumeRatio(localFrame));
        }

        public void Stop()
        {
            IsPlaying = false;
            if (audioCtrler.IsPlaying)
            {
                audioCtrler.Stop();
            }
        }

        public void Dispose()
        {
            Disposed = true;
            audioCtrler.Dispose();
            FFMTVideo.Dispose();
            VideoCapturer?.Dispose();
        }


        public ReactiveProperty<WriteableBitmap> ImageSource { get; } = new();
    }

    internal class PoolBMPSource : IPoolObject
    {
        public PoolBMPSource(BitmapSource sample, ReactiveProperty<WriteableBitmap> image)
        {
            Source = new WriteableBitmap(sample);
            Image = image;
        }

        private int id;
        public int Id { get => id; set => id = value; }


        public Action ReturnMethod { get; private set; }

        public WriteableBitmap Source { get; set; }
        public ReactiveProperty<WriteableBitmap> Image { get; }

        public byte[] ImageByte;

        public Panel GetParent()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
        }

        public void SetReturnMethod(Action returnMethod)
        {
            ReturnMethod = returnMethod;
        }
    }

    internal class ImageUpdater : IDisposable
    {
        public ImageUpdater(ConcurrentQueue<PoolBMPSource> sourceQueue, ReactiveProperty<BitmapSource> ImageSource)
        {
            //SourceQueue = sourceQueue;
            //this.ImageSource = ImageSource;
            //Task.Run(Update);
        }

        private bool Disposed { get; set; } = false;

        private async Task Update()
        {
            while (true)
            {
                if(Disposed)
                {
                    break;
                }

                if (SourceQueue.TryDequeue(out var s))
                {
                    if (!SourceQueue.Any())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ImageSource.Value = s.Source;
                        });
                    }
                }

                await Task.Delay(30);
            }
        }

        private ConcurrentQueue<PoolBMPSource> SourceQueue { get; }
        private ReactiveProperty<BitmapSource> ImageSource { get; }

        public void Dispose()
        {
            Disposed = true;
        }
    }

    internal class VideoCapturer : IDisposable
    {
        internal VideoCapturer(FFMTVideo ffmtVideo, PoolBMPSource queue)
        {
            FFMTVideo = ffmtVideo;
            Queue = queue;
        }



        private FFMTVideo? FFMTVideo { get; set; }
        private PoolBMPSource Queue { get; set; }
        //public Pool<PoolBMPSource> Pool { get; }
        public bool Disposed { get; private set; }

        /// <summary>
        /// キャプチャの最中か否か
        /// </summary>
        private bool IsBusy { get; set; }

        /// <summary>
        /// 次にキャプチャすべきフレーム
        /// </summary>
        private int reservedFrame = -1;

        public void Capture(int localOffsettedFrame)
        {
            if(Disposed)
            {
                throw new Exception("DisposeされたインスタンスのCaptureは使用できません");
            }

            reservedFrame = localOffsettedFrame;

            if (IsBusy) return;

            IsBusy = true;
            Task.Run(Run);
        }

        private async Task Run()
        {
            var targetFrame = reservedFrame;
            reservedFrame = -1;

            var targetTime = targetFrame / (double)60;

            if(FFMTVideo == null)
            {
                return;
            }

            //Stopwatch sw = Stopwatch.StartNew();
            var bmp = Queue;
            FFMTVideo.Capture(targetTime, out bmp.ImageByte, out int stride);


            Application.Current.Dispatcher.Invoke(() => {
                bmp.Source.Lock();
                bmp.Source.WritePixels(new Int32Rect(0, 0, 1920, 1080), bmp.ImageByte, stride,0);
                bmp.Source.Unlock();
                bmp.Image.Value = bmp.Source;

                //sw.Stop();
                //Debug.WriteLine(sw.Elapsed.ToString());
            });


            await Task.Delay(3);

            if (reservedFrame < 0)
            {
                IsBusy = false;
            }
            else
            {
                await Run();
            }
        }

        public void Dispose()
        {
            reservedFrame = -1;
            Disposed = true;
            Queue = null;
            FFMTVideo = null;
        }
    }
}
