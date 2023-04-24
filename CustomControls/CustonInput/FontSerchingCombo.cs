using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace ParameterEditor
{
    public class FontSerchingCombo : Control
    {
        #region コンストラクタ
        /// <summary>
        /// 静的なコンストラクタ
        /// </summary>
        static FontSerchingCombo()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontSerchingCombo), new FrameworkPropertyMetadata(typeof(FontSerchingCombo)));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontSerchingCombo()
        {
            var list = GetFontList();
            list.Insert(0, new MyFont() { FontFamily = null, LocalFontName = "フォントを選択してください" });
            DataContext = list;
        }
        #endregion

        /// <summary>
        /// すべてのフォントを取得するメソッド
        /// </summary>
        /// <returns></returns>
        private List<MyFont> GetFontList()
        {
            //カレントの言語を取得
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            //すべてのフォントを取得し、MyFontクラスの配列にする
            var fonts = Fonts.SystemFontFamilies.Select(i => new MyFont() { FontFamily = i, LocalFontName = i.Source }).ToArray();
            //フォント名がカレント言語に存在したらカレント言語（つまり日本語）を、
            //存在しなければ英語表記のフォント名を LocalFontNameにセットする。

            fonts.Select(i => i.LocalFontName = i.FontFamily.FamilyNames
                .FirstOrDefault(j => j.Key == this.Language).Value ?? i.FontFamily.Source).ToList();

            return fonts.ToList();
        }

        /// <summary>
        /// フォント情報を保持するクラス
        /// </summary>
        public class MyFont
        {
            public FontFamily FontFamily { get; set; }  //フォントファミリー
            public string LocalFontName { get; set; }   //フォント名
        }

        public ComboBox GetComboBox()
        {
            return (ComboBox)this.GetTemplateChild("FontsCombo");
        }

        new private static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
"Foreground", // プロパティ名を指定
typeof(Brush), // プロパティの型を指定
typeof(FontSerchingCombo), // プロパティを所有する型を指定
new PropertyMetadata(Brushes.White)); // メタデータを指定。ここではデフォルト値を設定してる
        new public Brush Foreground
        {
            get => (Brush)(this.GetValue(ForegroundProperty));
            set => this.SetValue(ForegroundProperty, value);
        }

        new private static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
"Background", // プロパティ名を指定
typeof(Brush), // プロパティの型を指定
typeof(FontSerchingCombo), // プロパティを所有する型を指定
new PropertyMetadata(Brushes.Black)); // メタデータを指定。ここではデフォルト値を設定してる
        new public Brush Background
        {
            get => (Brush)(this.GetValue(BackgroundProperty));
            set => this.SetValue(BackgroundProperty, value);
        }
    }
}
