using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.RegularExpressions;
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
    internal enum State
    {
        None,
        MouseOver,
        Editing
    }

    public class NumberOnlyTextBox : Control
    {
        static NumberOnlyTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberOnlyTextBox), new FrameworkPropertyMetadata(typeof(NumberOnlyTextBox)));
        }




        public NumberOnlyTextBox()
        {
            Loaded += Initialize;
        }

        private bool loaded = false;
        private void Initialize(object sender, RoutedEventArgs e)
        {
            if(loaded)
            {
                return;
            }

            Diff_par_movePixel.Value = OnlyInt ? 1 : 0.01;
            MainTextBox = this.GetChildOfType<TextBox>();
            loaded = true;
            Value = InitialValue;
            ResetValue = InitialValue;
            MainTextBox.LostKeyboardFocus += (s, e) => OnLostFocus();
            this.GotKeyboardFocus += (s, e) => OnGotFocus();
            this.MouseEnter += (s, e) => OnMouseEnter();
            this.MouseLeave += (s, e) => OnMouseExit();
            this.PreviewMouseLeftButtonDown += (s, e) => OnLeftButtonDown();
            this.PreviewMouseLeftButtonUp += (s, e) => OnLeftButtonUp();
            this.MouseMove += (s, e) => OnDrag();
            this.KeyDown += (s, e) => OnKeyDownCallback();
            this.PreviewTextInput += textBoxPrice_PreviewTextInput;
            MainTextBox.TextChanged += (s, e) => TextChangedCallback();
            InputMethod.SetIsInputMethodEnabled(MainTextBox, false);

            
            Background = Brushes.Transparent;
            BorderBrush = Brushes.DarkOrange;
            Foreground = Brushes.White;
            Height = FontSize.Value + 3;
            Width = FontSize.Value * 4;
        }

        private Subject<double> onValueChanged = new();
        public IObservable<double> OnValueChanged => onValueChanged;

        private Subject<double> onDecided = new();
        public IObservable<double> OnDecided => onDecided;

        private void TextChangedCallback()
        {
            string str = Regex.Replace(Text, @"-[^0-9.]", "");
            string prev = Text;

            if (str == "")
            {
                return;
            }

            while (true)
            {
                if (str[0] == '.')
                {
                    str = str.Substring(1);
                }
                else
                {
                    break;
                }
            }




            if (prev != str)
            {
                Value = double.Parse(str);
            }

        }

        private void OnKeyDownCallback()
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                Keyboard.ClearFocus();
            }
        }

        public double Value
        {
            get
            {
                if (double.TryParse(this.Text, out double v))
                {
                    return v;
                }
                else
                {
                    throw new Exception($"パースに失敗しました。テキストボックスの値={Text}");
                }
            }
            set
            {
                double temp = value;
                if (MaxValue.Value is double m && temp > m) temp = m;
                if (MinValue is double n && temp < n) temp = n;

                if (OnlyInt)
                {
                    temp = (int)temp;
                    Text = temp.ToString();
                }
                else
                {
                    Text = temp.ToString("0.00");
                }

                ResetValue = temp;
                onValueChanged.OnNext(temp);
            }
        }

        private double ResetValue = 0;
        private bool IsEditing { get; set; } = false;
        private bool IsUnderMouse { get; set; } = false;
        private bool IsPointerDown { get; set; } = false;
        private bool CanDrag => !IsEditing && IsPointerDown;
        private (int posX, double prevVal) MouseDownPointX_BoxValue { get; set; }
        public string Text
        {
            get => MainTextBox.Text;
            set => MainTextBox.Text = value;
        }

        private State State
        {
            get
            {
                if (IsEditing) return State.Editing;

                if (IsUnderMouse) return State.MouseOver;

                else return State.None;
            }
        }


        private void OnMouseExit()
        {
            Cursor = Cursors.Arrow;
            IsUnderMouse = false;
        }
        private void OnMouseEnter()
        {
            if (!IsEditing)
            {
                Cursor = Cursors.SizeWE;
            }
            else
            {
                Cursor = Cursors.IBeam;
            }
            IsUnderMouse = true;
        }
        private void OnGotFocus()
        {
            IsEditing = true;
            MainTextBox.Select(this.Text.Length, 0);
            if (IsPointerDown)
            {
                Keyboard.ClearFocus();
            }
            else
            {
                Cursor = Cursors.IBeam;
            }
        }
        private void OnLostFocus()
        {
            IsEditing = false;
            if (!double.TryParse(Text, out double n))
            {
                Value = ResetValue;
            }
            else
            {
                Value = n;
            }

            if (IsUnderMouse)
            {
                Cursor = Cursors.SizeWE;
            }

            onDecided.OnNext(Value);
        }

        private bool moved;
        private void OnLeftButtonDown()
        {
            CaptureMouse();
            moved = false;

            IsPointerDown = true;
            MouseDownPointX_BoxValue = ((int)(Mouse.GetPosition(this).X), this.Value);
        }
        private void OnDrag()
        {
            if(this.IsMouseCaptured)
            {
                moved = true;

            }
            if (CanDrag)
            {
                int currentX = (int)(Mouse.GetPosition(this).X);
                double diff = currentX - MouseDownPointX_BoxValue.posX;
                Value = MouseDownPointX_BoxValue.prevVal + diff * Diff_par_movePixel.Value;
            }
        }

        private void OnLeftButtonUp()
        {
            IsPointerDown = false;

            if (moved)
            {
                if (MouseDownPointX_BoxValue.posX == (int)(Mouse.GetPosition(this).X))
                {
                    Keyboard.Focus(this);
                }

                if (this.IsMouseCaptured)
                {
                    ReleaseMouseCapture();
                }
                moved = false;
                onDecided.OnNext(Value);
            }
            else
            {
                Keyboard.Focus(MainTextBox);

                MainTextBox.CaptureMouse();
                MainTextBox.Select(this.Text.Length, 0);
            }
        }

        private void textBoxPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9.]").IsMatch(e.Text);
        }



        public ReactiveProperty<double> Diff_par_movePixel { get; } = new(1d);
        //public ReactiveProperty<bool> OnlyInt { get; set; } = new(true);
        public ReactiveProperty<double> MaxValue { get; } = new(int.MaxValue);
        new public ReactiveProperty<int> FontSize { get; } = new(12);

        private TextBox MainTextBox { get; set; }

        private static readonly DependencyProperty OnlyIntProperty = DependencyProperty.Register(
"OnlyInt", // プロパティ名を指定
typeof(bool), // プロパティの型を指定
typeof(NumberOnlyTextBox), // プロパティを所有する型を指定
new PropertyMetadata(true)); // メタデータを指定。ここではデフォルト値を設定してる
        public bool OnlyInt
        {
            get => (bool)(this.GetValue(OnlyIntProperty));
            set => this.SetValue(OnlyIntProperty, value);
        }


        private static readonly DependencyProperty InitialValueProperty = DependencyProperty.Register(
"InitialValue", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(NumberOnlyTextBox), // プロパティを所有する型を指定
new PropertyMetadata(0d)); // メタデータを指定。ここではデフォルト値を設定してる
        public double InitialValue
        {
            get => (double)(this.GetValue(InitialValueProperty));
            set => this.SetValue(InitialValueProperty, value);
        }

        private static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
"MinValue", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(NumberOnlyTextBox), // プロパティを所有する型を指定
new PropertyMetadata(0d)); // メタデータを指定。ここではデフォルト値を設定してる
        public double MinValue
        {
            get => (double)(this.GetValue(MinValueProperty));
            set => this.SetValue(MinValueProperty, value);
        }



    }


}

