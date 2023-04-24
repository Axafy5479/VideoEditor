using CommandProject.Commands;
using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Timeline.Error;

namespace CommandProject.ScreenCommands
{
    public class Command_Zoom : RedoableCommandBase<IRenderingScreenObjVM>
    {

        protected override List<byte[]>? SerializeAllItem(List<IRenderingScreenObjVM> targets)
        {
            var result = base.SerializeAllItem(targets);

            TargetByteData = new();
            ZoomFrom = new();
            foreach (var item in targets)
            {
                ZoomFrom.Add(item.RenderingItem.Zoom);
                (var data, Error) = item.RenderingItem.Serialize();
                if (data is null) return null;

                TargetByteData.Add(data);
            }
            ZoomTo = targets[0].Zoom.Value;

            return result;
        }

        public double ZoomTo { get; set; }
        public List<double> ZoomFrom { get; set; }
        private List<byte[]> TargetByteData { get; set; }

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

            foreach (var byteItem in TargetByteData)
            {
                (var _item, var e) = TimelineItem.Deserialize(byteItem);
                if (_item is null) return e;

                (var item, var e2) = TimelineDataController.Instance.FindItemByPosition(_item.Layer, _item.Frame, _item.R);

                if(item is null) return e2; 

                var renderItem = (IRenderingItem)item;
                renderItem.Zoom = ZoomTo;
            }

