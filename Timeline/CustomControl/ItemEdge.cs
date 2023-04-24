using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Timeline.CustomControl
{
    public class ItemEdge : Border
    {
        public ItemEdge()
        {
            MouseLeftButtonDown += OnLeftMouseDown;
        }

        private void OnLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
