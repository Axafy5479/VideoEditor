using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeController
{
    public interface IReadOnlyPointer
    {
        public ReactiveProperty<List<ITimelineObjectViewModel>> SelectedItem { get; }
    }
}
