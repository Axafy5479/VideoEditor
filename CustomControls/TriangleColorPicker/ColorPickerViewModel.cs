using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace TriangleColorPicker
{
    public class ColorPickerViewModel : IDisposable
    {
        private CompositeDisposable Disposables { get; } = new();

        public void Dispose()
        {
            Disposables.Dispose();
        }

        public ColorPickerViewModel()
        {
            MarkerX = new();
            MarkerY = new();
            TargetHue = new();
            DrawHueRing();

        }

        public void Initialize(List<ReactiveProperty<SolidColorBrush>> property)
        {
            Model = new ColorPickerModel(property, TriWidth);

            Triangle.Value = new WriteableBitmap(
            BitmapSource.Create(TriWidth, TriHeight, 96, 96, PixelFormats.Bgra32, null, new byte[TriWidth * TriHeight * 4], TriWidth * 4)
            );

            Model.MarkerX.Subscribe(x=>MarkerX.Value = x);
            Model.MarkerY.Subscribe(y=>MarkerY.Value = y);
            Model.TargetHue.Subscribe(h=>TargetHue.Value=h);
            TargetHue.Subscribe(h => UpdateTriangle(h));

            Model.Property.Subscribe(_=> ModelPropertyChanged()).AddTo(Disposables);

            DrawHueRing();
        }

        public ColorPickerModel Model { get; private set; }
        public ReactiveProperty<Brush> SelectedBrush { get;  } = new();

        public ReactiveProperty<double> TargetHue { get; } = new();

        public int Radius { get; } = 150;
        public int InnerRadius { get; } = 100;
        public int TriWidth => (int)(InnerRadius * 0.90 * Math.Pow(3, 0.5)/2);
        public int TriHeight => (int)(TriWidth * 2/ Math.Pow(3, 0.5));

        public ReactiveProperty<double> MarkerX { get; } = new();
        public ReactiveProperty<double> MarkerY{ get; } = new();

        public void ModelPropertyChanged()
        {
            SelectedBrush.Value = Model.ToBrush();
            UpdateTriangle(Model.TargetHue.Value);
        }

        public void HueChanged(double hue)
        {
            Model.HueChanged(hue);
        }


        #region Hue Ring
        public ReactiveProperty<BitmapSource> HueRingSource { get; } = new();

        public int Width { get;private set; }
        public int Height { get;private set; }

        private void DrawHueRing()
        {
            int width = (int)(Radius * 1.03), height = (int)(Radius * 1.03), bytesperpixel = 4;
            int stride = width * bytesperpixel;
            byte[] imgdata = new byte[width * height * bytesperpixel];

            // draw a gradient from red to green from top to bottom (R00 -> ff; Gff -> 00)
            // draw a gradient of alpha from left to right
            // Blue constant at 00
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    double x = (col - width / 2.0) / (width / 2);
                    double y = (row - height / 2.0) / (height / 2);
                    double r = Math.Pow(x * x + y * y, 0.5);
                    double theta = (Math.Atan2(x, y) + Math.PI) * 180 / Math.PI;

                    Color c = new HsbColor() { Hue = theta, Brightness = 1, Saturation = 1 }.ToRgb();

                    // BGRA
                    //imgdata[row * stride + col * 4 + 0] = 0;
                    //imgdata[row * stride + col * 4 + 1] = Convert.ToByte((1 - (col / (float)width)) * 0xff);
                    //imgdata[row * stride + col * 4 + 2] = Convert.ToByte((col / (float)width) * 0xff);
                    //imgdata[row * stride + col * 4 + 3] = Convert.ToByte((row / (float)height) * 0xff);

                    imgdata[row * stride + col * 4 + 0] = Convert.ToByte(c.B);
                    imgdata[row * stride + col * 4 + 1] = Convert.ToByte(c.G);
                    imgdata[row * stride + col * 4 + 2] = Convert.ToByte(c.R);
                    imgdata[row * stride + col * 4 + 3] = Convert.ToByte(0xff * CalA(r));

                }
            }
            var gradient = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, imgdata, stride);
            Width = width; Height = height;
            HueRingSource.Value = gradient;
        }
        private double CalA(double r)
        {
            double r_ratio = r;
            var inner = InnerRadius / (double)(Radius);

            double alpha;
            if (r_ratio < inner - 0.01) alpha = 0;
            else if (r_ratio < inner + 0.01) alpha = (r_ratio - (inner - 0.01)) / 0.02;
            else if (r_ratio < 0.98) return 1;
            else if(r_ratio<1) alpha = (1 - r_ratio) / 0.02;
            else return 0;

            return alpha;
        }
        #endregion


        #region Triangle
        public ReactiveProperty<WriteableBitmap> Triangle { get; } = new();
        public unsafe void UpdateTriangle(double hue)
        {

            var tex = Triangle.Value;
            tex.Lock();
            var stride = tex.BackBufferStride;
            var p_buf = (byte*)tex.BackBuffer;

            double sqrt3 = Math.Pow(3, 0.5);
            int triangleHeight =  (int)((sqrt3 / 2) * tex.Width);

            for (int row = 0; row < tex.Height; row++)
            {

                for (int col = 0; col < tex.Width; col++)
                {
                    var brightness = (triangleHeight - row + sqrt3 * col) / 2;
                    brightness /= triangleHeight;
                    brightness = Math.Min(Math.Max(brightness, 0), 1);


                    //var saturation = (triangleHeight-row) / (double)triangleHeight;
                    var saturation = (2 * triangleHeight - 2 * row) / (sqrt3 * col + triangleHeight - row);
                    //saturation = 1;

                    saturation = Math.Min(Math.Max(saturation, 0), 1);

                    //var d = brightness - saturation;
                    //if (d<0)
                    //{
                    //    brightness -= d;
                    //}

                    Color c = new HsbColor() { Hue = hue, Brightness = brightness, Saturation = saturation }.ToRgb();

                    // BGRA
                    p_buf[row * stride + col * 4 + 0] = Convert.ToByte(c.B);
                    p_buf[row * stride + col * 4 + 1] = Convert.ToByte(c.G);
                    p_buf[row * stride + col * 4 + 2] = Convert.ToByte(c.R);
                    p_buf[row * stride + col * 4 + 3] = Convert.ToByte(0xff*CalTriA(row,col,triangleHeight,sqrt3));
                }
            }
            tex.AddDirtyRect(new Int32Rect() { Width =(int)tex.Width, Height = (int)tex.Height });
            tex.Unlock();
            Triangle.Value = null;
            Triangle.Value = tex;
        }

        private double CalTriA(double row, double col, double height, double sqrt3)
        {
            int threshold_ll = (int)((height - row) / sqrt3)-1;
            int threshold_lr = (int)((height - row) / sqrt3);
            int threshold_rl = TriWidth - threshold_lr;
            int threshold_rr = TriWidth - threshold_lr + 1;

            double ans = 0;

            if (row == height + 1) return 0.5;
            if (row > height + 1) return 0;

            if (col <= threshold_ll) ans = 0;
            else if (col < threshold_lr) ans = 0;
            else if (col < threshold_rl) ans = 1;
            else if (col <= threshold_rr) ans = 0;
            else ans = 0;
            return ans;
        }
        #endregion
    }
}
