using AquestalkProj;
using CommunityToolkit.Mvvm.ComponentModel;
using Data.CharacterData;
using MessagePack;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.ObjectExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriCharacterSettingsProject;

namespace Data
{
    public enum JimakuVisibility
    {
        UseCharacterSetting,
        Hide,
        Custom
    }

    public enum TextStyle
    {
        None,
        Border,
        Shadow
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public partial class VoiceItem : TimelineItem, ITextItem, IAudioItem
    {
        [SerializationConstructor]
        public VoiceItem() : base()
        {
           

        }

        public VoiceItem(string serifText, string pronuntiation, CharacterSettingManager charaSetting, int layer, int l, int _length) : base(charaSetting.SettingName, layer, l, _length)
        {
            Text = serifText;
            Pronuntiation = pronuntiation;
            CharaSettingName = charaSetting.SettingName;
            Speed = charaSetting.VoiceSettings.Speed;
            Pitch = charaSetting.VoiceSettings.Pitch;
            VoiceType = charaSetting.VoiceSettings.VoiceType;
        }

        [ObservableProperty] private string _FilePath = "DefaultName";
        [ObservableProperty] private double _Volume = 100;
        [ObservableProperty] private double _PlaybackRate = 100;

        public double ContentOffset
        {
            get => Offset / 60d;
            set => Offset = (int)Math.Round(value * 60d);
        }

        public override FileType FileType => FileType.Yukkuri;

        private string charaSettingName;
        public string CharaSettingName
        {
            get => charaSettingName;
            set
            {
                charaSettingName = value;
                var d = SubtitleSettings;
            }
        }
        [ObservableProperty] private string _Color;
        [ObservableProperty] private string _Color2;
        [ObservableProperty] private double _FontSize;
        [ObservableProperty] private TextStyle _Style = TextStyle.None;
        [ObservableProperty] private bool _Topmost = true;
        [ObservableProperty] private bool _Italic = false;
        [ObservableProperty] private bool _Bold = false;


        private SubtitleSettings? subtitleSettings;

        [IgnoreMember]
        public SubtitleSettings SubtitleSettings
        {
            get { 
                if(subtitleSettings == null)
                {
                    subtitleSettings = CharaDataSaveSystem.Instance.GetAllDict()[CharaSettingName].SubtitleSettings;
                    subtitleSettings.FontColorBrush.Select(b => b.Color.ToColorCode()).Subscribe(y =>
                    {
                        Color = y;
                    });
                    subtitleSettings.StyleColorBrush.Select(b => b.Color.ToColorCode()).Subscribe(y => Color2 = y);
                    subtitleSettings.ObserveProperty(x => x.FontSize).Subscribe(y => {
                        FontSize = y;
                    });
                    subtitleSettings.ObserveProperty(x => x.Style).Subscribe(y => Style = y);
                    subtitleSettings.ObserveProperty(x => x.Topmost).Subscribe(y => Topmost = y);
                    subtitleSettings.ObserveProperty(x => x.Italic).Subscribe(y => Italic = y);
                    subtitleSettings.ObserveProperty(x => x.Bold).Subscribe(y => Bold = y);
                    subtitleSettings.ObserveProperty(x => x.X).Subscribe(y => X = y);
                    subtitleSettings.ObserveProperty(x => x.Y).Subscribe(y => Y = y);
                }
                return subtitleSettings;
                
            }
        }



        [ObservableProperty] private JimakuVisibility _JimakuVisibility = JimakuVisibility.UseCharacterSetting;
        [ObservableProperty] private string _Text;
        [ObservableProperty] private string _Pronuntiation;
        [ObservableProperty] private VoiceType _VoiceType;
        [ObservableProperty] private double _Pitch;
        [ObservableProperty] private double _Speed;

        [ObservableProperty] private bool _IsLooped = false;
        [ObservableProperty] private bool _EchoIsEnabled = false;
        [ObservableProperty] private double _EchoInterval = 0.1;

        #region 描画プロパティ
        [ObservableProperty] private double _X = 0;
        [ObservableProperty] private double _Y = 0;
        [ObservableProperty] private double _Opacity = 100;
        [ObservableProperty] private double _Zoom = 100;
        [ObservableProperty] private double _Rotation = 0;
        [ObservableProperty] private double _FadeIn = 0;
        [ObservableProperty] private double _FadeOut = 0;
        [ObservableProperty] private bool _IsInverted = false;
        #endregion




    }
}


