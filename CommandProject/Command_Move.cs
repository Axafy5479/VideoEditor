using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommandProject.Commands;
using Timeline.Error;
using TimeController;

namespace CommandProject.Commands
{
    public class Command_Move : RedoableCommandBase<ITimelineObjectViewModel>
    {

        protected override List<byte[]>? SerializeAllItem(List<ITimelineObjectViewModel> targets)
        {
            N = targets.Count;

            layer_frame_length_offset__from = new();
            layer_frame_length_offset__to = new();
            foreach (var item in targets)
            {
                layer_frame_length_offset__from.Add(new[] { item.ItemData.Layer, item.ItemData.Frame, item.ItemData.Length, item.ItemData.Offset });
                layer_frame_length_offset__to.Add(new[] { item.Layer.Value, item.Frame.Value, item.Length.Value, item.OffsetFrame.Value });
            }

            return base.SerializeAllItem(targets);
        }

        public int N { get; private set; }
        private List<int[]> layer_frame_length_offset__from { get; set; }
        private List<int[]> layer_frame_length_offset__to { get; set; }

        protected override ErrorInfo? _Redo() => _Execute();

        protected override ErrorInfo? _Undo()
        {
            try
            {
                List<TimelineItem> targets = new();

                if (Error != null)
                {
                    return Error;
                }

                for (int i = 0; i < N; i++)
                {
                    int[] to = layer_frame_length_offset__to[i];
                    (var item, var e) = TimelineDataController.Instance.FindItemByPosition(to[0], to[1], to[1] + to[2]);
                    if (item is null)
                    {
                        return e;
                    }

                    targets.Add(item);
                }




                for (int i = 0; i < N; i++)
                {
                    var item = targets[i];
                    int[] from = layer_frame_length_offset__from[i];
                    //item.ChangeData(from[1], from[2], from[0], from[3]);
                    item.Layer = from[0];
                    item.Frame = from[1];
                    item.Length = from[2];
                    item.Offset = from[3];
                }
                return null;
            }
            catch (Exception e)
            {
                return new ErrorInfo(ErrorCode.UnknownError, e);
            }
        }

        protected override ErrorInfo? _Execute()
        {
            try
            {
                List<TimelineItem> targets = new();

                for (int i = 0; i < N; i++)
                {
                    int[] from = layer_frame_length_offset__from[i];
                    (var item, var e) = TimelineDataController.Instance.FindItemByPosition(from[0], from[1], from[1] + from[2]);
                    if (item is null)
                    {
                        return e;
                    }
                    targets.Add(item);
                }

                int layerDelta = 0;

                for (int i = 0; i < TimelineDataController.Instance.MaxLayer.Value; i++)
                {
                    bool canMove = true;

                    foreach (var target in targets)
                    {
                        if (!TimelineDataController.Instance.CanUseFrameRange(target.Layer + i, target.Frame, target.R, new(targets)))
                        {
                            canMove = false;
                            break;
                        }
                    }
                    if (canMove)
                    {
                        layerDelta += i;
                        break;
                    }
                }

                for (int i = 0; i < N; i++)
                {
                    var item = targets[i];
                    int[] to = layer_frame_length_offset__to[i];
                    //item.ChangeData(to[1], to[2], to[0], to[3]);
                    item.Layer = to[0] + layerDelta;
                    item.Frame = to[1];
                    item.Length = to[2];
                    item.Offset = to[3];
                }

                return null;
            }
            catch (Exception e)
            {
                return new ErrorInfo(ErrorCode.UnknownError, e);
            }
        }
    }

}
