using AquestalkProj;
using CommunityToolkit.Mvvm.ComponentModel;
using Data;
using Data.CharacterData;
using MessagePack;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Timeline.Error;

namespace YukkuriCharacterSettingsProject
{
    public enum PartsType
    {
        [CharaAlias("後3", "後")]
        Back3 = 0,

        [CharaAlias("後2", "後")]
        Back2 = 1,

        [CharaAlias("後1", "後")]
        Back1 = 2,

        [CharaAlias("体")]
        Body = 3,

        [CharaAlias("顔色")]
        FaceColor = 4,

        [CharaAlias("髪")]
        Hair = 5,

        [CharaAlias("口")]
        Mouth = 6,

        [CharaAlias("目")]
        Eye = 7,

        [CharaAlias("眉")]
        Eyebrow = 8,

        [CharaAlias("髪(半透明)")]
        HairT = 9,

        [CharaAlias("他3", "他")]
        Other3 = 10,

        [CharaAlias("他2", "他")]
        Other2 = 11,

        [CharaAlias("他1", "他")]
        Other1 = 12,
    }

    /// <summary>
    /// ゆっくりキャラ設定のクラス
    /// 1つのマテリアルパスから複数の設定を生成してよい
    /// </summary>
    [MessagePack.MessagePackObject(true)]
    public partial class CharacterSettingManager : ObservableObject
    {
        public readonly HashSet<PartsType> DefaultIgnoreParts = new HashSet<PartsType>() { PartsType.Other2,PartsType.Other3, PartsType.Back3, PartsType.Back2 };

        public CharacterSettingManager(string materialPath)
        {
            MaterialPath = materialPath;
            SettingName = Path.GetFileNameWithoutExtension(materialPath);
            CharaImagesDictionary = new(materialPath);
        }

        [MessagePack.SerializationConstructor]
        public CharacterSettingManager() { }

        [ObservableProperty]private string _MaterialPath;
        [ObservableProperty] private string _SettingName;
        [ObservableProperty] private string _ItemColor = "#ffffff";
        //[ObservableProperty] private string _Voice;

        public SubtitleSettings SubtitleSettings { get; set; }
        public VoiceSettings VoiceSettings { get; set; } = new();

        public UserSettingFace UserSettingFace { get; set; } = new UserSettingFace();

        public CharaImagesDictionary CharaImagesDictionary { get; set; }

        public Grid GetThumbnail()
        {
            var faceDict = UserSettingFace.GetDefault();

            List<PartsPathPair> defaultFace;

            if (faceDict == null)
            {
                List<PartsPathPair> partsPathMap = new();
                foreach (var item in YukkuriCharaUtility.GetAllPartsTypes())
                {
                    if (DefaultIgnoreParts.Contains(item)) continue;

                    if (CharaImagesDictionary.Dict[item].TryGetValue("00", out var info))
                    {
                        partsPathMap.Add(new(item, info.MaterialImageUri.ToString()));
                    }
                }

                UserSettingFace.SetDefault(partsPathMap);
                defaultFace = partsPathMap;
            }
            else
            {
                defaultFace = faceDict;
            }

            Grid grid = new Grid();
            double w = -1;
            double h = -1;
            foreach (var item in defaultFace)
            {
                var bmp = new BitmapImage(new Uri(item.Path));
                grid.Children.Add(new Image() { Source = bmp});
                if(item.Type == PartsType.Body)
                {
                   w = bmp.Width;
                   h = bmp.Height;
                }

                grid.Width = bmp.Width;
                grid.Height = bmp.Height;
            }

            if(w>0 && h>0)
            {
                grid.Width = w;
                grid.Height = h;
            }

            return grid;
        }

        public void Save()
        {
            
            CharaDataSaveSystem.Instance.Save();
        }
  
    }


    [MessagePack.MessagePackObject(true)]
    public class UserSettingFace
    {
        public UserSettingFace()
        {
            Faces = new();
        }

        public List<List<PartsPathPair>> Faces { get; set; }

        public List<PartsPathPair>? GetDefault()
        {
            return Faces.Any() ? Faces[0] : null;
        }

        public void Add(List<PartsPathPair> faceSet)
        {
            Faces.Add(faceSet);
        }

        public void SetDefault(List<PartsPathPair> faceSet)
        {
            if(Faces.Any())
            {
                Faces[0] = faceSet;
            }
            else
            {
                Faces.Add(faceSet);
            }
        }

        public void Change(int index, PartsType type, string? path)
        {
            var pair = Faces[index].Find(f=>f.Type == type);
            bool existPath = path!=null && File.Exists(path);

            if(pair == null)
            {
                if (existPath)
                {
                    Faces[index].Add(new PartsPathPair(type, path));
                    Faces[index].Sort((a,b)=>a.Type.CompareTo(b.Type));
                }
            }
            else
            {
                if (existPath)
                {
                    pair.Path = path;
                }
                else
                {
                    Faces[index].Remove(pair);
                }
            }
        }
    }

