using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukkuriCharacterSettings.CustomControls.CharaGridItems
{
    public class CharaGridItemsVM
    {
        public CharaGridItemsVM(string materialPath)
        {
            Model = new CharaGridItemsModel(materialPath);
        }

        public CharaGridItemsModel Model { get; }

    }
}
