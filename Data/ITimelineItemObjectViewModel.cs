using Reactive.Bindings;
using ScreenProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface ITimelineObjectViewModel : IItemObjectViewModel
    {
        ReactiveProperty<int> Layer { get; }
        ReactiveProperty<int> Frame { get; }
        ReactiveProperty<int> Length { get; }
        ReactiveProperty<int> OffsetFrame { get; }

        int R { get; }
    }
}
