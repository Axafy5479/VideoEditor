using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IAudioItem : ITimelineItem
    {
        double Volume { get; set; }
        double PlaybackRate { get; set; }

        double FadeIn { get; set; }
        double FadeOut { get; set; }
        bool IsLooped { get; set; }
        bool EchoIsEnabled { get; set; }
        double EchoInterval { get; set; }
    }
}
