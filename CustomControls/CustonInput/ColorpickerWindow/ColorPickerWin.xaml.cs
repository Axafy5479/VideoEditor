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
using System.Windows.Shapes;

namespace ParameterEditor.ColorpickerWindow
{
    /// <summary>
    /// ColorPickerWin.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorPickerWin : Window
    {
        public ColorPickerWin()
        {
            InitializeComponent();

            var pallete = (ColorPalette)this.FindName("Palette");
            ViewModel.Palette = pallete;
            pallete.OnEndEditing.Subscribe(_ =>this.Close());
            this.CaptureMouse();
            this.Focus();

            Application.Current.MainWindow.PreviewMouseDown += Clicked;
        }

        private void Clicked(object sender, MouseEventArgs e)
        {
            if(!this.IsMouseOver)
            {
                ViewModel.Palette?.OnCancelButtonClicked();

            }
        }

        public ColorPickerWinViewModel ViewModel => (ColorPickerWinViewModel)DataContext;

    }
}
