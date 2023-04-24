using CommunityToolkit.Mvvm.Input;
using Data.CharacterData;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace YukkuriCharacterSettingsProject.CharacterParameterEditorUserControl
{
    public class CharaParamEditorVM
    {
        public CharaParamEditorVM()
        {
            OnDeleteButtonClicked = new RelayCommand(()=>OnDeleted.OnNext(Unit.Default));
        }

        public Subject<Unit> OnDeleted { get; } = new();

        public CharacterSettingManager Settings { get; private set; }

        public void SetCharaSettings(CharacterSettingManager characterSettingManager)
        {
            Settings = characterSettingManager;
            Settings.ObserveProperty(x => x.SettingName).Subscribe(name => SettingName.Value = name);
            Settings.SubtitleSettings.FontColorBrush.Where(b=>b is not null).Subscribe(b => FontColor.Value=b);



            UpdatePreviewFace();
        }

        public void UpdatePreviewFace()
        {
            var charaGrid = Settings.GetThumbnail();
            CharacterPreview.Value = charaGrid;
        }


        public ReactiveProperty<string> SettingName { get; } = new("");

        public ReactiveProperty<SolidColorBrush> FontColor { get; set; } = new();


        public ReactiveProperty<Grid> CharacterPreview { get; set; } = new();

        public ReactiveProperty<string> VoiceSampleText { get; } = new("ゆっくりしていってね!!");

        public ICommand OnDeleteButtonClicked { get; set; }
    }
}
