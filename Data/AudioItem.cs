using CommunityToolkit.Mvvm.ComponentModel;
using MessagePack;
using NAudioProj;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    [MessagePackObject(keyAsPropertyName: true)]
    public partial class AudioItem : TimelineItem, IAudioItem
    {
        public AudioItem(string filePath, int layer, int l, int _length) : base(Path.GetFileName(filePath), layer, l, _length)
        {
            FilePath = filePath;

        }

        [SerializationConstructor]
        public AudioItem() : base() { }

        [ObservableProperty] private string _FilePath;
        [ObservableProperty] private double _Volume = 50;
        [ObservableProperty] private double _PlaybackRate = 100;
        public double ContentOffset
        {
            get => Offset / 60d;
            set => Offset = (int)(Math.Round(value * 60));
        }

        public override FileType FileType => FileType.Audio;

        [ObservableProperty] private double _FadeIn = 0;
        [ObservableProperty] private double _FadeOut = 0;
        [ObservableProperty] private bool _IsLooped = false;
        [ObservableProperty] private bool _EchoIsEnabled = false;
        [ObservableProperty] private double _EchoInterval = 0.1;

    }
}
