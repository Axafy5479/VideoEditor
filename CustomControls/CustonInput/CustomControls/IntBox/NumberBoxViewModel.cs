using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ParameterEditor
{
    public class NumberBoxViewModel : IDisposable
    {
        private const int MoveTreshold = 2;
        private CompositeDisposable disposables = new();

        public NumberBoxViewModel(List<ReactiveProperty<int>> parameters, int min, int max, Func<int,CommandBase>? commandOnDecided)
        {
            Min = min;
            Max = max;

            Model = new NumberBoxModel(parameters, commandOnDecided);
            Model.Number.Subscribe(value =>
            {
                if(value is int n)
                {
                    ShowingText.Value = n.ToString();
                }
                else
                {
                    ShowingText.Value = "-";
                }
            }).AddTo(disposables);

            ShowingText.Subscribe(text =>
            {
                bool canParse = int.TryParse(text, out var n);
                if(canParse && n != Model.Number.Value)
                {
                    Model.Number.Value = n;
                }
            }).AddTo(disposables);
        }
        public ReactiveProperty<string> ShowingText { get; } = new();
        public NumberBoxModel Model { get; }

        public double NumPerPixel { get; } = 1;
        public int Max { get; } = int.MaxValue;
        public int Min { get; } = 0;

        internal ReactiveProperty<EditNumBoxState> State { get; set; } = new(EditNumBoxState.None);
        private double init_x = 0;
        private bool mouseDowned = false;
        private int initialValue;
        internal void OnPreviewMouseLeftButtonDown(double init_x)
        {
            if (Model.Number.Value is not int n) return;

            initialValue = n;
            this.init_x = init_x;
            mouseDowned = true;

            // この段階で、ドラッグかタイピングのどちらかは決定しないため
            Keyboard.ClearFocus();
        }


        internal void OnPreviewMouseLeftButtonUp(double x)
        {
            if (!mouseDowned) return;
            mouseDowned = false;

            if (Math.Abs(x-init_x)<MoveTreshold)
            {
                // カーソルを閾値以上動かさなかった場合タイピング開始
               State.Value = EditNumBoxState.Typping;
            }
            else
            {
                // カーソルを閾値以上動かした場合ドラッグ終了
                State.Value = EditNumBoxState.None;
                Model.Decided();
            }

        }

        internal void OnPreviewMouseMove(double x)
        {
            if (!mouseDowned || State.Value == EditNumBoxState.Typping) return;

            double diff = x- init_x;

            if (Math.Abs(diff) > MoveTreshold && State.Value!= EditNumBoxState.Drag)
            {
                State.Value = EditNumBoxState.Drag;
                init_x = x;
            }

            if(State.Value == EditNumBoxState.Drag)
            {
                int temp = initialValue + (int)(diff * NumPerPixel);
                temp = Math.Min(temp, Max);
                temp = Math.Max(temp, Min);
                Model.Number.Value = temp;
            }
        }

        internal void OnLostKeyboardFocus()
        {

            if (int.TryParse(ShowingText.Value, out int temp))
            {
                temp = Math.Min(temp, Max);
                temp = Math.Max(temp, Min);
                Model.Number.Value = temp;
                Model.Decided();
            }
            else
            {
                ShowingText.Value = Model.Number.Value is int m ? m.ToString() : "-";
            }


            // カーソルを閾値以上動かした場合ドラッグ終了
            State.Value = EditNumBoxState.None;
        }

        public void Dispose()
        {
            Model.Dispose();
            disposables.Dispose();
        }


    }
}
