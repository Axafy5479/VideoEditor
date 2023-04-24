using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Data
{
    public interface IItemObjectViewModel
    {
        TimelineItem ItemData { get; }
    }

    public interface IRenderingScreenObjVM : IItemObjectViewModel
    {
        public IRenderingItem RenderingItem { get; }
        public ReactiveProperty<double> Zoom { get; }
        public ReactiveProperty<double> X { get; }
        public ReactiveProperty<double> Y { get; }
    }

    public interface ITextScreenObjVM : IRenderingScreenObjVM
    {
        public ReactiveProperty<double> FontSize { get; }
        public ReactiveProperty<SolidColorBrush> Color { get; }
        public ReactiveProperty<SolidColorBrush> Color2 { get; }
        public ReactiveProperty<string> Text { get; }
    }
}
