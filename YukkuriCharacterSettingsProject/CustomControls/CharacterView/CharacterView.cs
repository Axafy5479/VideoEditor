using CommunityToolkit.Mvvm.Input;
using Reactive.Bindings;
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

namespace YukkuriCharacterSettingsProject.CustomControls
{
  
    public class CharacterView : Control
    {
        static CharacterView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CharacterView), new FrameworkPropertyMetadata(typeof(CharacterView)));
        }

        public CharacterView()
        {
            ViewModel = new CharacterView_VM();
            DataContext = ViewModel;
        }
        public CharacterView_VM ViewModel { get; }

        public void SetCellSize(double x, double y)
        {
            ViewModel.MaxWidth.Value = x;
            ViewModel.MaxHeight.Value = y;
        }


        public void SetCharaData(CharacterSettingManager settings, Action<CharacterSettingManager> onClick)
        {
            ViewModel.SetCharaData(settings, onClick);
        }


    }
}
