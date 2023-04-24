using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ParameterEditor.CustomControls
{
    public class DoubleBox : TextBox, IDisposable
    {

        public DoubleBox(List<ReactiveProperty<double>> properties, double min, double max, Func<double, CommandBase>? commandOnDecided, double numPerPixel)
        {
            Properties = properties;



            ViewModel = new DoubleBoxViewModel(properties, min, max, commandOnDecided, numPerPixel);
            ViewModel.State.Subscribe(state => {
                switch (state)
                {
                    case EditNumBoxState.None:
                        Application.Current.MainWindow.Focus();
                        break;
                    case EditNumBoxState.Typping:
                        this.Focus();

                        break;
                    case EditNumBoxState.Drag:
                        Application.Current.MainWindow.Focus();
                        break;
                    default:
                        break;
                }
            }).AddTo(disposables);

            var binding = new Binding()
            {
                Path = new PropertyPath("ShowingText.Value"),
                Source = ViewModel,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };


            this.InputBindings.Add(new KeyBinding(
                new DelegateCommand(() => CheckEnterDown(null, null)), Key.Enter, ModifierKeys.None));

            this.SetBinding(TextProperty, binding);
            Width = 60;
            Height = 15;
            TextAlignment = TextAlignment.Center;
            this.HorizontalAlignment = HorizontalAlignment.Left;

            this.Template = Data.Utility.GetRoundedTextBoxTemplate(4);
        }

        public List<ReactiveProperty<double>> Properties { get; }
        public DoubleBoxViewModel ViewModel { get; }
        private CompositeDisposable disposables { get; } = new();


        private void CheckEnterDown(object s, KeyEventArgs e)
        {

            // Move to a parent that can take focus
            FrameworkElement parent = (FrameworkElement)this.Parent;
            while (parent != null && parent is IInputElement && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(this);
            FocusManager.SetFocusedElement(scope, parent as IInputElement);
            Keyboard.Focus(Application.Current.MainWindow);
        }


        private double init_x = 0;
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            init_x = e.GetPosition(this).X;
            ViewModel.OnPreviewMouseLeftButtonDown(init_x);





        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            double x = e.GetPosition(this).X;
            ViewModel.OnPreviewMouseLeftButtonUp(x);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            base.OnPreviewMouseMove(e);
            double x = e.GetPosition(this).X;
            ViewModel.OnPreviewMouseMove(x);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            ViewModel.OnLostKeyboardFocus();
        }

        public void Dispose()
        {
            ViewModel.Dispose();
            disposables.Dispose();
        }
    }
}
