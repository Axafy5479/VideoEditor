using CommunityToolkit.Mvvm.ComponentModel;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Data
{
    [MessagePackObject(keyAsPropertyName: true)]
    public partial class TextItem : TimelineItem, ITextItem
    {
        public TextItem(int layer, int l, int _length) : base("", layer, l, _length) { }

        [SerializationConstructor]
        public TextItem() : base() { }

        public double ContentOffset
        {
            get => Offset / 60d;
            set => Offset = (int)Math.Round(value * 60d);
        }

        public override FileType FileType => FileType.Text;


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

        #region Textプロパティ
        [ObservableProperty] private string _Text = "";
        [ObservableProperty] private string _Color = "#ffffff";
        [ObservableProperty] private string _Color2 = "#000000";
        [ObservableProperty] private double _FontSize = 20;
        #endregion



    }
}




