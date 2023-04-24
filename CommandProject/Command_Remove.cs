using Data;
using System.Windows.Input;
using Timeline.Error;

namespace CommandProject.Commands
{
    public class Command_Remove : RedoableCommandBase<IItemObjectViewModel>
    {

        protected override ErrorInfo? _Redo() => _Execute();

        protected override ErrorInfo? _Undo()
        {
            foreach (var byteData in TargetsByteData)
            {
                (var newItem, var e) = TimelineItem.Deserialize(byteData);
                if (newItem == null) return e;
                else TimelineDataController.Instance.AddItem(newItem);
            }

            return null;
        }

        protected override ErrorInfo? _Execute()
        {
            foreach (var byteData in TargetsByteData)
            {
                (var item, var e) = TimelineItem.Deserialize(byteData);
                if (item == null) return e;

                if (TimelineDataController.Instance.RemoveItem(item.Layer, item.Frame, item.Length) is ErrorInfo e2)
                {
                    return e2;
                }
            }

            Mouse.Captured?.ReleaseMouseCapture();
            return null;
        }
    }

}
