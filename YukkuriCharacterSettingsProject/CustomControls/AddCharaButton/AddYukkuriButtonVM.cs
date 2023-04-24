using CommunityToolkit.Mvvm.Input;
using CustomControls;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YukkuriCharacterSettingsProject.CustomControls.AddCharaButton
{
    public class AddYukkuriButtonVM
    {

        public AddYukkuriButtonVM()
        {
            OnClick = new RelayCommand(_OnClick);
        }


        public ICommand OnClick { get; }
        public Subject<string> OnAddButtonClicked { get; } = new();

        private void _OnClick()
        {

            SearchExplorerDialog.ShowSearchFolderDialog(OnAddButtonClicked.OnNext);
        }



        public ReactiveProperty<double> X { get; set; } = new();
        public ReactiveProperty<double> Y { get; set; } = new();
        public ReactiveProperty<double> RectWidth { get; set; } = new();
        public ReactiveProperty<double> RectHeight { get; set; } = new();
    }

}
