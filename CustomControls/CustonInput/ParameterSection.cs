using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ParameterEditor
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ParameterEditor"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ParameterEditor;assembly=ParameterEditor"
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
    ///     <MyNamespace:ParameterSection/>
    ///
    /// </summary>
    public class ParameterSection : Control, IDisposable
    {
        static ParameterSection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ParameterSection), new FrameworkPropertyMetadata(typeof(ParameterSection)));
        }

        public ParameterSection()
        {
            DataContext = Parameters;
        }

        public ObservableCollection<IDisposable> Parameters { get; } = new();
        private CompositeDisposable disposables = new CompositeDisposable();
        

        public void AddParameters(List<IDisposable> controls)
        {
            foreach (IDisposable c in controls)
            {
                Parameters.Add(c);
                disposables.Add(c);
            }
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        private static readonly DependencyProperty SectionTitleProperty = DependencyProperty.Register(
"SectionTitle", // プロパティ名を指定
typeof(string), // プロパティの型を指定
typeof(ParameterSection), // プロパティを所有する型を指定
new PropertyMetadata("")); // メタデータを指定。ここではデフォルト値を設定してる
        public string SectionTitle
        {
            get => (string)(this.GetValue(SectionTitleProperty));
            set => this.SetValue(SectionTitleProperty, value);
        }
    }

}
