using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandProject.Commands;
using System.Windows.Input;
using Timeline.Error;
using TimeController;

namespace CommandProject.Commands
{
    public class Command_RemoveAndFillBlank : RedoableCommandBase
    {
        public override bool CanExecute(object? parameter)
        {
            bool condition1 = Pointer.Instance.SelectedBlank.Value.layer >= 0;
            bool condition2 = Pointer.Instance.SelectedVMs.Any();
            return condition1 || condition2;
        }

        private ICommandInfo? Info { get; set; }

        protected override ErrorInfo? _Redo()
        {
            return Info is not null ? Info.Redo() : new ErrorInfo(ErrorCode.CommandInfoNull);
        }

        protected override ErrorInfo? _Undo()
        {
            return Info is not null ? Info.Undo() : new ErrorInfo(ErrorCode.CommandInfoNull);
        }

        protected override ErrorInfo? _Execute()
        {
            if (Pointer.Instance.SelectedBlank.Value.layer >= 0)
            {
                Info = new RemoveBlankInfo(Pointer.Instance.SelectedBlank.Value);
            }
            else
            {
                Info = new RemoveFillCommandInfo(Pointer.Instance.SelectedItem.Value);
            }
            Info.Execute();

            return null;

        }
    }


    public interface ICommandInfo
    {
        ErrorInfo? Execute();
        ErrorInfo? Redo();
        ErrorInfo? Undo();
    }

    public class RemoveFillCommandInfo : ICommandInfo
    {
        public RemoveFillCommandInfo(List<ITimelineObjectViewModel> tlItems)
        {
            N = tlItems.Count;

            foreach (var item in tlItems)
            {
                (var serializedData, var e) = item.ItemData.Serialize();
                if (serializedData == null)
                {
                    ErrorInfo = e;
                    return;
                }
                else
                {
                    ByteDatas.Add(serializedData);
                }
            }

            Dictionary<int, List<ITimelineObjectViewModel>> layer_removingItem_map = new();

            foreach (var item in tlItems)
            {
                if (!layer_removingItem_map.ContainsKey(item.Layer.Value))
                {
                    layer_removingItem_map.Add(item.Layer.Value, new());
                }

                layer_removingItem_map[item.Layer.Value].Add(item);
            }

            foreach (var layer_removingItem in layer_removingItem_map)
            {
                int layer = layer_removingItem.Key;
                var removingItems = new HashSet<(int L, int R)>(layer_removingItem.Value.ConvertAll(d=>(d.Frame.Value, d.Frame.Value + d.Length.Value)));

                var layerItems = TimelineDataController.Instance.Items.FindAll(d => d.Layer == layer);
                layerItems.Sort((a, b) => a.Frame - b.Frame);

                int delta = 0;
                for (int i = 0; i < layerItems.Count; i++)
                {
                    var item = layerItems[i];
                    if (removingItems.Contains((item.Frame, item.R)))
                    {
                        delta += item.Length;
                    }
                    else
                    {
                        if (delta > 0)
                        {
                            FillTargets_WhenRemove.Add((layer, item.Frame, item.R), delta);
                        }
                    }
                }
            }


        }

        public ErrorInfo? ErrorInfo { get; }
        public int N { get; }
        private List<byte[]> ByteDatas { get; } = new();

        private Dictionary<(int layer, int l, int r), int> FillTargets_WhenRemove { get; } = new();

        public ErrorInfo? Execute()
        {
            if (ErrorInfo != null)
            {
                return ErrorInfo;
            }

            for (int i = 0; i < N; i++)
            {
                var byteData = ByteDatas[i];
                (var item, var e) = TimelineItem.Deserialize(byteData);
                if (item == null)
                {
                    return e;
                }

                if (TimelineDataController.Instance.RemoveItem(item.Layer, item.Frame, item.Length) is ErrorInfo e2)
                {
                    return e2;
                }
            }

            foreach (var item_delta in FillTargets_WhenRemove)
            {
                (var item, var e) = TimelineDataController.Instance.FindItemByPosition(item_delta.Key.layer, item_delta.Key.l, item_delta.Key.r);
                if (item == null) return e;

                //item.ChangeData(item.Frame - item_delta.Value, item.Length, item.Layer, item.Offset);
                item.Frame -= item_delta.Value;
            }

            Mouse.Captured?.ReleaseMouseCapture();
            return null;

        }



        public ErrorInfo? Redo()
        {
            if (ErrorInfo != null)
            {
                return ErrorInfo;
            }

            return Execute();
        }

        public ErrorInfo? Undo()
        {
            if (ErrorInfo != null)
            {
                return ErrorInfo;
            }

            foreach (var pos_delta in FillTargets_WhenRemove)
            {
                (var layer, var l, var r) = pos_delta.Key;
                int delta = pos_delta.Value;

                (var item,var e)=TimelineDataController.Instance.FindItemByPosition(layer, l-delta, r-delta);
                if (item is null) return e;

                //item.ChangeData(l,item.Length, item.Layer, item.Offset);
                item.Frame = l;
            }

            for (int i = 0; i < N; i++)
            {
                var byteData = ByteDatas[i];

                (var newItem, var e) = TimelineItem.Deserialize(byteData);
                if (newItem == null)
                {
                    return e;
                }
                else
                {
                    TimelineDataController.Instance.AddItem(newItem);
                }
            }

            return null;
        }
    }

    public class RemoveBlankInfo : ICommandInfo
    {
        public RemoveBlankInfo((int layer, int l,int r) blank)
        {
            Blank = blank;
            Layer = blank.layer;
            Delta = blank.r - blank.l;

            foreach (var item in TimelineDataController.Instance.Items)
            {
                if(item.Layer == blank.layer && item.Frame>=blank.r)
                {
                    FillTargets_WhenRemove.Add((item.Frame, item.R));
                }
            }

            FillTargets_WhenRemove.Sort((a, b) => a.l - b.l);
        }

        private int Layer { get; }
        private int Delta { get; }
        public (int layer, int l, int r) Blank { get; }
        public ErrorInfo? ErrorInfo { get; }

        private List<(int l,int r)> FillTargets_WhenRemove { get; } = new();

        public ErrorInfo? Execute()
        {
            foreach (var (l, r) in FillTargets_WhenRemove)
            {
                (var item, var e) = TimelineDataController.Instance.FindItemByPosition(Layer, l, r);
                if (item is null) return e;

                //item.ChangeData(l - Delta, item.Length, Layer, item.Offset);
                item.Frame = l - Delta;
            }
            
            Mouse.Captured?.ReleaseMouseCapture();
            return null;

        }



        public ErrorInfo? Redo()
        {
            if (ErrorInfo != null)
            {
                return ErrorInfo;
            }

            return Execute();
        }

        public ErrorInfo? Undo()
        {
            foreach (var (l, r) in FillTargets_WhenRemove)
            {
                (var item, var e) = TimelineDataController.Instance.FindItemByPosition(Layer, l - Delta, r - Delta);
                if (item is null) return e;

                //item.ChangeData(l, item.Length, Layer, item.Offset);
                item.Frame = l;
            }

            Mouse.Captured?.ReleaseMouseCapture();

            return null;
        }
    }
}
