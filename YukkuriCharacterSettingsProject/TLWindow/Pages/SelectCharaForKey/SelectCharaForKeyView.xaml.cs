using Data;
using Data.CharacterData;
using Reactive.Bindings.Extensions;
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
using YukkuriCharacterSettingsProject.CharacterParameterEditorUserControl;
using YukkuriCharacterSettingsProject.NavigationService;

namespace YukkuriCharacterSettingsProject.TLWindow
{
    /// <summary>
    /// SelectCharaForKeyView.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectCharaForKeyView : UserControl
    {
        public SelectCharaForKeyView(Key key)
        {
            InitializeComponent();

            this.ObserveDependencyProperty(ItemWidthProperty).Subscribe(_ => ViewModel.ItemWidth.Value = ItemWidth);
            this.ObserveDependencyProperty(ItemHeightProperty).Subscribe(_ => ViewModel.ItemHeight.Value = ItemHeight);


            var settings = GetShowingCharaSettings();
            if (settings != null)
            {
                foreach (var item in settings)
                {
                    ViewModel.AddCharacter(item, OnClicked);

                }
            }

            Loaded += (s, e) =>
            {
                ViewModel.ItemWidth.Value = ItemWidth;
                ViewModel.ItemHeight.Value = ItemHeight;
            };
            Key = key;
        }

        private SelectCharaForKeyVM ViewModel => (SelectCharaForKeyVM)DataContext;
        private void OnClicked(CharacterSettingManager settings)
        {
            TimelineDataController.Instance.KeyCharacterBinds.Add(new KeyCharacterBind(settings.SettingName, Key));
            Window.GetWindow(this).Close();
        }



        #region Dependency Properties

        public static readonly DependencyProperty ItemWidthProperty =
    DependencyProperty.Register(
        "ItemWidth", // プロパティ名を指定
        typeof(double), // プロパティの型を指定
        typeof(SelectCharaForKeyView), // プロパティを所有する型を指定
        new PropertyMetadata(150d)); // メタデータを指定。ここではデフォルト値を設定してる

        // 依存関係プロパティのCLRのプロパティのラッパー
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
DependencyProperty.Register(
"ItemHeight", // プロパティ名を指定
typeof(double), // プロパティの型を指定
typeof(SelectCharaForKeyView), // プロパティを所有する型を指定
new PropertyMetadata(150d)); // メタデータを指定。ここではデフォルト値を設定してる

        // 依存関係プロパティのCLRのプロパティのラッパー
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public Key Key { get; }

        #endregion

        protected virtual void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        protected virtual List<CharacterSettingManager> GetShowingCharaSettings()
        {
            List<CharacterSettingManager> ans = new();
            HashSet<string> Used 
                = new(TimelineDataController.Instance.KeyCharacterBinds.ConvertAll(x => x.CharaSettingName));
            foreach (var item in CharaDataSaveSystem.Instance.Settings.Settings)
            {
                if (!Used.Contains(item.SettingName))
                {
                    ans.Add(item);
                }
            }
            return ans;
        }
    }
}
