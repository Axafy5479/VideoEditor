using CommandProject.Commands;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Timeline.Error;

namespace CommandProject.Commands
{
    public class Command_Paste : RedoableCommandBase
    {
        public List<byte[]> TargetsByteData { get; set; }
        public int CursorFrame { get; set; }

        protected override void BeforeExecute()
        {
            base.BeforeExecute();
            CursorFrame = Pointer.Instance.CursorFrame.Value;
            TargetsByteData = ReSerializeAllItem(Pointer.Instance.CopiedItemData);
        }

        public override bool CanExecute(object? parameter)
        {
            return Pointer.Instance.CopiedItemData.Any();
        }


        protected List<byte[]>? ReSerializeAllItem(List<byte[]> targetsByte)
        {
            var result = new List<byte[]>();


            // 選択中のVMをレイヤーの若い順にソート
            var items = new List<TimelineItem>();
            foreach (var itemByte in targetsByte)
            {
                (var item, Error) = TimelineItem.Deserialize(itemByte);
                if (item == null) return null;
                items.Add(item);
            }

            int minFrame = items.Min(x => x.Frame);
            int frameDelta = CursorFrame - minFrame;
            items.ForEach(x => x.Frame += frameDelta);

            items.Sort((a, b) => a.Layer - b.Layer);

            // 配置ターゲット (最もレイヤー番号の若いアイテムがselectedLayerに配置される)
            int selectedLayer = Pointer.Instance.SelectedBlank.Value.layer != -1
                ? Pointer.Instance.SelectedBlank.Value.layer
                : 0;

            // ターゲットのレイヤーまでの移動量
            int layerDelta = selectedLayer - items[0].Layer;

            // 他のアイテムと干渉しない場所を見つける
            for (int delta = layerDelta; delta < TimelineDataController.Instance.MaxLayer.Value; delta++)
            {
                bool canPlace = true;

                foreach (var item in items)
                {
                    int newLayer = item.Layer + delta;
                    if (newLayer >= 100)
                    {
                        return null;
                    }
                    canPlace = TimelineDataController.Instance.CanUseFrameRange(newLayer, item.Frame, item.R, null);
                    if (!canPlace)
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    layerDelta = delta;
                    break;
                }
            }

            items.ForEach(d => d.Layer += layerDelta);

            foreach (var item in items)
            {
                (var byteItem, var e) = item.Serialize();
                if (byteItem is null)
                {
                    Error = e;
                    return null;
                }
                else
                {
                    result.Add(byteItem);
                }
            }

            return result;
        }

        protected override ErrorInfo? _Redo() => _Execute();

        protected override ErrorInfo? _Undo()
        {
            try
            {
                Mouse.Captured?.ReleaseMouseCapture();
                var N = TargetsByteData.Count;

                for (int i = 0; i < N; i++)
                {
                    (var tlItem, var e) = TimelineItem.Deserialize(TargetsByteData[i]);

                    if (tlItem == null)
                    {
                        return e;
                    }

                    if (TimelineDataController.Instance.RemoveItem(tlItem.Layer, tlItem.Frame, tlItem.Length) is ErrorInfo e2)
                    {
                        return e2;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return new(ErrorCode.UnknownError, e);
            }
        }

        protected override ErrorInfo? _Execute()
        {
            try
            {
                foreach (var item in TargetsByteData)
                {
                    (var newItem, var e) = TimelineItem.Deserialize(item);
                    if (newItem == null) return e;
                    TimelineDataController.Instance.AddItem(newItem);
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
