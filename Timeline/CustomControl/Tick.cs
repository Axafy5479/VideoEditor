using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WB.Pool;

namespace Timeline.CustomControl
{
    public class Tick : Control, IPoolObject
    {
        static Tick()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tick), new FrameworkPropertyMetadata(typeof(Tick)));
        }


        public int Id { get; set; }

        public int Frame { get; set; }

        public Action Return { get; private set; }

        public Panel GetParent()
        {
            return (TickLayer)this.Parent;
        }

        public void Initialize()
        {
            Frame = -1;
        }

        public void SetReturnMethod(Action returnMethod)
        {
            Return = returnMethod;
        }


        private static readonly DependencyProperty X1Property = DependencyProperty.Register(
"X1", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(Tick), // プロパティを所有する型を指定
new PropertyMetadata(0d)); // メタデータを指定。ここではデフォルト値を設定してる

        public double X1
        {
            get => (double)(this.GetValue(X1Property));
            set => this.SetValue(X1Property, value);
        }

        private static readonly DependencyProperty X2Property = DependencyProperty.Register(
"X2", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(Tick), // プロパティを所有する型を指定
new PropertyMetadata(0d)); // メタデータを指定。ここではデフォルト値を設定してる

        public double X2
        {
            get => (double)(this.GetValue(X2Property));
            set => this.SetValue(X2Property, value);
        }

        private static readonly DependencyProperty Y1Property = DependencyProperty.Register(
"Y1", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(Tick), // プロパティを所有する型を指定
new PropertyMetadata(0d)); // メタデータを指定。ここではデフォルト値を設定してる

        public double Y1
        {
            get => (double)(this.GetValue(Y1Property));
            set => this.SetValue(Y1Property, value);
        }

        private static readonly DependencyProperty Y2Property = DependencyProperty.Register(
"Y2", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(Tick), // プロパティを所有する型を指定
new PropertyMetadata(10d)); // メタデータを指定。ここではデフォルト値を設定してる

        public double Y2
        {
            get => (double)(this.GetValue(Y2Property));
            set => this.SetValue(Y2Property, value);
        }

        private static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
"Color", // プロパティ名を指定
typeof(Brush), // プロパティの型を指定
typeof(Tick), // プロパティを所有する型を指定
new PropertyMetadata(Brushes.White)); // メタデータを指定。ここではデフォルト値を設定してる

        public Brush Color
        {
            get => (Brush)(this.GetValue(ColorProperty));
            set => this.SetValue(ColorProperty, value);
        }

        private static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
"Thickness", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(Tick), // プロパティを所有する型を指定
new FrameworkPropertyMetadata(
            1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)); // メタデータを指定。ここではデフォルト値を設定してる

        public double Thickness
        {
            get => (double)(this.GetValue(ThicknessProperty));
            set => this.SetValue(ThicknessProperty, value);
        }


        private static readonly DependencyProperty TextProperty = DependencyProperty.Register(
"Text", // プロパティ名を指定
typeof(string), // プロパティの型を指定
typeof(Tick), // プロパティを所有する型を指定
new PropertyMetadata("")); // メタデータを指定。ここではデフォルト値を設定してる

        public string Text
        {
            get => (string)(this.GetValue(TextProperty));
            set => this.SetValue(TextProperty, value);
        }
    }





}

