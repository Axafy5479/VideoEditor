using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ParameterEditor.ColorpickerWindow
{
    public class ColorPickerWinViewModel
    {
        public ReactiveProperty<double> X { get; } = new();
        public ReactiveProperty<double> Y { get; } = new();
        public IReadOnlyReactiveProperty<SolidColorBrush> SelectedColor { get; }
        public ColorPalette? Palette { get; internal set; }

        public void SetProperty(List<ReactiveProperty<SolidColorBrush>> property)
        {
            if (Palette == null) throw new Exception("パレットが設定される前に侘＾ゲットプロパティを設定使用としました");
            Palette?.SetProperty(property);
        }
    }
}
