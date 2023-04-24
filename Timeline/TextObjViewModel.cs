using Data;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class TextObjViewModel : TimelineItemObjectViewModel, ITextScreenObjVM
    {
        public TextObjViewModel(TextItem textItem, TextScreenObjModel model) : base(textItem, model)
        {
            Model = model;
            Zoom = Model.Zoom;
            X = Model.X;
            Y = Model.Y;

            FadeIn = Model.FadeIn;
            FadeOut = Model.FadeOut;

            Text = Model.Text;
            Color = Model.Color;
            Color2 = Model.Color2;
            FontSize = Model.FontSize;
        }

        public string FilePath { get; }
        new public TextScreenObjModel Model { get; }
        public ReactiveProperty<double> Zoom { get; }
        public ReactiveProperty<double> X { get; }
        public ReactiveProperty<double> Y { get; }

        public IRenderingItem RenderingItem => Model.TextItem;


        public ReactiveProperty<double> FadeIn { get; }
        public ReactiveProperty<double> FadeOut { get; }

        public ReactiveProperty<string> Text { get; }
        public ReactiveProperty<SolidColorBrush> Color { get; }
        public ReactiveProperty<SolidColorBrush> Color2 { get; }
        public ReactiveProperty<double> FontSize { get; }


    }
}
