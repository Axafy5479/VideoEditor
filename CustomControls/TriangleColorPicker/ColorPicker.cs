using CommunityToolkit.Mvvm.Input;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TriangleColorPicker
{
    public class ColorPicker : Control, IDisposable
    {
        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));

        }

        public ColorPicker()
        {
            ViewModel = new ColorPickerViewModel();
            DataContext = ViewModel;
            MoveCommand = new RelayCommand(OnMouseMove);
            UpCommand = new RelayCommand(OnMouseLeftButtonUp);
            DownCommand = new RelayCommand(OnMouseLeftButtonDown);

            TriDownCommand = new RelayCommand(TriOnMouseLeftButtonDown);
            TriUpCommand = new RelayCommand(TriOnMouseLeftButtonUp);
            TriMoveCommand = new RelayCommand(TriOnMouseMove);



        }

        public void SetAndShowPicker(List<ReactiveProperty<SolidColorBrush>> picker)
        {
            if (IsLoaded)
            {
                ViewModel.Initialize(picker);
                ViewModel.Model.Change(picker[0].Value.Color);
            }
            else
            {
                Loaded += (s, e) =>
                {
                    ViewModel.Initialize(picker);
                    ViewModel.Model.Change(picker[0].Value.Color);
                };
            }
        }

        public ColorPickerViewModel ViewModel { get; }

        private Image? ringImage;
        public Image RingImage => ringImage ??= (Image)this.Template.FindName("Ring",this);

        private Image? triangleImage;
        public Image TriangleImage => triangleImage ??= (Image)this.Template.FindName("Triangle", this);

        public ICommand MoveCommand { get; set; }
        public ICommand UpCommand { get; set; }
        public ICommand DownCommand { get; set; }

        public ICommand TriMoveCommand { get; set; }
        public ICommand TriUpCommand { get; set; }
        public ICommand TriDownCommand { get; set; }


        private void OnMouseMove()
        {
            if (!RingImage.IsMouseCaptured) return;
            ChangeTargetHue();
        }


        private void OnMouseLeftButtonDown()
        {
            RingImage.CaptureMouse();
            OnMouseMove();
        }
        private void OnMouseLeftButtonUp()
        {
            RingImage.ReleaseMouseCapture();
        }
        private void ChangeTargetHue()
        {
            var source = (BitmapSource)RingImage.Source;

            var pos = Mouse.GetPosition(RingImage);
            int x = (int)(pos.X);
            int y = (int)(pos.Y);

            if (x >= source.PixelWidth || x < 0 || y >= source.PixelHeight || y < 0) return;

            // Copy the single pixel into a new byte array representing RGBA
            var pixel = new byte[4];

            source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            Color c = new Color() { R = pixel[2], G = pixel[1], B = pixel[0] };
            ViewModel.HueChanged(c.ToHsb().Hue);
        }




        private void TriOnMouseLeftButtonDown()
        {
            TriangleImage.CaptureMouse();
            TriOnMouseMove();
        }


        private void TriOnMouseMove()
        {
            if (!TriangleImage.IsMouseCaptured) return;

            var source = (BitmapSource)TriangleImage.Source;

            var pos = Mouse.GetPosition(TriangleImage);
            int x = (int)(pos.X);
            int y = (int)(pos.Y);

            if (x >= source.PixelWidth || x < 0 || y >= source.PixelHeight || y < 0) return;

            // Copy the single pixel into a new byte array representing RGBA
            var pixel = new byte[4];

            source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            ViewModel.Model.ColorChanged(pixel[2], pixel[1], pixel[0]);
            //Debug.WriteLine(pixel[2] + "," + pixel[1] + "," + pixel[0]);
        }


        private void TriOnMouseLeftButtonUp()
        {
            TriangleImage.ReleaseMouseCapture();
        }

        public void Dispose()
        {
            ViewModel.Dispose();
        }



        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
    "Color", // プロパティ名を指定
    typeof(ReactiveProperty<SolidColorBrush>), // プロパティの型を指定
    typeof(ColorPicker), // プロパティを所有する型を指定
    new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる

        public ReactiveProperty<SolidColorBrush> SolidColor
        {
            get => (ReactiveProperty<SolidColorBrush>)(this.GetValue(ColorProperty));
            set => this.SetValue(ColorProperty, value);
        }
    }
}
