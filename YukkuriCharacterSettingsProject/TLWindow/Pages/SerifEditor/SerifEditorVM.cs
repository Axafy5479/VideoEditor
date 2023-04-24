using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YukkuriCharacterSettingsProject.CustomControls;

namespace YukkuriCharacterSettingsProject.TLWindow
{
    public class SerifEditorVM
    {
        public void Initialize(CharacterSettingManager setting)
        {
            Setting = setting;

            CharacterView view = new CharacterView();
            view.ViewModel.SetCharaData(setting, null);
            view.SetCellSize(150, 150);
            CharaView.Value = view;
            NameText.Value = setting.SettingName;
        }

        public CharacterSettingManager Setting { get; private set; }

        public ReactiveProperty<FrameworkElement> CharaView { get; private set; } = new();

        public ReactiveProperty<string> NameText { get; } = new();

    }
}
