using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.Error;

namespace Data
{
    public interface ITimelineItem
    {
        int Layer { get; }
        int Frame { get; }
        int Length { get; }
        int Offset { get; }
        (byte[]? ,ErrorInfo?) Serialize();
    }
}