    [MessagePack.MessagePackObject(true)]
    public class PartsPathPair
    {
        public PartsPathPair(PartsType type, string path)
        {
            Type = type;
            Path = path;
        }

        [MessagePack.SerializationConstructor]
        public PartsPathPair() { }

        public PartsType Type { get; set; }
        public string Path { get; set; }
    }

    [MessagePack.MessagePackObject(true)]
    public partial class VoiceSettings : ObservableObject
    {
        [ObservableProperty] private VoiceType _VoiceType = VoiceType.F1;
        [ObservableProperty] private double _Speed = 100;
        [ObservableProperty] private double _Pitch = 100;
        [ObservableProperty] private double _Volume = 100;
        [ObservableProperty] private double _Pan = 0.1;
    }

    [MessagePack.MessagePackObject(true)]
    public partial class SubtitleSettings : ObservableObject
    {
        public SubtitleSettings()
        {
            UUID = Guid.NewGuid().ToString();
        }

        public string UUID { get; }


        [IgnoreMember] public ReactiveProperty<SolidColorBrush> FontColorBrush { get; set; } = new(Brushes.White);
        [IgnoreMember] public ReactiveProperty<SolidColorBrush> StyleColorBrush { get; set; } = new(Brushes.Black);
        [IgnoreMember] public ReactiveProperty<FontFamily> Font = new(new FontFamily("meiryo"));
        [ObservableProperty] private double _FontSize = 80;
        [ObservableProperty] private double _X = 0;
        [ObservableProperty] private double _Y = 530;
        [ObservableProperty] private TextStyle _Style = TextStyle.None;
        [ObservableProperty] private bool _Topmost = true;
        [ObservableProperty] private bool _Italic = false;
        [ObservableProperty] private bool _Bold = false;


        public string FontColor
        {
            get=>FontColorBrush.Value.Color.ToColorCode();
            set => FontColorBrush.Value = value.ToBrushOrNull() ?? Brushes.Transparent;
        }

        public string StyleColor
        {
            get => StyleColorBrush.Value.Color.ToColorCode();
            set => StyleColorBrush.Value = value.ToBrushOrNull() ?? Brushes.Transparent;
        }

        public string FontFamily
        {
            get => Font.Value.FamilyNames[XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)];
            set=>Font.Value = new FontFamily(value);
        }
    }

    /// <summary>
    /// マテリアルパスから画像集を生成するクラス
    /// </summary>
    [MessagePackObject(true)]
    public class CharaImagesDictionary
    {
        public CharaImagesDictionary(string materialPath)
        {
            MaterialPath = materialPath;
        }

        [MessagePack.SerializationConstructor]
        public CharaImagesDictionary() { }

        public void Load()
        {
            Dict.Clear();

            HashSet<PartsType> types = new();
            foreach (PartsType type in Enum.GetValues(typeof(PartsType)))
            {
                Dict.Add(type, new Dictionary<string, PartsImageInfo>());
                types.Add(type);
            }

            var directories = new HashSet<string>(Directory.GetDirectories(MaterialPath));


            foreach (var type in YukkuriCharaUtility.GetAllPartsTypes())
            {
                (var name, var folder) = type.ToAliasEnumString();
                string directory = Path.Combine(MaterialPath, folder);
                if (directories.Contains(directory))
                {
                    string[] filePathes = Directory.GetFiles(directory);
                    Array.Sort(filePathes);
                    foreach (var path in filePathes)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(path);
                        if (fileName.Contains('.'))
                        {
                            continue;
                        }


                        var files = Array.FindAll(filePathes, f => Path.GetFileNameWithoutExtension(f).Contains(fileName + "."));

                        if (files.Length > 0)
                        {
                            var filesList = files.ToList();
                            filesList.Insert(0, path);
                            Dict[type].Add(fileName, new PartsStreamImageInfo(type, filesList));
                        }
                        else
                        {
                            Dict[type].Add(fileName, new PartsImageInfo(type, path));
                        }
                    }
                }
            }

        }

        [IgnoreMember]

        private Dictionary<PartsType, Dictionary<string, PartsImageInfo>>? dict;
        public Dictionary<PartsType, Dictionary<string, PartsImageInfo>> Dict
        {
            get{
                if(dict == null)
                {
                    dict = new();
                    Load();
                }
                return dict;
            }
        }

        public string MaterialPath { get; set; }

        [IgnoreMember]
        public string DirName => Path.GetDirectoryName(MaterialPath) ?? "";
    }
}
