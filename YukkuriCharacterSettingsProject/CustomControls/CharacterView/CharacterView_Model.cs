using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukkuriCharacterSettingsProject.CustomControls
{
    public partial class CharacterView_Model : ObservableObject
    {


        public void SetCharaData(CharacterSettingManager settings)
        {
            Settings = settings;
        }
        [ObservableProperty] private CharacterSettingManager _Settings;
    }
}
