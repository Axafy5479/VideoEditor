using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeController
{
    public interface ITimelineItemObject
    {
        public int L { get; }
        public int R { get; }
        public int Layer { get; }
        public int Length { get; }
        public int OffsetFrame { get; }
        public TimelineItem ItemData { get; }
    }

}
