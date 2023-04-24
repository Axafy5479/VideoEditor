using Data;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class VideoObjViewModel : TimelineItemObjectViewModel, IRenderingScreenObjVM, IAudioScreenObjectVM
    {
        public VideoObjViewModel(VideoItem videoItem, VideoScreenObjModel model) : base(videoItem, model)
        {
            FilePath = videoItem.FilePath;

            Model = model;
            Zoom = Model.Zoom;
            X = Model.X; 
            Y = Model.Y;

            Volume = Model.Volume;
            PlaybackRate = Model.PlaybackRate;
            FadeIn = Model.FadeIn;
            FadeOut = Model.FadeOut;
            IsLooped = Model.IsLooped;
            EchoIsEnabled = Model.EchoIsEnabled;
            EchoInterval = Model.EchoInterval;
        }

        public string FilePath { get; }
        public VideoScreenObjModel Model { get; }
        public ReactiveProperty<double> Zoom { get; }
        public ReactiveProperty<double> X { get; }
        public ReactiveProperty<double> Y { get; }

        public IRenderingItem RenderingItem => Model.VideoItem;
        public IAudioItem AudioItem => Model.VideoItem;


        public ReactiveProperty<double> Volume { get; }
        public ReactiveProperty<double> PlaybackRate { get; }
        public ReactiveProperty<double> FadeIn { get; }
        public ReactiveProperty<double> FadeOut { get; }
        public ReactiveProperty<bool> IsLooped { get; }
        public ReactiveProperty<bool> EchoIsEnabled { get; }
        public ReactiveProperty<double> EchoInterval { get; }

        public double GetCurrentVolumeRatio(int localFrame)
        {
            double currentTime = localFrame / 60.0;
            double rest = (Length.Value / 60) - currentTime;
            if (currentTime < FadeIn.Value)
            {
                return 0.01 * Volume.Value * currentTime / FadeIn.Value;
            }
            else if(currentTime>(Length.Value/60)-FadeOut.Value)
            {
                return 0.01 * Volume.Value * rest / FadeOut.Value;
            }
            else
            {
                return 0.01 * Volume.Value;
            }
        }
    }
}
