using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandProject.Commands;
using System.Threading.Tasks;
using System.Windows.Controls;
using Timeline.Error;

namespace CommandProject.Commands
{
    public class Command_Divide : RedoableCommandBase<ITimelineObjectViewModel>
    {
        protected override void BeforeExecute()
        {
            base.BeforeExecute();

            DividingFrame = Pointer.Instance.CursorFrame.Value;
            ItemByteData_After = new();
            CreateAfterDevidedItemByteData();
        }

        private void CreateAfterDevidedItemByteData()
        {
            foreach (var vm in Pointer.Instance.SelectedItem.Value)
            {
                var item = vm.ItemData;
                int prev_l = item.Frame;
                int prev_r = item.R;
                int prev_Offset = item.Offset;

                // アイテムの右端を分割位置にし、シリアル化
                item.Length = DividingFrame - prev_l;
                (var byteData_l, Error) = item.Serialize();
                if (byteData_l == null) return;

                // アイテムの左端を分割位置にし、シリアル化
                item.Length = prev_r - DividingFrame;
                item.Frame = DividingFrame;
                item.Offset += DividingFrame - prev_l;
                (var byteData_r, Error) = item.Serialize();
                if (byteData_r == null) return;

                // 変更前に戻す
                item.Frame = prev_l;
                item.Length = prev_r - prev_l;
                item.Offset = prev_Offset;
                ItemByteData_After.Add((byteData_l, byteData_r));
            }

        }


        public int N { get; private set; }
        public List<(byte[] l, byte[] r)> ItemByteData_After { get; set; }

        public int DividingFrame { get; private set; }

        protected override ErrorInfo? _Execute()
        {
            #region Delete target items
            foreach (var byteItem in TargetsByteData)
            {
                (var item, Error) = TimelineItem.Deserialize(byteItem);
                if(item == null) return Error;

                if (TimelineDataController.Instance.RemoveItem(item.Layer, item.Frame, item.Length) is ErrorInfo e2)
                {
                    return Error;
                }
            }
            #endregion

            #region Create divided Items
            foreach ((var itemByte_l,var itemByte_r) in ItemByteData_After)
            {
                (var item_l, var Error) = TimelineItem.Deserialize(itemByte_l);
                if(item_l == null) return Error;

                TimelineDataController.Instance.AddItem(item_l);

                (var item_r, Error) = TimelineItem.Deserialize(itemByte_r);
                if (item_r == null) return Error;

                TimelineDataController.Instance.AddItem(item_r);
            }
            #endregion

            return null;
        }

        protected override ErrorInfo? _Redo()
        {
            #region Delete target item
            foreach (var byteData in TargetsByteData)
            {
                (var item, Error) = TimelineItem.Deserialize(byteData);
                if (item is null) return Error;

                if (TimelineDataController.Instance.RemoveItem(item.Layer, item.Frame, item.Length) is ErrorInfo e2)
                {
                    return e2;
                }

            }
            #endregion

            #region Create divided Items
            foreach ((var itemByte_l, var itemByte_r) in ItemByteData_After)
            {
                (var item_l, Error) = TimelineItem.Deserialize(itemByte_l);
                if (item_l == null) return Error;

                TimelineDataController.Instance.AddItem(item_l);


                (var item_r, Error) = TimelineItem.Deserialize(itemByte_r);
                if (item_r == null) return Error;

                TimelineDataController.Instance.AddItem(item_r);
            }
            #endregion

            return null;
        }

        protected override ErrorInfo? _Undo()
        {
            #region Delete divided items
            foreach ((var byte_l, var byte_r) in ItemByteData_After)
            {
                (var item_l, Error) = TimelineItem.Deserialize(byte_l);
                if (item_l is null) return Error;

                (var item_r, Error) = TimelineItem.Deserialize(byte_r);
                if (item_r is null) return Error;

                if (TimelineDataController.Instance.RemoveItem(item_l.Layer, item_l.Frame, item_l.Length) is ErrorInfo e2)
                {
                    return e2;
                }
                if (TimelineDataController.Instance.RemoveItem(item_r.Layer, item_r.Frame, item_r.Length) is ErrorInfo e3)
                {
                    return e3;
                }
            }
            #endregion

            #region Create item before deviding
            foreach (var byteData in TargetsByteData)
            {
                (var item, Error) = TimelineItem.Deserialize(byteData);
                if (item is null) return Error;

                TimelineDataController.Instance.AddItem(item);

            }
            #endregion

            return null;
        }


    }
}
