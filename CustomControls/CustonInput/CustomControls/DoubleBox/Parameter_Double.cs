using CommandProject.Commands;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace ParameterEditor.CustomControls
{
    public class Parameter_Double : Grid, IDisposable
    {
        static Parameter_Double()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Parameter_Double), new FrameworkPropertyMetadata(typeof(Parameter_Double)));
        }

        public void Dispose()
        {
            NumberBox.Dispose();
        }

        private DoubleBox NumberBox { get; }

        public Parameter_Double(List<ReactiveProperty<double>> properties, double min, double max, Func<double, CommandBase>? commandOnDecided, string title, double numPerPixel)
        {
            // タイトルを作成
            var cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd1);
            TextBlock titleBlock = new TextBlock() { Text = title, Margin = new Thickness(0, 0, 40, 0) };
            titleBlock.Text = title;
            titleBlock.VerticalAlignment = VerticalAlignment.Center;


            var binding = new Binding()
            {
                Path = new PropertyPath("Foreground"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            this.SetBinding(Control.ForegroundProperty, binding);

            SetColumn(titleBlock, 0);
            this.Children.Add(titleBlock);

            //NumberBoxの作成
            NumberBox = new(properties, min, max, commandOnDecided, numPerPixel);
            var cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(cd2);
            SetColumn(NumberBox, 1);
            this.Children.Add(NumberBox);


            var bindingNumberBox = new Binding()
            {
                Path = new PropertyPath("Foreground"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            NumberBox.SetBinding(Control.ForegroundProperty, bindingNumberBox);

            var bindingNumberBoxBG = new Binding()
            {
                Path = new PropertyPath("Background"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            NumberBox.SetBinding(Control.BackgroundProperty, bindingNumberBoxBG);
        }
    }
}
