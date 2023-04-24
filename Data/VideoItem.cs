using CommunityToolkit.Mvvm.ComponentModel;
using MessagePack;
using NAudioProj;
using OpenCVProj;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    [MessagePackObject(keyAsPropertyName: true)]
    public partial class VideoItem : TimelineItem, IRenderingItem, IAudioItem
    {
        public VideoItem(string filePath, int layer, int l, int _length) : base(Path.GetFileName(filePath), layer, l, _length)
        {
            FilePath = filePath;
        }

        [ObservableProperty] private string _FilePath = "DefaultName";
        [ObservableProperty] private double _Volume = 100;
        [ObservableProperty] private double _PlaybackRate = 100;

        public double ContentOffset
        {
            get => Offset / 60d;
            set => Offset = (int)Math.Round(value*60d);
        }

        public override FileType FileType => FileType.Video;

        [ObservableProperty] private bool _IsLooped = false;
        [ObservableProperty] private bool _EchoIsEnabled = false;
        [ObservableProperty] private double _EchoInterval = 0.1;

        #region 描画プロパティ
        [ObservableProperty] private double _X = 0;
        [ObservableProperty] private double _Y = 0;
        [ObservableProperty] private double _Opacity = 100;
        [ObservableProperty] private double _Zoom = 100;
        [ObservableProperty] private double _Rotation = 0;
        [ObservableProperty] private double _FadeIn = 0;
        [ObservableProperty] private double _FadeOut = 0;    
        [ObservableProperty] private bool _IsInverted = false;
        #endregion

        [SerializationConstructor]
        public VideoItem() : base() { }
    }
}
