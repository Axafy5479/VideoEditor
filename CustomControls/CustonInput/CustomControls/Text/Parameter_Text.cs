using CommandProject.Commands;
using ParameterEditor.CustomControls;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ParameterEditor
{
 
    public class Parameter_Text : Grid, IDisposable
    {
        static Parameter_Text()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Parameter_Text), new FrameworkPropertyMetadata(typeof(Parameter_Text)));
        }

        public void Dispose()
        {
            NumberBox.Dispose();
        }

        private TextItemBox NumberBox { get; }

        public Parameter_Text(List<ReactiveProperty<string>> properties, Func<string, CommandBase>? commandOnDecided, string title)
        {
            // タイトルを作成
            var cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd1);
            TextBlock titleBlock = new TextBlock() {Text=title, Margin = new Thickness(0,0,40,0)};
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
            NumberBox = new(properties, commandOnDecided);
            NumberBox.Width = 100;
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

