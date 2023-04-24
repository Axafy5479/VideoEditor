using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Timeline.CustomControl
{
    public class TimelineScrollThumb : Border
    {
        public TimelineScrollThumb()
        {
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            EdgeNob_L.Value = this.ActualWidth/2 - 5;
            EdgeNob_R.Value = -this.ActualWidth/2 + 5;
        }

        public ReactiveProperty<double> EdgeNob_L { get; set; } = new();
        public ReactiveProperty<double> EdgeNob_R { get; set; } = new();

    }
}
