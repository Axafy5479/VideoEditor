using AquestalkProj;
using CommunityToolkit.Mvvm.ComponentModel;
using Data;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class VoiceScreenObjModel : TimelineItemObjectModel, IDisposable
    {
        public void Dispose()
        {
            Disposables.Dispose();
        }

        public VoiceScreenObjModel(VoiceItem voiceItem) : base(voiceItem)
        {
            VoiceItem = voiceItem;

            #region Observe Rendering Properties
            VoiceItem.ObserveProperty(x => x.Zoom).Subscribe(z => Zoom.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.FadeIn).Subscribe(z => FadeIn.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.FadeOut).Subscribe(z => FadeOut.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.X).Subscribe(z => X.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.Y).Subscribe(z => Y.Value = z).AddTo(Disposables);
            #endregion

            #region Observe Audio Properties
            VoiceItem.ObserveProperty(x => x.Text).Subscribe(z => Text.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.Color).Subscribe(z => Color.Value = z.ToBrushOrNull()).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.Color2).Subscribe(z => Color2.Value = z.ToBrushOrNull()).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.FontSize).Subscribe(z => FontSize.Value = z).AddTo(Disposables);
            #endregion

            VoiceItem.ObserveProperty(x => x.Pronuntiation).Subscribe(z=>Pronuntiation.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.VoiceType).Subscribe(z=> VoiceType.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.Pitch).Subscribe(z=> Pitch.Value = z).AddTo(Disposables);
            VoiceItem.ObserveProperty(x => x.Speed).Subscribe(z=> Speed.Value = z).AddTo(Disposables);
        }

        private CompositeDisposable Disposables { get; } = new();

        #region Rendering Properties
        public ReactiveProperty<double> Zoom { get; } = new();
        public ReactiveProperty<double> X { get; } = new();
        public ReactiveProperty<double> Y { get; } = new();
        //public ReactiveProperty<double> FadeIn { get; } = new();
        //public ReactiveProperty<double> FadeOut { get; } = new();
        #endregion

        #region Text Properties
        public ReactiveProperty<double> FontSize { get; } = new();
        public ReactiveProperty<SolidColorBrush> Color { get; } = new();
        public ReactiveProperty<SolidColorBrush> Color2 { get; } = new();
        public ReactiveProperty<string> Text { get; } = new();
        #endregion


        #region Audioプロパティ
        public ReactiveProperty<double> Volume { get; } = new(50d);
        public ReactiveProperty<double> PlaybackRate { get; } = new(100d);
        public ReactiveProperty<double> FadeIn { get; } = new(0d);
        public ReactiveProperty<double> FadeOut { get; } = new(0d);
        public ReactiveProperty<bool> IsLooped { get; } = new(false);
        public ReactiveProperty<bool> EchoIsEnabled { get; } = new(false);
        public ReactiveProperty<double> EchoInterval { get; } = new(0.1);
        #endregion

        #region Voice Item
        public ReactiveProperty<string> Pronuntiation { get; } = new();
        public ReactiveProperty<VoiceType> VoiceType { get; } = new();
        public ReactiveProperty<double> Pitch { get; } = new();
        public ReactiveProperty<double> Speed { get; } = new();
        #endregion

        public VoiceItem VoiceItem { get; } = new();
    }
}
