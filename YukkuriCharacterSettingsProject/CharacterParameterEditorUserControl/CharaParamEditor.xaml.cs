using AquestalkProj;
using CommunityToolkit.Mvvm.Input;
using Data;
using Data.CharacterData;
using NAudioProj;
using ParameterEditor;
using ParameterEditor.CustomControls;
using ParameterEditor.CustomControls.Color;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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
using YukkuriCharacterSettingsProject.NavigationService;

namespace YukkuriCharacterSettingsProject.CharacterParameterEditorUserControl
{
    /// <summary>
    /// CharaParamEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class CharaParamEditor : UserControl, IDisposable
    {
      

        public CharaParamEditor()
        {
            InitializeComponent();
            ViewModel.OnDeleted.Subscribe(_ => {
                CharaDataSaveSystem.Instance.Remove(Settings);
                OnBackButtonClicked(null, null);
            });
        }
        public CharaParamEditorVM ViewModel => (CharaParamEditorVM)DataContext;
        private CharacterSettingManager Settings { get; set; }
        private CompositeDisposable Disposables { get; } = new();
        public void Dispose()
        {
            Disposables.Dispose();
        }

        public void SetSelectedCharaSettings(CharacterSettingManager characterSettingManager)
        {
            ViewModel.SetCharaSettings(characterSettingManager);
            Settings = characterSettingManager;


            MakeGeneralMenu();
            MakeSubtitleMenu();
            MakeVoiceSettingMenu();
            MakeDefaultFaceSettingMenu();
        }


        #region General
        public ReactiveProperty<WrapPanel> GeneralSettingContents { get; } = new();

        private void MakeGeneralMenu()
        {
            //本体となるWrapPanel
            var wrapPanel = MakeBasePanel(50);


            //素材フォルダのパス
            var materialPath = new TextBlock();
            materialPath.Text = Settings.MaterialPath;
            materialPath.Foreground = Brushes.Gray;
            wrapPanel.Children.Add(materialPath);




            // 設定名
            var settingNameProperty = Settings.ToReactivePropertyAsSynchronized(x => x.SettingName);
            settingNameProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var nameItem = new Parameter_Text(
                new() { settingNameProperty },
                null,
                "設定名"
            );

            wrapPanel.Children.Add(nameItem);


            // アイテムの色
            ReactiveProperty<SolidColorBrush> colorProperty = new(Settings.ItemColor.ToBrushOrNull() ?? Brushes.Transparent);
            colorProperty.Where(b=>b is not null).Subscribe(b=> { 
                Settings.ItemColor = b.Color.ToColorCode();
                Settings.Save();
            });
            var tlItemColorPalette = new Parameter_Color(
                new() { colorProperty },
                null, 
                "TLアイテムの色"
            );
            wrapPanel.Children.Add(tlItemColorPalette);








            GeneralSettingContents.Value = wrapPanel;
        }
        #endregion

        #region Subtitle
        public ReactiveProperty<WrapPanel> SubtitleSettingContents { get; } = new();

        private void MakeSubtitleMenu()
        {
            //本体となるWrapPanel
            var wrapPanel = MakeBasePanel(50);


            // フォント色
            var fontColorProperty = Settings.SubtitleSettings.FontColorBrush;
            fontColorProperty.Subscribe(_=> { 
                Settings.Save(); 
            }).AddTo(Disposables);
            var fontColorPalette = new Parameter_Color(
                new() { fontColorProperty },
                null,
                "フォント色"
            );
            wrapPanel.Children.Add(fontColorPalette);


            // 装飾色
            var styleColorProperty = Settings.SubtitleSettings.StyleColorBrush;
            styleColorProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);

            var styleColorPalette = new Parameter_Color(
                new() { styleColorProperty },
                null,
                "装飾色"
            );
            wrapPanel.Children.Add(styleColorPalette);




            // フォントサイズ
            var fontSizeProperty = Settings.SubtitleSettings.ToReactivePropertyAsSynchronized(x => x.FontSize);
            fontSizeProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var fontSizeParameter = new Parameter_Double(
                new() { fontSizeProperty },
                0,int.MaxValue,
                null,
                "フォントサイズ",
                0.1
            );
            wrapPanel.Children.Add(fontSizeParameter);




            // FontFamily
            var fontFamiilyProperty = Settings.SubtitleSettings.ToReactivePropertyAsSynchronized(x => x.FontFamily);
            fontFamiilyProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var fontFamilyCombobox = new Parameter_FontFamily();
            wrapPanel.Children.Add(fontFamilyCombobox);


            SubtitleSettingContents.Value = wrapPanel;
        }
        #endregion

        #region Voice
        public ReactiveProperty<WrapPanel> VoiceSettingContents { get; } = new();

