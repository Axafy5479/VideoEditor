//using MahApps.Metro.Controls;
//using System.Windows.Controls.Primitives;
//using System.Windows.Controls;
//using System.Windows;
//using System.Windows.Media;


//public static class ComboboxColor
//{
//    /// <summary>
//    /// ComboBoxBackground プロパティ 変更イベントメソッド
//    /// </summary>
//    /// <param name="dpObj">添付対象コントロール</param>
//    /// <param name="e">イベントデータ</param>
//    private static void ComboBoxBackgroundPropertyChanged(
//            DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
//    {
//        if (!(dpObj is ComboBox comboBox))
//        {
//            return;
//        }

//        if (!comboBox.IsLoaded)
//        {
//            comboBox.Loaded += ComboBox_Loaded;
//        }
//        else
//        { //Loaded済みの時はメソッドを直接呼ぶ
//            ComboBox_Loaded(comboBox, null);
//        }
//    }

//    /// <summary>
//    /// ComboBox Loadedイベントメソッド
//    /// </summary>
//    /// <param name="sender">イベントソース</param>
//    /// <param name="e">イベントデータ</param>
//    private static void ComboBox_Loaded(object sender, RoutedEventArgs e)
//    {
//        var comboBox = sender as ComboBox;
//        comboBox.Loaded -= ComboBox_Loaded;

//        var toggleButton = comboBox.Template?.
//                FindName("toggleButton", comboBox) as ToggleButton;
//        var boxBorder = toggleButton?.Template?.
//                FindName("templateRoot", toggleButton) as Border;

//        if (boxBorder == null)
//        {
//            return;
//        }

//        //ComboBoxHelper.

//       // var bgColor = ComboBoxHelper.GetComboBoxBackground(comboBox);

//        if (bgColor == null)
//        {
//            //Local値をクリアして、Templateの値を適用させる。
//            boxBorder.ClearValue(Border.BackgroundProperty);
//        }
//        else
//        {
//            boxBorder.SetValue(Border.BackgroundProperty, bgColor);
//        }
//        return;
//    }


//    /// <summary>
//    /// ComboBox 背景色を取得します。
//    /// </summary>
//    /// <param name="obj">設定対象</param>
//    /// <returns>背景色</returns>
//    public static Brush GetComboBoxBackground(DependencyObject obj)
//    {
//        return (Brush)obj.GetValue(ComboBoxBackgroundProperty);
//    }

//    /// <summary>
//    /// ComboBox 背景色を設定します。
//    /// </summary>
//    /// <param name="obj">設定対象</param>
//    /// <param name="value">背景色</param>
//    public static void SetComboBoxBackground(DependencyObject obj, Brush value)
//    {
//        obj.SetValue(ComboBoxBackgroundProperty, value);
//    }

//    /// <summary>ComboBox 背景色を設定します。</summary>
//    public static readonly DependencyProperty ComboBoxBackgroundProperty =
//        DependencyProperty.RegisterAttached("ComboBoxBackground",
//                    typeof(Brush),
//                    typeof(ComboBoxHelper),
//                    new FrameworkPropertyMetadata(null,
//                        FrameworkPropertyMetadataOptions.AffectsRender,
//                        ComboBoxBackgroundPropertyChanged));

//}
