using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Timeline.CustomControl
{
    public class BlankRect : Grid
    {
        public BlankRect()
        {
            trn = new();
            IsHitTestVisible = false;
            this.RenderTransform = trn;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            Background = Brushes.White;
            Opacity = 0.3;
        }

        public int Layer { get; private set; }
        public int L { get; private set; }
        public int R { get; private set; }

        private TranslateTransform trn;

        private TimelineBase tlBase;
        private IReadOnlyReactiveProperty<double> PixelPerFrame { get; set; }
        public TimelineBase TLBase
        {
            get => tlBase;
            set
            {
                PixelPerFrame = value.MainDataContext.PixelPerFrame;
                PixelPerFrame.Subscribe(x => Move(Layer, L, R));
                this.Height = value.MainDataContext.LayerHeight;
                tlBase = value;
            }
        }

        public void Move(int layer, int l, int r)
        {
            if (TLBase == null)
            {
                return;
            }

            Layer = layer;
            L = l;
            R = r;

            trn.X = L * PixelPerFrame.Value;
            Width = (R - L) * PixelPerFrame.Value;

            if(this.Parent is Grid g)
            {
                g.Children.Remove(this);
            }

            TLBase.GetTLBaseObject(layer).Children.Add(this);
        }

    }
}
