using Data;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.Error;

namespace Timeline.CustomControl
{
    public abstract class TimelineItemObjectModel : BindableBase
    {
        public TimelineItemObjectModel(TimelineItem tlItem)
        {
            itemName = tlItem.ItemName;
            Frame = tlItem.Frame;
            Layer = tlItem.Layer;
            Length = tlItem.Length;

            tlItem.ObserveProperty(x => x.Frame).Subscribe(f=>Frame = f);
            tlItem.ObserveProperty(x => x.Layer).Subscribe(layer=>Layer = layer);
            tlItem.ObserveProperty(x => x.Length).Subscribe(length=>Length = length);
            tlItem.ObserveProperty(x => x.ItemName).Subscribe(name=>ItemName = name);
            tlItem.ObserveProperty(x => x.Offset).Subscribe(offset=>OffsetFrame = offset);
            ItemData = tlItem;
        }

        private string itemName = "NoName";
        public string ItemName { get => itemName; set => SetProperty(ref itemName, value); }

        private int frame;
        public int Frame { get => frame; set => SetProperty(ref frame, value); }

        private int length;
        public int Length { get => length; set => SetProperty(ref length, value); }

        private int layer;
        public int Layer { get => layer; set => SetProperty(ref layer, value); }

        private int offsetFrame;
        public int OffsetFrame { get => offsetFrame; set => SetProperty(ref offsetFrame, value); }


        public int R => Frame + Length;

        public TimelineItem ItemData { get; }

        public void Move(int l)
        {
            if (l >= 0)
            {
                MoveLR(l, l + Length, OffsetFrame);
            }
        }
        public void OnTLSizeChanged() => MoveLR(Frame, Frame + Length, OffsetFrame);

        public void MoveLR(int l, int r, int offsetFrame)
        {
            Frame = Math.Max(l, 0);
            Length = Math.Max(1, r - l);
            OffsetFrame = offsetFrame;
            TimelineDataController.Instance.TLObjectMoved(this);
        }



    }
}
