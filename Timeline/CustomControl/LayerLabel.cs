using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Timeline.CustomControl
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Timeline.CustomControl"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Timeline.CustomControl;assembly=Timeline.CustomControl"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:LayerLabel/>
    ///
    /// </summary>
    public class LayerLabel : Control
    {
        static LayerLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayerLabel), new FrameworkPropertyMetadata(typeof(LayerLabel)));
        }

        public LayerLabel()
        {
            Loaded += Initialize;
        }

        private void Initialize(object sender, RoutedEventArgs e)
        {
            this.GetChildOfType<Rectangle>().MouseDown+=OnClicked;
            TLBase = this.GetParentOfType<UserControl1>().GetChildOfType<TimelineBase>();
        }

        private void OnClicked(object sender, MouseButtonEventArgs e)
        {
            TLBase.InsertTLBaseObject(int.Parse(Label.Split(" ")[1]));
        }

        private TimelineBase TLBase { get; set; }

        public static readonly DependencyProperty MyLabelProperty =
                                DependencyProperty.Register("Label",
                                typeof(string),
                                typeof(LayerLabel),
                                new PropertyMetadata(null));

        public string Label
        {
            get => (string)this.GetValue(MyLabelProperty);
            set=> this.SetValue(MyLabelProperty, value);
        }



        public static readonly DependencyProperty MyColorProperty =
                                DependencyProperty.Register("Foreground",
                                typeof(Brush),
                                typeof(LayerLabel),
                                new PropertyMetadata(null));

        public Brush Foreground
        {
            get => (Brush)this.GetValue(MyColorProperty);
            set => this.SetValue(MyColorProperty, value);
        }





    }
}
