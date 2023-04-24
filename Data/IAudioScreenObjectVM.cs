using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IAudioScreenObjectVM : ITimelineObjectViewModel
    {
        public IAudioItem AudioItem { get; }

        ReactiveProperty<double> Volume { get; }
        ReactiveProperty<double> PlaybackRate { get; }

        ReactiveProperty<double> FadeIn { get; }
        ReactiveProperty<double> FadeOut { get; }
        ReactiveProperty<bool> IsLooped { get; }
        ReactiveProperty<bool> EchoIsEnabled { get; }
        ReactiveProperty<double> EchoInterval { get; }

        double GetCurrentVolumeRatio(int localFrame);
    }
}
