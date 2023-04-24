using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ScreenProject
{
    public class TextScreenObject : Control, IScreenObject
    {
        static TextScreenObject()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextScreenObject), new FrameworkPropertyMetadata(typeof(TextScreenObject)));
        }

        public TextScreenObject(TextObjViewModel vm)
        {
            TlItemObjVM = vm;
            ViewModel = vm;
            DataContext = vm;
        }


        public TextObjViewModel ViewModel { get; }


        public Control Control => this;

        public ITimelineObjectViewModel TlItemObjVM { get; }




        public void Show(int globalFrame, bool isPlaying)
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }


        public bool IsPlaying => true;
    }
}
