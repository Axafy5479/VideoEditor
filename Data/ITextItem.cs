using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    internal interface ITextItem : IRenderingItem
    {
        public string Text { get; set; }
        public string Color { get; set; }
        public string Color2 { get; set; }
        public double FontSize { get; set; }
    }
}
