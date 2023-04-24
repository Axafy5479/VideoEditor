using Data.CharacterData;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using YukkuriCharacterSettingsProject.CustomControls;

namespace YukkuriCharacterSettingsProject
{
    public class CharaSettingsVM
    {
        public CharaSettingsVM()
        {
        }




        public void AddAddingButton(Action<CharacterSettingManager> onClick)
        {
            var addButton = new AddYukkuriButton()
            {
                Width = ItemWidth.Value,
                Height = ItemHeight.Value,
            };

            addButton.ViewModel.OnAddButtonClicked.Subscribe(path => {
                CharacterSettingManager setting = new CharacterSettingManager(path);
                CharaDataSaveSystem.Instance.Add(setting);
                AddCharacter(setting, onClick);
            });
            addButton.ViewModel.RectWidth.Value = ItemWidth.Value * 0.3;
            addButton.ViewModel.RectHeight.Value = ItemWidth.Value * 0.3 / 4;
            addButton.ViewModel.X.Value = -addButton.ViewModel.RectWidth.Value / 2;
            addButton.ViewModel.Y.Value = -addButton.ViewModel.RectHeight.Value / 2;
            Characters.Add(addButton);

            ItemWidth.Subscribe(w => addButton.Width = w );
            ItemHeight.Subscribe(h => addButton.Height = h);
        }

        public void AddCharacter(CharacterSettingManager charaSetting, Action<CharacterSettingManager> onClick)
        {
            var cv = new CharacterView();
            cv.ViewModel.MaxWidth = ItemWidth;
            cv.ViewModel.MaxHeight = ItemHeight;
            cv.SetCharaData(charaSetting, onClick);
            Characters.Add(cv);
        }

        public ReactiveCollection<FrameworkElement> Characters { get; } = new();

        public ReactiveProperty<double> ItemWidth { get; } = new(100);
        public ReactiveProperty<double> ItemHeight { get; } = new(100);
    }
}
