using Data.CharacterData;
using Reactive.Bindings;
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

namespace YukkuriCharacterSettingsProject
{
    /// <summary>
    /// YukkuriCharacterSettings.xaml �̑��ݍ�p���W�b�N
    /// </summary>
    public partial class YukkuriCharacterSettings : UserControl
    {
        public YukkuriCharacterSettings()
        {
            InitializeComponent();

            this.ObserveDependencyProperty(ItemWidthProperty).Subscribe(_ => ViewModel.ItemWidth.Value = ItemWidth);
            this.ObserveDependencyProperty(ItemHeightProperty).Subscribe(_ => ViewModel.ItemHeight.Value = ItemHeight);

            ViewModel.AddAddingButton(OnClicked);

            var settings = GetShowingCharaSettings();
            if(settings != null)
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


        }

        private CharaSettingsVM ViewModel => (CharaSettingsVM)DataContext;
        private void OnClicked(CharacterSettingManager settings)
        {
            var editorPage = new CharaParamEditor();
            editorPage.SetSelectedCharaSettings(settings);
            this.Navigate(editorPage);
        }



        #region Dependency Properties

        public static readonly DependencyProperty ItemWidthProperty =
    DependencyProperty.Register(
        "ItemWidth", // �v���p�e�B�����w��
        typeof(double), // �v���p�e�B�̌^���w��
        typeof(YukkuriCharacterSettings), // �v���p�e�B�����L����^���w��
        new PropertyMetadata(150d)); // ���^�f�[�^���w��B�����ł̓f�t�H���g�l��ݒ肵�Ă�

        // �ˑ��֌W�v���p�e�B��CLR�̃v���p�e�B�̃��b�p�[
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
DependencyProperty.Register(
"ItemHeight", // �v���p�e�B�����w��
typeof(double), // �v���p�e�B�̌^���w��
typeof(YukkuriCharacterSettings), // �v���p�e�B�����L����^���w��
new PropertyMetadata(150d)); // ���^�f�[�^���w��B�����ł̓f�t�H���g�l��ݒ肵�Ă�

        // �ˑ��֌W�v���p�e�B��CLR�̃v���p�e�B�̃��b�p�[
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        #endregion

        protected virtual void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        protected virtual List<CharacterSettingManager> GetShowingCharaSettings()
        {
            return CharaDataSaveSystem.Instance.Settings.Settings;
        }
    }
}
