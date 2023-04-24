using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommandProject.Commands;
using Timeline.Error;

namespace CommandProject.Commands
{
    /// <summary>
    /// タイムラインにアイテムを追加する
    /// </summary>
    public class Command_AddItem : RedoableCommandBase
    {
        public List<byte[]>? TargetsByteData { get; }

        public Command_AddItem(List<TimelineItem> newItem)
        {
            TargetsByteData = SerializeAllItem(newItem);
        }

        public override bool CanExecute(object? parameter) => true;

        public List<byte[]>? SerializeAllItem(List<TimelineItem> targets)
        {
            var result = new List<byte[]>();


            // 選択中のVMをレイヤーの若い順にソート
            targets.Sort((a, b) => a.Layer - b.Layer);

            // 配置ターゲット (最もレイヤー番号の若いアイテムがselectedLayerに配置される)
            int selectedLayer = Pointer.Instance.SelectedBlank.Value.layer != -1
                ? Pointer.Instance.SelectedBlank.Value.layer
                : 0;

            // ターゲットのレイヤーまでの移動量
            int layerDelta = selectedLayer - targets[0].Layer;

            // 他のアイテムと干渉しない場所を見つける
            for (int delta = layerDelta; delta < TimelineDataController.Instance.MaxLayer.Value; delta++)
            {
                bool canPlace = true;

                foreach (var item in targets)
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

            targets.ForEach(d => d.Layer += layerDelta);

            foreach (var item in targets)
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
