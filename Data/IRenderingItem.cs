using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IRenderingItem : ITimelineItem
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Opacity { get; set; }
        public double Zoom { get; set; }
        public double Rotation { get; set; }
        public double FadeIn { get; set; }
        public double FadeOut { get; set; }
        public bool IsInverted { get; set; }
    }
}
