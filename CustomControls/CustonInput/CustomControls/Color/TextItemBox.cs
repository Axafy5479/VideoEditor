using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ParameterEditor.CustomControls
{
    public class ColorCodeBox : TextBox, IDisposable
    {

        public ColorCodeBox(List<ReactiveProperty<SolidColorBrush>> properties, Func<SolidColorBrush, CommandBase>? commandOnDecided)
        {
            Properties = properties;



            ViewModel = new ColorCodeBoxViewModel(properties, commandOnDecided);

            var binding = new Binding() { Path = new PropertyPath("ShowingText.Value"),
                Source = ViewModel, Mode = BindingMode.TwoWay, 
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

        public List<ReactiveProperty<SolidColorBrush>> Properties { get; }
        public ColorCodeBoxViewModel ViewModel { get; }
        private CompositeDisposable disposables{get;} = new();


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
