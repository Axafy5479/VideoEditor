using CommunityToolkit.Mvvm.Input;
using Data;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Timeline.Error;

namespace CommandProject.Commands
{
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// このコマンドが実行可能であるかを返すデリゲート
        /// </summary>
        public virtual bool CanExecute(object? parameter)
        {
            return Pointer.Instance.SelectedVMs.Any();
        }

        public event EventHandler? CanExecuteChanged;

        public ErrorInfo? Error { get; protected set; }

        /// <summary>
        /// このコマンドが実行可能である場合、実行する
        /// </summary>
        /// <returns></returns>
        public ErrorInfo? Execute()
        {
            if (CanExecute(null) && Error is null)
            {
                BeforeExecute();
                return _Execute();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// コンテキストメニューから実行された場合に呼ばれる
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            CommandInvoker.Instance.Execute(this);
        }

        /// <summary>
        /// コマンドの中身
        /// </summary>
        /// <returns></returns>
        protected abstract ErrorInfo? _Execute();

        public RelayCommand CreateContextmenuCommand()
        {
            return new RelayCommand(
                () => CommandInvoker.Instance.Execute((CommandBase)Activator.CreateInstance(GetType())),
                () => CanExecute(null));
        }

        protected virtual void BeforeExecute() { }
    }


    public abstract class CommandBase<T> : CommandBase where T : IItemObjectViewModel
    {
        protected override void BeforeExecute()
        {
            var targets = Pointer.Instance.SelectedVMs.ConvertAll(vm => (T)vm);
            TargetsByteData = SerializeAllItem(targets) ?? new();
        }

        protected virtual List<byte[]>? SerializeAllItem(List<T> targets)
        {
            var result = new List<byte[]>();    
            foreach (var t in targets)
            {
                (var byteData, var e) = t.ItemData.Serialize();
                if (byteData is null)
                {
                    Error = e;
                    return null;
                }

                result.Add(byteData);
            }
            return result;
        }

        protected List<byte[]> TargetsByteData { get; set; } = new();

        public override bool CanExecute(object? parameter)
        {
            var condition1 = Pointer.Instance.SelectedItem.Value!=null;
            if (!condition1) return false;
            var condition2 = Pointer.Instance.SelectedItem.Value.Any();
            var condition3 = Pointer.Instance.SelectedItem.Value.All(vm => vm is T);
            return condition2 && condition3;
        }

    }
}
