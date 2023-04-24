using Data;
using FFMediaProject;
using NAudioProj;
using Reactive.Bindings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenProject
{
    public class ImageScreenObject : Control, IScreenObject
    {
        static ImageScreenObject()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageScreenObject), new FrameworkPropertyMetadata(typeof(ImageScreenObject)));
        }

        public ImageScreenObject(string filePath, ImageObjViewModel vm)
        {
            TlItemObjVM = vm;
            Uri = filePath;
            ViewModel = vm;
            DataContext = vm;
            ImageSource.Value = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            Width = ImageSource.Value.Width;
            Height = ImageSource.Value.Height;
            Loaded += Initialize;
        }


        public ImageObjViewModel ViewModel { get; }


        private void Initialize(object sender, RoutedEventArgs e)
        {
            //Width = FFMTVideo.Width;
            //Height = FFMTVideo.Height;
        }


        private VideoCapturer? VideoCapturer { get; set; }
        public bool Disposed { get; set; } = false;

        public Control Control => this;

        public ITimelineObjectViewModel TlItemObjVM { get; }

        public string Uri { get; }



        public void Show(int globalFrame, bool isPlaying)
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }

        public ReactiveProperty<BitmapSource> ImageSource { get; } = new();

        public bool IsPlaying => true;
    }
}
