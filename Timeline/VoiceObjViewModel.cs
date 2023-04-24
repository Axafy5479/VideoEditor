using AquestalkProj;
using Data;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class VoiceObjViewModel : TimelineItemObjectViewModel, ITextScreenObjVM
    {
        public VoiceObjViewModel(VoiceItem voiceItem, VoiceScreenObjModel model) : base(voiceItem, model)
        {
            Model = model;
            Zoom = Model.Zoom;
            X = Model.X;
            Y = Model.Y;

            FadeIn = Model.FadeIn;
            FadeOut = Model.FadeOut;

            Text = Model.Text;
            Color = Model.Color;
            Color2 = Model.Color2;
            FontSize = Model.FontSize;

            Serif = Model.Text;
            Pronuntition = Model.Pronuntiation;
            VoiceType = Model.VoiceType;
            Pitch = Model.Pitch;
            Speed = Model.Speed;
            Volume = Model.Volume;
        }

        public ReactiveProperty<string> Serif { get; }
        public ReactiveProperty<string> Pronuntition { get; }
        public ReactiveProperty<VoiceType> VoiceType { get; }
        public ReactiveProperty<double> Pitch { get; }
        public ReactiveProperty<double> Speed { get; }
        new public VoiceScreenObjModel Model { get; }
        public ReactiveProperty<double> Zoom { get; }
        public ReactiveProperty<double> X { get; }
        public ReactiveProperty<double> Y { get; }

        public IRenderingItem RenderingItem => Model.VoiceItem;



        public ReactiveProperty<string> Text { get; }
        public ReactiveProperty<SolidColorBrush> Color { get; }
        public ReactiveProperty<SolidColorBrush> Color2 { get; }
        public ReactiveProperty<double> FontSize { get; }






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
            double rest = (Length.Value / 60d) - currentTime;

            double ans = 0;
            if (currentTime < FadeIn.Value)
            {
                ans = 0.01 * Volume.Value * currentTime / FadeIn.Value;
            }
            else if (currentTime > (Length.Value / 60d) - FadeOut.Value)
            {
                ans = 0.01 * Volume.Value * rest / FadeOut.Value;
            }
            else
            {
                ans = 0.01 * Volume.Value;
            }

            return Math.Min(Math.Max(ans,0.00001),0.99999);
        }

    }
}
