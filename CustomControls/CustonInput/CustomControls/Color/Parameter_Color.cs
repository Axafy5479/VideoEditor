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
using System.Windows.Media;
using System.Reactive.Disposables;
using Reactive.Bindings.TinyLinq;
using Data;
using ParameterEditor.ColorpickerWindow;

namespace ParameterEditor.CustomControls.Color
{
    public class Parameter_Color : Grid, IDisposable
    {
        static Parameter_Color()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Parameter_Color), new FrameworkPropertyMetadata(typeof(Parameter_Color)));
        }


        public void Dispose()
        {
            NumberBox.Dispose();
        }

        private ColorCodeBox NumberBox { get; }
        private NumberBox AlphaBox { get; }
        public ReactiveProperty<SolidColorBrush> SelectedBrush { get; private set; }

        public Parameter_Color(List<ReactiveProperty<SolidColorBrush>> properties, Func<SolidColorBrush, CommandBase>? commandOnDecided, string title)
        {
            NumberBox = new(properties, commandOnDecided);

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
            Grid rect = new Grid() { Width = 20, Height = 10, Background = Brushes.White, HorizontalAlignment = HorizontalAlignment.Left };
            var cd3 = new ColumnDefinition();
            cd3.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd3);
            SetColumn(rect, 1);
            this.Children.Add(rect);
            SelectedBrush = NumberBox.ViewModel.Model.Text.Select(t =>
            {
                return t.ToBrushOrNull() is SolidColorBrush c ? c : Brushes.Transparent;
            }).ToReactiveProperty();
            var bindingSelectingColor = new Binding()
            {
                Path = new PropertyPath("SelectedBrush.Value"),
                Source = this,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            rect.SetBinding(Control.BackgroundProperty, bindingSelectingColor);

            rect.MouseLeftButtonDown += (o, e) =>
            {
                var win = new ColorPickerWin();
                win.Show();
                win.ViewModel.SetProperty(properties);

                win.Closing += (o, e) =>
                {
                    NumberBox.ViewModel.Model.DecidedFromBrush();
                };
            };







            //NumberBoxの作成

            var cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Auto);
            this.ColumnDefinitions.Add(cd2);
            SetColumn(NumberBox, 2);
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
