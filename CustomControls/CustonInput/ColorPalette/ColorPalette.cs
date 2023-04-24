using CommunityToolkit.Mvvm.Input;
using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TriangleColorPicker;

namespace ParameterEditor
{

    public class ColorPalette : Control, IDisposable
    {
        static ColorPalette()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPalette), new FrameworkPropertyMetadata(typeof(ColorPalette)));
        }

        public ColorPalette()
        {
            DecideButton = new RelayCommand(OnDecidedButtonClicked);
            CancelButton = new RelayCommand(OnCancelButtonClicked);
        }

        public Subject<Unit> OnEndEditing { get; } = new Subject<Unit>();
        public ColorPicker? Picker { get; private set; }
        public void SetProperty(List<ReactiveProperty<SolidColorBrush>> property)
        {
            OldColor.Value = property[0].Value;

            if (IsLoaded)
            {
                Picker = (ColorPicker)Template.FindName("ColorPicker", this);
                Picker.SetAndShowPicker(property);

                Picker.ViewModel.Model.Property.Subscribe(c =>
                {
                    CurrentColor.Value = c;
                });
                AddRGBProperty(Picker.ViewModel.Model.Property);
            Picker.ViewModel.Model.OnEndEditing.Subscribe(OnEndEditing.OnNext);
            }
            else
            {
                Loaded += (s, e) =>
                {
                    Picker = (ColorPicker)Template.FindName("ColorPicker", this);
                    Picker.SetAndShowPicker(property);

                    Picker.ViewModel.Model.Property.Subscribe(c =>
                    {
                        CurrentColor.Value = c;
                    });
                AddRGBProperty(Picker.ViewModel.Model.Property);
            Picker.ViewModel.Model.OnEndEditing.Subscribe(OnEndEditing.OnNext);
                };
            }



        }

        private void AddRGBProperty(ReactiveProperty<SolidColorBrush> property)
        {
            var wrapPanel = (WrapPanel)this.Template.FindName("RGBPropertyGrid",this);

            var r = new ReactiveProperty<int>();
            var g = new ReactiveProperty<int>();
            var b = new ReactiveProperty<int>();
            var colorCode = new ReactiveProperty<string>();

            property
                //.Where(v=> v.Color.ToColorCode() != colorCode.Value)
                .Subscribe(brush => { 
                    r.Value = brush.Color.R; g.Value = brush.Color.G; b.Value = brush.Color.B; 
                    colorCode.Value = Data.Utility.ToColorCode(brush.Color);
                });


            r.Where(v=>v != property.Value.Color.R).Subscribe(v => Picker?.ViewModel.Model.Change((byte)r.Value, (byte)g.Value, (byte)b.Value));
            g.Where(v => v != property.Value.Color.G).Subscribe(v => Picker?.ViewModel.Model.Change((byte)r.Value, (byte)g.Value, (byte)b.Value));
            b.Where(v => v != property.Value.Color.B).Subscribe(v => Picker?.ViewModel.Model.Change((byte)r.Value, (byte)g.Value, (byte)b.Value));
            colorCode.Where(v => {
                var c1 = Data.Utility.ToColorOrNull(v);
                if (c1 is not Color c) return false;
                var c2 = property.Value.Color;
                return c.R != c2.R || c.G !=c2.G || c.B !=c2.B;
            })
            .Select(v=>Data.Utility.ToColorOrNull(v)).Where(c=>c is not null).Subscribe(c =>
            { 
                Picker?.ViewModel.Model.Change((Color)c);
            });


            wrapPanel.Children.Add(new Parameter_Int(new() { r }, 0, 255, null, "R"));
            wrapPanel.Children.Add(new Parameter_Int(new() { g }, 0, 255, null, "G"));
            wrapPanel.Children.Add(new Parameter_Int(new() { b }, 0, 255, null, "B"));
            wrapPanel.Children.Add(new Parameter_Text(new() { colorCode }, null, "Code"));
        }




        public ICommand DecideButton { get; }
        public ICommand CancelButton { get; }
        public void OnDecidedButtonClicked()
        {
            if(Picker == null)
            {
            Debug.WriteLine("Pickerのインスタンスが存在しません");
                OnEndEditing.OnNext(Unit.Default);
                return;
            }
            Picker.ViewModel.Model.Decided();
        }

        public void OnCancelButtonClicked()
        {
            if(Picker == null)
            {
            Debug.WriteLine("Pickerのインスタンスが存在しません");
                OnEndEditing.OnNext(Unit.Default);
                return;
            }
            Picker.ViewModel.Model.Canceled();
        }



        public void Dispose()
        {
            Picker?.Dispose();
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
    "Color", // プロパティ名を指定
    typeof(ReactiveProperty<SolidColorBrush>), // プロパティの型を指定
    typeof(ColorPalette), // プロパティを所有する型を指定
    new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる

        public ReactiveProperty<SolidColorBrush> SolidColor
        {
            get => (ReactiveProperty<SolidColorBrush>)(this.GetValue(ColorProperty));
            set => this.SetValue(ColorProperty, value);
        }

        public ReactiveProperty<Brush> CurrentColor { get; } = new(Brushes.Black);
        public ReactiveProperty<Brush> OldColor { get; } = new(Brushes.Black);
    }
}
