using CommunityToolkit.Mvvm.ComponentModel;
using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Data
{
    public enum ColorEnum
    {
        Background,
        Foreground,
        SubForeground,
        CursorLine,
        Layer0,
        Layer1,
        Accent,
        Border
    }

    public enum FileType
    {
        Video,
        Audio,
        Text,
        Image,
        Yukkuri,
        Shape,
    }



    public partial class ColorManager : ObservableObject
    {
        internal static readonly ColorDataTemplate LightTheme =
            new ColorDataTemplate
            (
                background: "#d1d1d1".ToBrushOrNull(),
                foreground: Brushes.Black,
                subForeground: "#5c5c5c".ToBrushOrNull(),
                cursorLine: Brushes.Orange,
                layer0: "#d1d1d1".ToBrushOrNull(),
                layer1: "#ababab".ToBrushOrNull(),
                accent: "#ffbe99".ToBrushOrNull(),
                border: "#5c5c5c".ToBrushOrNull()
            );

        internal static readonly ColorDataTemplate DarkTheme = new ColorDataTemplate
            (
                background:"#222222".ToBrushOrNull(),
                foreground:"#bfbfbf".ToBrushOrNull(),
                subForeground: "#878787".ToBrushOrNull(),
                cursorLine:Brushes.Orange,
                layer0:"#222222".ToBrushOrNull(),
                 layer1:"#111111".ToBrushOrNull(),
                 accent:"#3b1e00".ToBrushOrNull(),
                 border:"#5c5c5c".ToBrushOrNull()
            );

        internal static readonly ColorDataItemTemplate DefaultItemColor = new ColorDataItemTemplate(new()
        {
            {FileType.Video, "#29708c" },
            {FileType.Audio, "#757523" },
            {FileType.Text, "#61615f" },
            {FileType.Image, "#214f20" },
            {FileType.Yukkuri, "#000000" },
            {FileType.Shape, "#6850a3" }
        });


        private ColorManager()
        {
            var theme = DarkTheme;
            var itemTheme = DefaultItemColor;

            Background = theme.Background;
            Foreground = theme.Foreground;
            _SubForeground = theme.SubForeground;
            Layer0 = theme.Layer0;
            Layer1 = theme.Layer1;
            Accent = theme.Accent;
            CursorLine = theme.CursorLine;
            Border = theme.Border;

            TLItemTheme = DefaultItemColor.ItemColorMap.ToDictionary(d=>d.Key,d=>new ReactiveProperty<Brush>(d.Value.ToBrushOrNull()));
        }
        private static ColorManager? instance;
        public static ColorManager Instance=>instance ??= new();



        [ObservableProperty] private Brush _Background;
        [ObservableProperty] private Brush _Foreground;
        [ObservableProperty] private Brush _SubForeground;
        [ObservableProperty] private Brush _Layer0;
        [ObservableProperty] private Brush _Layer1;
        [ObservableProperty] private Brush _Accent;
        [ObservableProperty] private Brush _CursorLine;
        [ObservableProperty] private Brush _Border;


        public Dictionary<FileType, ReactiveProperty<Brush>> TLItemTheme { get; } = new();
    }


    internal class ColorDataTemplate
    {
        public ColorDataTemplate(Brush background, Brush foreground, Brush subForeground, Brush cursorLine, Brush layer0, Brush layer1, Brush accent, Brush border)
        {
            Background = background;
            Foreground = foreground;
            SubForeground = subForeground;
            CursorLine = cursorLine;
            Layer0 = layer0;
            Layer1 = layer1;
            Accent = accent;
            Border = border;
        }

        public Brush Background { get; set; }
        public Brush Foreground { get; set; } 
        public Brush SubForeground { get; set; }
        public Brush CursorLine { get; set; }
        public Brush Layer0 { get; set; }
        public Brush Layer1 { get; set; }
        public Brush Accent { get; set; }
        public Brush Border { get; set; }
    }


    internal class ColorDataItemTemplate
    {
        public Dictionary<FileType, string> ItemColorMap { get; }

        public ColorDataItemTemplate(Dictionary<FileType, string> itemColorMap)
        {
            ItemColorMap = itemColorMap;
        }
    }


}
