
using CommunityToolkit.Mvvm.ComponentModel;
using MessagePack;
using System;
using System.IO;

namespace Data
{
    [MessagePackObject(keyAsPropertyName: true)]
    public partial class ImageItem : TimelineItem, IRenderingItem
    {
        public ImageItem(string filePath, int layer, int l, int _length) : base(Path.GetFileName(filePath), layer, l, _length)
        {
            FilePath = filePath;
        }

        [ObservableProperty] private string _FilePath = "DefaultName";

        public override FileType FileType => FileType.Image;

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
        public ImageItem() : base() { }
    }
}