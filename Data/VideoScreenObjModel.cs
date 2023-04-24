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
using Timeline.CustomControl;

namespace ScreenProject
{
    public class VideoScreenObjModel : TimelineItemObjectModel, IDisposable
    {
        public void Dispose()
        {
            Disposables.Dispose();
        }

        public VideoScreenObjModel(VideoItem videoItem) : base(videoItem)
        {
            VideoItem = videoItem;

            #region Observe Rendering Properties
            VideoItem.ObserveProperty(x => x.Zoom).Subscribe(z=>Zoom.Value = z).AddTo(Disposables); ;
            VideoItem.ObserveProperty(x => x.X).Subscribe(z=>X.Value = z).AddTo(Disposables); ;
            VideoItem.ObserveProperty(x => x.Y).Subscribe(z=>Y.Value = z).AddTo(Disposables); ;
            #endregion

            #region Observe Audio Properties
            VideoItem.ObserveProperty(x => x.Volume).Subscribe(z => Volume.Value = z).AddTo(Disposables);
            VideoItem.ObserveProperty(x => x.PlaybackRate).Subscribe(z => PlaybackRate.Value = z).AddTo(Disposables);
            VideoItem.ObserveProperty(x => x.FadeIn).Subscribe(z => FadeIn.Value = z).AddTo(Disposables);
            VideoItem.ObserveProperty(x => x.FadeOut).Subscribe(z => FadeOut.Value = z).AddTo(Disposables);
            VideoItem.ObserveProperty(x => x.IsLooped).Subscribe(z => IsLooped.Value = z).AddTo(Disposables);
            VideoItem.ObserveProperty(x => x.EchoIsEnabled).Subscribe(z => EchoIsEnabled.Value = z).AddTo(Disposables);
            VideoItem.ObserveProperty(x => x.EchoInterval).Subscribe(z => EchoInterval.Value = z).AddTo(Disposables);
            #endregion
        }

        private CompositeDisposable Disposables { get; } = new();

        #region Rendering Properties
        public ReactiveProperty<double> Zoom { get; } = new();
        public ReactiveProperty<double> X { get; } = new();
        public ReactiveProperty<double> Y { get; } = new();
        #endregion

        #region Audio Properties
        public ReactiveProperty<double> Volume { get; } = new();
        public ReactiveProperty<double> PlaybackRate { get; } = new();
        public ReactiveProperty<double> FadeIn { get; } = new();
        public ReactiveProperty<double> FadeOut { get; } = new();
        public ReactiveProperty<bool> IsLooped { get; } = new();
        public ReactiveProperty<bool> EchoIsEnabled { get; } = new();
        public ReactiveProperty<double> EchoInterval { get; } = new();
        #endregion

        public VideoItem VideoItem { get; } = new();
    }
}