        private void MakeVoiceSettingMenu()
        {
            //本体となるWrapPanel
            var wrapPanel = MakeBasePanel(50);

            // VoicetypeCombo
            var voicetypeProperty = Settings.VoiceSettings.ToReactivePropertyAsSynchronized(x => x.VoiceType);
            voicetypeProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var voicetypeCombo = new Parameter_EnumCombo();
            voicetypeCombo.Initialize<VoiceType>(new() { voicetypeProperty }, null, "声色");
            wrapPanel.Children.Add(voicetypeCombo);


            // Speed
            var speedProperty = Settings.VoiceSettings.ToReactivePropertyAsSynchronized(x => x.Speed);
            speedProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var speedTextBox = new Parameter_Double(
                new() { speedProperty },
                0,int.MaxValue,
                null,
                "速度",0.1
            );
            wrapPanel.Children.Add(speedTextBox);



            // pitch
            var pitchProperty = Settings.VoiceSettings.ToReactivePropertyAsSynchronized(x => x.Pitch);
            pitchProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var pitchTextBox = new Parameter_Double(
                new() { pitchProperty },
                0, int.MaxValue,
                null,
                "ピッチ", 0.1
            );
            wrapPanel.Children.Add(pitchTextBox);



            // volume
            var volumeProperty = Settings.VoiceSettings.ToReactivePropertyAsSynchronized(x => x.Volume);
            volumeProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var volumeTextBox = new Parameter_Double(
                new() { volumeProperty },
                0, int.MaxValue,
                null,
                "音量", 0.1
            );
            wrapPanel.Children.Add(volumeTextBox);


            // volume
            var panProperty = Settings.VoiceSettings.ToReactivePropertyAsSynchronized(x => x.Pan);
            panProperty.Subscribe(_ => Settings.Save()).AddTo(Disposables);
            var panTextBox = new Parameter_Double(
                new() { panProperty },
                0, int.MaxValue,
                null,
                "余韻", 0.1
            );
            wrapPanel.Children.Add(panTextBox);

            VoiceSettingContents.Value = wrapPanel;
        }
        #endregion

        #region Face Setting
        public ReactiveCollection<CharaImageCombobox> DefaultFaceSettingContents { get; } = new();

        private void MakeDefaultFaceSettingMenu()
        {
            //本体となるWrapPanel
            var wrapPanel = MakeBasePanel(200);

            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Back3));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Back2));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Back1));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Body));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.FaceColor));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Hair));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Mouth));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Eye));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Eyebrow));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.HairT));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Other3));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Other2));
            DefaultFaceSettingContents.Add(MakePartsDropDown(PartsType.Other1));
        }

        private CharaImageCombobox MakePartsDropDown(PartsType partsType)
        {

            var temp = new CharaImageCombobox();

            List<string> allMaterialPathes = Settings.CharaImagesDictionary.Dict[partsType].Values.ToList().ConvertAll(x => x.MaterialImagePath);
            allMaterialPathes.Insert(0,$"なし ({partsType.ToAliasEnumString().name})");

            string? currentSelectedPath = Settings.UserSettingFace.GetDefault()?.Find(x => x.Type == partsType)?.Path;
            var onPartsChanged = new ReactiveProperty<string>();
            if (!string.IsNullOrEmpty(currentSelectedPath))
            {
                onPartsChanged.Value = currentSelectedPath;
            }

            onPartsChanged.Subscribe(path =>
            {
                if(File.Exists(path))
                {
                    Settings.UserSettingFace.Change(0,partsType, path);
                    ViewModel.UpdatePreviewFace();
                }
                else
                {
                    Settings.UserSettingFace.Change(0, partsType, null);
                    ViewModel.UpdatePreviewFace();
                }
                Settings.Save();
            });


            temp.Initialize(allMaterialPathes,
                new() { onPartsChanged },
                null);
            return temp;
        }
        #endregion





        private WrapPanel MakeBasePanel(double height)
        {
            var wrapPanel = new WrapPanel();
            wrapPanel.VerticalAlignment = VerticalAlignment.Stretch;
            wrapPanel.ItemHeight = height;
            wrapPanel.ItemWidth = 400;
            wrapPanel.Margin = new Thickness(10, 10, 10, 10);
            return wrapPanel;
        }

        private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Navigate(new YukkuriCharacterSettings());
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void PlaySampleVoice(object sender, RoutedEventArgs e)
        {
            var pronunciation = Kanji2Koe.Convert(ViewModel.VoiceSampleText.Value);
            byte[] soundData = CreateVoice.Create(pronunciation, Settings.VoiceSettings.VoiceType, Settings.VoiceSettings.Speed/100, Settings.VoiceSettings.Pitch/100);

            Task.Run(()=>new Talker(pronunciation, Settings.VoiceSettings.Pitch/100, Settings.VoiceSettings.Speed/100, Settings.VoiceSettings.VoiceType)
                .Play(0, Settings.VoiceSettings.Volume / 100));
        }
    }
}
