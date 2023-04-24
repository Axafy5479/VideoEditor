using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
 
// ファイル選択ダイアログの名前空間を using
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace CustomControls
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public static class SearchExplorerDialog
    {

        /// <summary>
        /// ファイルを開くダイアログボックスを表示
        /// </summary>
        public static void Show()
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "テキストファイル (*.txt)|*.txt|全てのファイル (*.*)|*.*";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をメッセージボックスに表示
                MessageBox.Show(dialog.FileName);
            }
        }

        /// <summary>
        /// 名前を付けて保存ダイアログボックスを表示
        /// </summary>
        public static void button2_Click()
        {
            // ダイアログのインスタンスを生成
            var dialog = new SaveFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "テキストファイル (*.txt)|*.txt|全てのファイル (*.*)|*.*";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をメッセージボックスに表示
                MessageBox.Show(dialog.FileName);
            }
        }

        public static void ShowSearchFolderDialog( Action<string> OnFolderDecided)
        {
            using (var file_dlg = new CommonOpenFileDialog()
            {
                Title = "フォルダを選択してください。"
                ,
                InitialDirectory = @"E:\"
                ,
                IsFolderPicker = true,
                 
            })
            {
                if (file_dlg.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }
                OnFolderDecided(file_dlg.FileName);
            }
        }


    }
}