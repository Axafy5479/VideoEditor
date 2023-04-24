using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

namespace Timeline.CustomControl
{

    public class CursorLine : Control
    {
        static CursorLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CursorLine), new FrameworkPropertyMetadata(typeof(CursorLine)));
        }

        public CursorLine()
        {
            var vm = new CursorLineViewModel();
            DataContext = vm;
        }



    }
}
