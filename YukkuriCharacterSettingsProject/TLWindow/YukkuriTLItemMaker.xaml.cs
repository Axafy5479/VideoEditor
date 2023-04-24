using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace YukkuriCharacterSettingsProject.TLWindow
{
    /// <summary>
    /// YukkuriTLItemMaker.xaml の相互作用ロジック
    /// </summary>
    public partial class YukkuriTLItemMaker : Window
    {
        public static bool IsShowing => instance != null;
        private static YukkuriTLItemMaker? instance;

        public YukkuriTLItemMaker()
        {
            if(IsShowing)
            {
                throw new Exception("YukkuriTLItemMakerは複数窓に対応していません");
            }

            instance = this;

            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            instance = null;
        }

        public static void CloseWin()
        {
            instance?.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
