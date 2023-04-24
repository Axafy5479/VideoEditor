using CommunityToolkit.Mvvm.ComponentModel;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TriangleColorPicker
{
    public partial class ColorPickerModel : ObservableObject
    {
        public ColorPickerModel(List<ReactiveProperty<SolidColorBrush>> property, int triangleLength)
        {
            MainProperty = property;
            Property = new(property[0].Value);
            TriangleLength = triangleLength;
        }

        public ReactiveProperty<double> MarkerX { get; } = new();
        public ReactiveProperty<double> MarkerY { get; } = new();
        public ReactiveProperty<SolidColorBrush> Property { get; }
        private List<ReactiveProperty<SolidColorBrush>> MainProperty { get; }


        public int TriangleLength { get; }

        public ReactiveProperty<double> TargetHue = new(0);
        public void HueChanged(double hue)
        {
            TargetHue.Value = hue;

            HsbColor c = ToHsb();
            HsbColor newColor = new HsbColor() { Hue = hue, Brightness = c.Brightness, Saturation = c.Saturation };
            var rgb = newColor.ToRgb();
            TargetHue.Value = hue;
            ColorChanged(rgb.R, rgb.G, rgb.B);
        }

        public void ColorChanged(byte r, byte g, byte b)
        {
            Color newColor = new Color() {A= Property.Value.Color.A, R = r, G = g, B = b };
            Property.Value = newColor.ToSolidColorBrush();
            HsbColor c = newColor.ToHsb();
            SB_to_MarkerPos(c.Saturation, c.Brightness);
        }

        public void Change(byte r, byte g, byte b)
        {
            Color newColor = new Color() { A = Property.Value.Color.A, R = r, G = g, B = b };
            Property.Value = newColor.ToSolidColorBrush();
            HsbColor c = newColor.ToHsb();
            if (r + g + b != 255 * 3 && r + g + b > 0)
            {
                TargetHue.Value = c.Hue;
            }
            SB_to_MarkerPos(c.Saturation, c.Brightness);
        }

        public void Change(Color newColor)
        {
            Change(newColor.R, newColor.G, newColor.B);
        }

        public Color ToColor()=> Property.Value.Color;
        public HsbColor ToHsb()=> ToColor().ToHsb();
        public Brush ToBrush()=> ToColor().ToSolidColorBrush();

        private void SB_to_MarkerPos(double s, double b)
        {
            
            var sqrt3 = Math.Sqrt(3);
            var a = TriangleLength;
            var h = TriangleLength * sqrt3 / 2;

            MarkerY.Value = h - s*h * b - a/sqrt3;

            MarkerX.Value = h*b * (2 - s) / sqrt3 - a/2;

        }


        public Subject<Unit> OnEndEditing { get; } = new();

        public void Decided()
        {
            foreach (var item in MainProperty)
            {
                item.Value = Property.Value;
            }
            OnEndEditing.OnNext(Unit.Default);
        }

        public void Canceled()
        {
            OnEndEditing.OnNext(Unit.Default);

        }
    }
}
