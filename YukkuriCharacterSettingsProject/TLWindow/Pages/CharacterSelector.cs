using Data;
using Data.CharacterData;
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
using YukkuriCharacterSettingsProject.NavigationService;

namespace YukkuriCharacterSettingsProject.TLWindow
{

    public class CharacterSelector : Control
    {
        static CharacterSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CharacterSelector), new FrameworkPropertyMetadata(typeof(CharacterSelector)));
        }

        public CharacterSelector()
        {
            ViewModel = new CharacterSelectorVM(MoveToSerifEditor);

            DataContext = ViewModel;

            Loaded += (s, e) =>
            {
                Window.GetWindow(this).KeyDown += OnKeyDownForCharaBind;
                Window.GetWindow(this).KeyUp += OnKeyUpForCharaBind;
            };

        }

        private void OnKeyUpForCharaBind(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                Window.GetWindow(this).Close();
            }

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightAlt || e.SystemKey == Key.LeftCtrl || e.SystemKey == Key.RightCtrl)
            {
                Window.GetWindow(this).Close();
            }
        }

        /// <summary>
        /// Alt-Ctrl の状態で追加キーが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDownForCharaBind(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.LeftAlt && e.Key != Key.RightAlt && e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl)
            {
                var keyCharaBinds = TimelineDataController.Instance.KeyCharacterBinds;
                if (keyCharaBinds.Find(x => x.Key == e.Key) is KeyCharacterBind setting)
                {
                    MoveToSerifEditor(CharaDataSaveSystem.Instance.GetAllDict()[setting.CharaSettingName]);
                }
                else
                {
                    Window.GetWindow(this).KeyDown -= OnKeyDownForCharaBind;
                    Window.GetWindow(this).KeyUp -= OnKeyUpForCharaBind;

                    var page = new SelectCharaForKeyView(e.Key);

                    Window.GetWindow(this).Navigate(page);
                }
            }
        }

        public CharacterSelectorVM ViewModel { get; }

        private void MoveToSerifEditor(CharacterSettingManager setting)
        {
            Window.GetWindow(this).KeyDown -= OnKeyDownForCharaBind;
            Window.GetWindow(this).KeyUp-= OnKeyUpForCharaBind;
            var serifEditor = new SerifEditorView();
            serifEditor.Initialize(setting);
            this.Navigate(serifEditor); 
        }
    }
}
