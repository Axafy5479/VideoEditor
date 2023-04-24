using Data;
using Reactive.Bindings;
using ScreenProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timeline.CustomControl
{
    public class AudioObjViewModel : TimelineItemObjectViewModel, IAudioScreenObjectVM
    {
        public AudioObjViewModel(AudioItem audioItem, AudioScreenObjModel model) : base(audioItem, model)
        {
            FilePath = audioItem.FilePath;

            Model = model;
            AudioItem = audioItem;

            Volume = Model.Volume;
            PlaybackRate = Model.PlaybackRate;
            FadeIn = Model.FadeIn;
            FadeOut = Model.FadeOut;
            IsLooped = Model.IsLooped;
            EchoIsEnabled = Model.EchoIsEnabled;
            EchoInterval = Model.EchoInterval;
        }


        new public AudioScreenObjModel Model { get; }

        public IAudioItem AudioItem { get; }

        public string FilePath { get; }


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
            else if (currentTime > (Length.Value / 60) - FadeOut.Value)
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
