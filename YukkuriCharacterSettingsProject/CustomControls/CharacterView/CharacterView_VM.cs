using CommunityToolkit.Mvvm.Input;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using YukkuriCharacterSettingsProject;

namespace YukkuriCharacterSettingsProject.CustomControls

{
    public class CharacterView_VM
    {
        public CharacterView_VM()
        {
            Model = new CharacterView_Model();

        }

        public void SetCharaData(CharacterSettingManager settings, Action<CharacterSettingManager>? onClick)
        {
            Settings = settings;

            OnClick = new RelayCommand(() => onClick?.Invoke(Settings));

            Model.SetCharaData(settings);

            Content.Value = new() { settings.GetThumbnail() };

            PixelWidth.Value = Content.Value[0].Width;
            PixelHeight.Value = Content.Value[0].Height;


            MaxWidth.Subscribe(_ => UpdateTargetSize());
            MaxHeight.Subscribe(_ => UpdateTargetSize());
            UpdateTargetSize();
        }

        private void UpdateTargetSize()
        {
            if (MaxHeight.Value / MaxWidth.Value < Ratio)
            {
                Size.Value = MaxHeight.Value / PixelHeight.Value;
            }
            else
            {
                Size.Value = MaxWidth.Value / PixelWidth.Value;
            }
        }


        public CharacterView_Model Model { get; }

        public ReactiveProperty<double> PixelWidth { get; } = new(200);
        public ReactiveProperty<double> PixelHeight { get; } = new(300);

        public ReactiveProperty<double> Size { get; } = new(1);

        public double Ratio => PixelHeight.Value / PixelWidth.Value;

        public ReactiveProperty<double> MaxWidth { get; set; } = new(120);
        public ReactiveProperty<double> MaxHeight { get; set; } = new(80);

        public ReactiveProperty<List<Grid>> Content { get; set; } = new();
        public CharacterSettingManager Settings { get; private set; }
        public RelayCommand OnClick { get; set; }

    }
}
