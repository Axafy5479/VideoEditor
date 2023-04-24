using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Windows;
using YukkuriCharacterSettingsProject.CustomControls;
using Data.CharacterData;
using System.Windows.Input;
using Data;

namespace YukkuriCharacterSettingsProject.TLWindow
{
    public class CharacterSelectorVM
    {

        public CharacterSelectorVM(Action<CharacterSettingManager> moveToSerifEditor)
        {
            var celsize = 100;

            var dict = CharaDataSaveSystem.Instance.GetAllDict();

            foreach (var chara in TimelineDataController.Instance.KeyCharacterBinds)
            {
                var cv = new CharacterView();
                cv.ViewModel.MaxWidth.Value = celsize;
                cv.ViewModel.MaxHeight.Value = celsize;
                cv.SetCharaData(dict[chara.CharaSettingName], moveToSerifEditor);
                cv.SetCellSize(celsize, celsize);

                OtherCharacters.Add(new CharaKey(cv,chara.Key));
            }


        }

        public ReactiveCollection<CharaKey> OtherCharacters { get; } = new();



    }

    public class CharaKey
    {
        public CharaKey(CharacterView view, Key key)
        {
            View = view;
            Key = key.ToString();
            if(key== System.Windows.Input.Key.Return)
            {
                Key = "Enter";
            }
        }

        public CharacterView View { get;}
        public string Key { get; }
    }
}
