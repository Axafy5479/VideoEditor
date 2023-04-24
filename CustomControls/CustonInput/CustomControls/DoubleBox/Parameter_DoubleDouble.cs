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
    public class Parameter_DoubleDouble : Grid, IDisposable
    {
        static Parameter_DoubleDouble()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Parameter_DoubleDouble), new FrameworkPropertyMetadata(typeof(Parameter_DoubleDouble)));
        }

        public void Dispose()
        {
            NumberBox1.Dispose();
        }

        private DoubleBox NumberBox1 { get; }
        public DoubleBox NumberBox2 { get; }

        public Parameter_DoubleDouble(List<ReactiveProperty<double>> properties1, List<ReactiveProperty<double>> properties2, double min, double max, Func<double, CommandBase> commandOnDecided, string mainTitle, string subTitle1, string subTitle2, double numberPerPixel)
        {
            // メインタイトルを作成
            var cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd1);
            TextBlock mainTitleBlock = new TextBlock() { Text = mainTitle, Margin = new Thickness(0, 0, 40, 0) };
            mainTitleBlock.VerticalAlignment = VerticalAlignment.Center;


            var binding = new Binding()
            {
                Path = new PropertyPath("Foreground"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mainTitleBlock.SetBinding(Control.ForegroundProperty, binding);

            SetColumn(mainTitleBlock, 0);
            this.Children.Add(mainTitleBlock);





            // サブタイトル1を作成
            var cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd2);
            TextBlock subTitleBlock1 = new TextBlock() { Text = subTitle1, Margin = new Thickness(0, 0, 20, 0) };
            subTitleBlock1.VerticalAlignment = VerticalAlignment.Center;



            var binding2 = new Binding()
            {
                Path = new PropertyPath("Foreground"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            subTitleBlock1.SetBinding(Control.ForegroundProperty, binding2);

            SetColumn(subTitleBlock1, 1);
            this.Children.Add(subTitleBlock1);










            //NumberBox1の作成
            NumberBox1 = new(properties1, min, max, commandOnDecided, numberPerPixel) { Margin = new Thickness(0, 0, 40, 0) };
            var cd3 = new ColumnDefinition();
            cd3.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd3);
            SetColumn(NumberBox1, 2);
            this.Children.Add(NumberBox1);


            var bindingNumberBox1 = new Binding()
            {
                Path = new PropertyPath("Foreground"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            NumberBox1.SetBinding(Control.ForegroundProperty, bindingNumberBox1);

            var bindingNumberBoxBG1 = new Binding()
            {
                Path = new PropertyPath("Background"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            NumberBox1.SetBinding(Control.BackgroundProperty, bindingNumberBoxBG1);





            // サブタイトル2

            // サブタイトル2を作成
            var cd4 = new ColumnDefinition();
            cd4.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd4);
            TextBlock subTitleBlock2 = new TextBlock() { Text = subTitle2, Margin = new Thickness(0, 0, 20, 0) };
            subTitleBlock2.VerticalAlignment = VerticalAlignment.Center;



            var binding4 = new Binding()
            {
                Path = new PropertyPath("Foreground"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            subTitleBlock2.SetBinding(Control.ForegroundProperty, binding4);

            SetColumn(subTitleBlock2, 3);
            this.Children.Add(subTitleBlock2);





            // number box2
            NumberBox2 = new(properties2, min, max, commandOnDecided, numberPerPixel);
            var cd5 = new ColumnDefinition();
            cd5.Width = new GridLength(1, GridUnitType.Star);
            this.ColumnDefinitions.Add(cd5);
            SetColumn(NumberBox2, 4);
            this.Children.Add(NumberBox2);
            var bindingNumberBox2 = new Binding()
            {
                Path = new PropertyPath("Foreground"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            NumberBox2.SetBinding(Control.ForegroundProperty, bindingNumberBox2);

            var bindingNumberBoxBG2 = new Binding()
            {
                Path = new PropertyPath("Background"),
                Source = Data.ColorManager.Instance,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            NumberBox2.SetBinding(Control.BackgroundProperty, bindingNumberBoxBG2);
        }
    }
}
