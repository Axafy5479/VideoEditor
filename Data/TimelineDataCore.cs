using CommunityToolkit.Mvvm.ComponentModel;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YukkuriCharacterSettingsProject;

namespace Data
{

    [MessagePackObject(keyAsPropertyName: true)]
    public partial class TimelineDataCore : ObservableObject
    {
        public List<TimelineItem> Items { get; } = new();

        [ObservableProperty] private int _CurrentFrame;
        [ObservableProperty] private int _CurrentLayer;
        [ObservableProperty] private int _Length = 600;
        [ObservableProperty] private int _MaxLayer = 100;
        [ObservableProperty] private int _PixelWidth = 1920;
        [ObservableProperty] private int _PixelHeight = 1080;
        [ObservableProperty] private double _PixelPerFrame = 10;
        public List<KeyCharacterBind> KeyCharacterBinds { get; set; } = new();
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class KeyCharacterBind
    {
        public KeyCharacterBind(string charaSettingName, Key key)
        {
            CharaSettingName = charaSettingName;
            Key = key;
        }
        public string CharaSettingName { get; set; }
        public Key Key { get; set; }
    }
}
