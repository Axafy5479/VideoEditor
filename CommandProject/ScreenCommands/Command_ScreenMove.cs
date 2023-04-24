using CommandProject.Commands;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timeline.Error;

namespace CommandProject.ScreenCommands
{
    public class Command_ScreenMove : RedoableCommandBase<IRenderingScreenObjVM>
    {
        protected override List<byte[]>? SerializeAllItem(List<IRenderingScreenObjVM> targets)
        {
            var result = base.SerializeAllItem(targets);

            foreach (var item in targets)
            {
                MoveFrom.Add((item.RenderingItem.X, item.RenderingItem.Y));
                MoveTo.Add((item.X.Value, item.Y.Value));

                (var data, var e) = item.RenderingItem.Serialize();
                if (data is null) return null;

                TargetByteData.Add(data);
            }

            return result;
        }

        public override bool CanExecute(object? parameter)
        {
            return Pointer.Instance.SelectedVMs.All(mv => mv is IRenderingScreenObjVM);
        }

        public List<(double x, double y)> MoveTo { get; } = new();
        public List<(double x, double y)> MoveFrom { get; } = new();
        private List<byte[]> TargetByteData { get; } = new();

        protected override ErrorInfo? _Redo()
        {
            if (Error != null) return Error;
            throw new NotImplementedException();
        }

        protected override ErrorInfo? _Undo()
        {
            if (Error != null) return Error;
            throw new NotImplementedException();
        }

        protected override ErrorInfo? _Execute()
        {
            if (Error != null) return Error;


            for (int i = 0; i < TargetByteData.Count; i++)
            {
                var byteItem = TargetByteData[i];

                (var _item, var e) = TimelineItem.Deserialize(byteItem);
                if (_item is null) return e;

                (var item, var e2) = TimelineDataController.Instance.FindItemByPosition(_item.Layer, _item.Frame, _item.R);

                if (item is null) return e2;

                var renderItem = (IRenderingItem)item;
                renderItem.X = MoveTo[i].x;
                renderItem.Y = MoveTo[i].y;
            }

            return null;
        }
    }
}
