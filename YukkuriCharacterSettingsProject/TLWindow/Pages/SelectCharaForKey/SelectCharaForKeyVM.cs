

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

namespace YukkuriCharacterSettingsProject.TLWindow
{
    public class SelectCharaForKeyVM
    {
        public SelectCharaForKeyVM()
        {
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
