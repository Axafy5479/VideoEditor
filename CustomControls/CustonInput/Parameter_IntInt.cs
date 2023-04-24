using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
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
//    public class Parameter_IntInt : Control
//    {
//        static Parameter_IntInt()
//        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(Parameter_IntInt), new FrameworkPropertyMetadata(typeof(Parameter_IntInt)));
//        }

//        public Parameter_IntInt(double x = 0,double y = 0, double minValue = int.MinValue)
//        {
//            InitialValue1 = new(x);
//            InitialValue2 = new(y);
//            MinValue.Value = minValue;

//            Loaded += (s, e) =>
//            {
//                NumberOnlyTextBox1 = (NumberOnlyTextBox)this.Template.FindName("NumberOnlyTextBox_Param1",this);
//                NumberOnlyTextBox2 = (NumberOnlyTextBox)this.Template.FindName("NumberOnlyTextBox_Param2",this);
//                NumberOnlyTextBox1.OnValueChanged.Subscribe(onValueChanged1.OnNext);
//                NumberOnlyTextBox2.OnValueChanged.Subscribe(onValueChanged2.OnNext);

//                NumberOnlyTextBox1.OnDecided.Subscribe(onDecided1.OnNext);
//                NumberOnlyTextBox2.OnDecided.Subscribe(onDecided2.OnNext);
//            };
//        }

//        private NumberOnlyTextBox NumberOnlyTextBox1 { get; set; }
//        private NumberOnlyTextBox NumberOnlyTextBox2 { get; set; }

//        private Subject<double> onValueChanged1 = new();
//        public IObservable<double> OnValueChanged1 => onValueChanged1;

//        private Subject<double> onValueChanged2 = new();
//        public IObservable<double> OnValueChanged2 => onValueChanged2;

//        private Subject<double> onDecided1 = new();
//        public IObservable<double> OnDecided1 => onValueChanged1;

//        private Subject<double> onDecided2 = new();
//        public IObservable<double> OnDecided2 => onValueChanged2;




//        private static readonly DependencyProperty MainTitleProperty = DependencyProperty.Register(
//"MainTitle", // プロパティ名を指定
//typeof(string), // プロパティの型を指定
//typeof(Parameter_IntInt), // プロパティを所有する型を指定
//new PropertyMetadata("")); // メタデータを指定。ここではデフォルト値を設定してる
//        public string MainTitle
//        {
//            get => (string)(this.GetValue(MainTitleProperty));
//            set => this.SetValue(MainTitleProperty, value);
//        }

//        private static readonly DependencyProperty SubTitle1Property = DependencyProperty.Register(
//"SubTitle1", // プロパティ名を指定
//typeof(string), // プロパティの型を指定
//typeof(Parameter_IntInt), // プロパティを所有する型を指定
//new PropertyMetadata("")); // メタデータを指定。ここではデフォルト値を設定してる
//        public string SubTitle1
//        {
//            get => (string)(this.GetValue(SubTitle1Property));
//            set => this.SetValue(SubTitle1Property, value);
//        }

//        private static readonly DependencyProperty SubTitle2Property = DependencyProperty.Register(
//"SubTitle2", // プロパティ名を指定
//typeof(string), // プロパティの型を指定
//typeof(Parameter_IntInt), // プロパティを所有する型を指定
//new PropertyMetadata("")); // メタデータを指定。ここではデフォルト値を設定してる
//        public string SubTitle2
//        {
//            get => (string)(this.GetValue(SubTitle2Property));
//            set => this.SetValue(SubTitle2Property, value);
//        }

//        private static readonly DependencyProperty OnlyIntProperty = DependencyProperty.Register(
//"OnlyInt", // プロパティ名を指定
//typeof(bool), // プロパティの型を指定
//typeof(Parameter_IntInt), // プロパティを所有する型を指定
//new PropertyMetadata(true)); // メタデータを指定。ここではデフォルト値を設定してる
//        public bool OnlyInt
//        {
//            get => (bool)(this.GetValue(OnlyIntProperty));
//            set => this.SetValue(OnlyIntProperty, value);
//        }

//        public ReactiveProperty<double> InitialValue1 { get; set; }
//        public ReactiveProperty<double> InitialValue2 { get; set; }
//        public ReactiveProperty<double> MinValue { get; set; } = new();

//    }
}
