using Reactive.Bindings.Extensions;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.CustomControl;
using System.Reactive.Disposables;

namespace Data
{
    public class AudioScreenObjModel : TimelineItemObjectModel, IDisposable
    {
        public void Dispose()
        {
            Disposables.Dispose();
        }

        public AudioScreenObjModel(AudioItem audioItem) : base(audioItem)
        {
            Disposables = new();
            AudioItem = audioItem;
            AudioItem.ObserveProperty(x => x.Volume).Subscribe(z => Volume.Value = z).AddTo(Disposables);
            AudioItem.ObserveProperty(x => x.PlaybackRate).Subscribe(z => PlaybackRate.Value = z).AddTo(Disposables);
            AudioItem.ObserveProperty(x => x.FadeIn).Subscribe(z => FadeIn.Value = z).AddTo(Disposables);
            AudioItem.ObserveProperty(x => x.FadeOut).Subscribe(z => FadeOut.Value = z).AddTo(Disposables);
            AudioItem.ObserveProperty(x => x.IsLooped).Subscribe(z => IsLooped.Value = z).AddTo(Disposables);
            AudioItem.ObserveProperty(x => x.EchoIsEnabled).Subscribe(z => EchoIsEnabled.Value = z).AddTo(Disposables);
            AudioItem.ObserveProperty(x => x.EchoInterval).Subscribe(z => EchoInterval.Value = z).AddTo(Disposables);

        }

        private CompositeDisposable Disposables { get; }

        public ReactiveProperty<double> Volume { get; } = new(50d);
        public ReactiveProperty<double> PlaybackRate { get; } = new(100d);
        public ReactiveProperty<double> FadeIn { get; } = new(0d);
        public ReactiveProperty<double> FadeOut { get; } = new(0d);
        public ReactiveProperty<bool> IsLooped { get; } = new(false);
        public ReactiveProperty<bool> EchoIsEnabled { get; } = new(false);
        public ReactiveProperty<double> EchoInterval { get; } = new(0.1);

        public AudioItem AudioItem { get; }
    }
}
