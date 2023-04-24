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
using YukkuriCharacterSettingsProject.CustomControls.AddCharaButton;

namespace YukkuriCharacterSettingsProject.CustomControls
{
  
    public class AddYukkuriButton : Control
    {
        static AddYukkuriButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AddYukkuriButton), new FrameworkPropertyMetadata(typeof(AddYukkuriButton)));
        }

        public AddYukkuriButton()
        {
            ViewModel = new AddYukkuriButtonVM();
            DataContext = ViewModel;
        }

        public AddYukkuriButtonVM ViewModel { get; internal set; }
    }
}
