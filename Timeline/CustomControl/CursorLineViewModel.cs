using Data;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimeController;

namespace Timeline.CustomControl
{
    public class CursorLineViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public CursorLineViewModel()
        {
            Model = Pointer.Instance;

            CursorFrame = Model.CursorFrame;
            PixelPerFrame = Model.PixelPerFrame;
        }

        public Pointer Model { get; }

        public IReadOnlyReactiveProperty<int> CursorFrame { get; }
        public IReadOnlyReactiveProperty<double> PixelPerFrame { get; }
    }
}
