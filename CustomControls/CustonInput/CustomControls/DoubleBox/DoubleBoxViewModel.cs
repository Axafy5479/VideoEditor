using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ParameterEditor.CustomControls
{
    public class DoubleBoxViewModel
    {
        private const int MoveTreshold = 2;
        private CompositeDisposable disposables = new();

        public DoubleBoxViewModel(List<ReactiveProperty<double>> parameters, double min, double max, Func<double, CommandBase>? commandOnDecided,double numPerPixel)
        {
            Min = min;
            Max = max;
            NumPerPixel = numPerPixel;

            Model = new DoubleBoxModel(parameters, commandOnDecided);
            Model.Number.Subscribe(value =>
            {
                if (value is double n)
                {
                    ShowingText.Value = n.ToString("0.00");
                }
                else
                {
                    ShowingText.Value = "-";
                }
            }).AddTo(disposables);

            ShowingText.Subscribe(text =>
            {
                bool canParse = double.TryParse(text, out var d);
                if (canParse && d != Model.Number.Value)
                {
                    Model.Number.Value = d;
                }
            }).AddTo(disposables);
        }
        public ReactiveProperty<string> ShowingText { get; } = new();
        public DoubleBoxModel Model { get; }

        public double NumPerPixel { get; } = 0.1;
        public double Max { get; } = 100;
        public double Min { get; } = 0;

        internal ReactiveProperty<EditNumBoxState> State { get; set; } = new(EditNumBoxState.None);
        private double init_x = 0;
        private bool mouseDowned = false;
        private double initialValue;
        internal void OnPreviewMouseLeftButtonDown(double init_x)
        {
            if (Model.Number.Value is not double n) return;

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

            if (Math.Abs(x - init_x) < MoveTreshold)
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

            double diff = x - init_x;

            if (Math.Abs(diff) > MoveTreshold && State.Value != EditNumBoxState.Drag)
            {
                State.Value = EditNumBoxState.Drag;
                init_x = x;
            }

            if (State.Value == EditNumBoxState.Drag)
            {
                double temp = initialValue + (double)(diff * NumPerPixel);
                temp = Math.Min(temp, Max);
                temp = Math.Max(temp, Min);
                temp = Math.Round(temp, 2);
                Model.Number.Value = temp;
            }
        }

        internal void OnLostKeyboardFocus()
        {

            if (double.TryParse(ShowingText.Value, out double temp))
            {
                temp = Math.Min(temp, Max);
                temp = Math.Max(temp, Min);
                Model.Number.Value = temp;
                Model.Decided();
            }
            else
            {
                ShowingText.Value = Model.Number.Value is double m ? m.ToString() : "-";
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
