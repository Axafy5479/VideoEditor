using System;
using System.Collections.Generic;
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
 
    public class Parameter_FontFamily : Control
    {
        static Parameter_FontFamily()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Parameter_FontFamily), new FrameworkPropertyMetadata(typeof(Parameter_FontFamily)));
        }

        private static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
"Title", // プロパティ名を指定
typeof(string), // プロパティの型を指定
typeof(Parameter_FontFamily), // プロパティを所有する型を指定
new PropertyMetadata("")); // メタデータを指定。ここではデフォルト値を設定してる
        public string Title
        {
            get => (string)(this.GetValue(TitleProperty));
            set => this.SetValue(TitleProperty, value);
        }
    }
}
