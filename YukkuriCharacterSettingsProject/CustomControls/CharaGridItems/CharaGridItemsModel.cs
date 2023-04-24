using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukkuriCharacterSettings.CustomControls.CharaGridItems
{
    public partial class CharaGridItemsModel : ObservableObject
    {
        public CharaGridItemsModel(string materialPath)
        {
            MaterialPath = materialPath;
        }

        public void SetMaterialPath(string materialPath)
        {

        }
        [ObservableProperty] private string _MaterialPath;
    }
}
