using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.Error;
using CommandProject.Commands;
using Data;

namespace Timeline.Commands
{
    public class Command_InsertLayer : RedoableCommandBase
    {
        public Command_InsertLayer(TimelineBase tlBase, int layer)
        {
            TLBase = tlBase;
            Layer = layer;
        }

        public TimelineBase TLBase { get; }
        public int Layer { get; }

        protected override ErrorInfo? _Redo()
        {
            return Execute();
        }

        protected override ErrorInfo? _Undo()
        {
            if (TimelineDataController.Instance.Items.Any(d => d.Layer == Layer))
            {
                return new(ErrorCode.CantDeleteLayerWithTLItem);
            }

            foreach (var item in TimelineDataController.Instance.Items)
            {
                if (item.Layer > Layer)
                {
                    //item.ChangeData(item.Frame, item.Length, item.Layer - 1, item.Offset);
                    item.Layer--;
                }
            }

            TLBase.MainDataContext.RemoveLastLayer();
            return null;
        }

        protected override ErrorInfo? _Execute()
        {
            TLBase.MainDataContext.AddLayer();
            foreach (var item in TimelineDataController.Instance.Items)
            {
                if (item.Layer >= Layer)
                {
                    //item.ChangeData(item.Frame, item.Length, item.Layer + 1, item.Offset);
                    item.Layer++;
                }
            }

            return null;
        }
    }
}
