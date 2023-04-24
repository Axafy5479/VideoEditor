using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeController;

namespace Timeline.CustomControl
{
    public class TimelineLayerObjectViewModel
    {
        public ReactiveCollection<TimelineItemObject> Items { get; set; } = new();

        private void OnClick(object sender, MouseButtonEventArgs e)
        {

        }
    }


}