            return null;
        }
    }

    public abstract class Command_ParameterChangeBase<T_ItemVMType, T_Param> : RedoableCommandBase<T_ItemVMType> where T_ItemVMType : IItemObjectViewModel
    {
        protected abstract T_Param GetParameterFromItemData(T_ItemVMType itemViewModel);
        protected abstract IReadOnlyReactiveProperty<T_Param> GetParameterFromViewModel(T_ItemVMType itemViewModel);
        protected abstract void SetParameterToItemData(TimelineItem itemData, T_Param value);

        protected override List<byte[]>? SerializeAllItem(List<T_ItemVMType> targets)
        {
            var result = base.SerializeAllItem(targets);

            TargetByteData = new();
            ParameterFrom = new();
            foreach (var item in targets)
            {
                // 古い情報は(ViewModelではなく)ItemDataから取得
                ParameterFrom.Add(GetParameterFromItemData(item));
                (var data, Error) = item.ItemData.Serialize();
                if (data is null) return null;

                TargetByteData.Add(data);
            }

            // 古い情報は(ItemDataではなく)ViewModelから取得
            ParameterTo = GetParameterFromViewModel(targets[0]).Value;

            return result;
        }

        public T_Param ParameterTo { get; set; }
        public List<T_Param> ParameterFrom { get; set; }
        private List<byte[]> TargetByteData { get; set; }

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

            foreach (var byteItem in TargetByteData)
            {
                (var _item, var e) = TimelineItem.Deserialize(byteItem);
                if (_item is null) return e;

                (var item, var e2) = TimelineDataController.Instance.FindItemByPosition(_item.Layer, _item.Frame, _item.R);

                if (item is null) return e2;

                SetParameterToItemData(item, ParameterTo);
            }

            return null;
        }
    }

    public class Command_ChangeVolume : Command_ParameterChangeBase<IAudioScreenObjectVM, double>
    {
        protected override double GetParameterFromItemData(IAudioScreenObjectVM itemViewModel)
        {
            return itemViewModel.AudioItem.Volume;
        }

        protected override IReadOnlyReactiveProperty<double> GetParameterFromViewModel(IAudioScreenObjectVM itemViewModel)
        {
            return itemViewModel.Volume;
        }

        protected override void SetParameterToItemData(TimelineItem itemData, double value)
        {
            ((IAudioItem)itemData).Volume = value;
        }
    }


    public class Command_ChangePlaybackRate : Command_ParameterChangeBase<IAudioScreenObjectVM, double>
    {
        protected override double GetParameterFromItemData(IAudioScreenObjectVM itemViewModel)
        {
            return itemViewModel.AudioItem.PlaybackRate;
        }

        protected override IReadOnlyReactiveProperty<double> GetParameterFromViewModel(IAudioScreenObjectVM itemViewModel)
        {
            return itemViewModel.PlaybackRate;
        }

        protected override void SetParameterToItemData(TimelineItem itemData, double value)
        {
            ((AudioItem)itemData).PlaybackRate = value;
        }
    }

    public class Command_ChangeFade : Command_ParameterChangeBase<IAudioScreenObjectVM, double[]>
    {
        protected override double[] GetParameterFromItemData(IAudioScreenObjectVM itemViewModel)
        {
            return new double[2] { itemViewModel.AudioItem.FadeIn, itemViewModel.AudioItem.FadeOut };
        }

        protected override IReadOnlyReactiveProperty<double[]> GetParameterFromViewModel(IAudioScreenObjectVM itemViewModel)
        {
            return new ReactiveProperty<double[]>(new double[2] { itemViewModel.FadeIn.Value, itemViewModel.FadeOut.Value });
        }

        protected override void SetParameterToItemData(TimelineItem itemData, double[] value)
        {
            ((IAudioItem)itemData).FadeIn = value[0];
            ((IAudioItem)itemData).FadeOut = value[1];
        }
    }

    public class Command_ChangeEchoInterval : Command_ParameterChangeBase<IAudioScreenObjectVM, double>
    {
        protected override double GetParameterFromItemData(IAudioScreenObjectVM itemViewModel)
        {
            return itemViewModel.AudioItem.EchoInterval;
        }

        protected override IReadOnlyReactiveProperty<double> GetParameterFromViewModel(IAudioScreenObjectVM itemViewModel)
        {
            return itemViewModel.EchoInterval;
        }

        protected override void SetParameterToItemData(TimelineItem itemData, double value)
        {
            ((AudioItem)itemData).EchoInterval = value;
        }
    }


    public class Command_ChangeText : Command_ParameterChangeBase<ITextScreenObjVM, string>
    {
        protected override string GetParameterFromItemData(ITextScreenObjVM itemViewModel)
        {
            return ((TextItem)itemViewModel.ItemData).Text;
        }

        protected override IReadOnlyReactiveProperty<string> GetParameterFromViewModel(ITextScreenObjVM itemViewModel)
        {
            return itemViewModel.Text;
        }

        protected override void SetParameterToItemData(TimelineItem itemData, string value)
        {
            ((TextItem)itemData).Text = value;
        }
    }


    public class Command_ChangeFontSize : Command_ParameterChangeBase<ITextScreenObjVM, double>
    {
        protected override double GetParameterFromItemData(ITextScreenObjVM itemViewModel)
        {
            return ((TextItem)itemViewModel.ItemData).FontSize;
        }

        protected override IReadOnlyReactiveProperty<double> GetParameterFromViewModel(ITextScreenObjVM itemViewModel)
        {
            return itemViewModel.FontSize;
        }

        protected override void SetParameterToItemData(TimelineItem itemData, double value)
        {
            ((TextItem)itemData).FontSize = value;
        }
    }



    public class Command_ChangeColor : Command_ParameterChangeBase<ITextScreenObjVM, SolidColorBrush>
    {
        protected override SolidColorBrush GetParameterFromItemData(ITextScreenObjVM itemViewModel)
        {
            return ((TextItem)itemViewModel.ItemData).Color.ToBrushOrNull() ?? Brushes.Pink;
        }

        protected override IReadOnlyReactiveProperty<SolidColorBrush> GetParameterFromViewModel(ITextScreenObjVM itemViewModel)
        {
            return itemViewModel.Color;
        }

        protected override void SetParameterToItemData(TimelineItem itemData, SolidColorBrush value)
        {
            ((TextItem)itemData).Color = value.Color.ToColorCode();
        }
    }

}
