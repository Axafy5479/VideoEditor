using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Shapes;
using YukkuriCharacterSettingsProject;

namespace Data.CharacterData
{
    public class CharaDataSaveSystem
    {
        private static CharaDataSaveSystem? instance;
        public static CharaDataSaveSystem Instance => instance ??= new();
        private CharaDataSaveSystem()
        {
            Load();
        }

        private const string PATH = "./settings/characters.chara3f";
        private const string DIR_PATH = "./settings/";

        public CharacterSettings? Settings { get; private set; }

        private void Load()
        {
            if(!Directory.Exists(DIR_PATH)) Directory.CreateDirectory(DIR_PATH);
                Settings = new CharacterSettings();
            if (!File.Exists(PATH))
            {
                Save();
            }
            else
            {
                using var stream = new FileStream(PATH, FileMode.Open, FileAccess.Read);
                var lists = MessagePackSerializer.Deserialize<CharacterSettings>(stream).Settings;
                Settings.Settings.AddRange(lists);
            }
        }

        public void Save()
        {
            byte[] data = MessagePackSerializer.Serialize(Settings);
            using var stream = new FileStream(PATH, FileMode.Create, FileAccess.Write);
            stream.Write(data, 0, data.Length);
        }

        public void Remove(CharacterSettingManager settings)
        {
            bool canRemove = Settings.Settings.Remove(settings);
            if(!canRemove)
            {
                throw new Exception("削除対象となるsettingsが一覧に存在しませんでした");
            }

            Save();
        }

        public void Add(CharacterSettingManager settings)
        {
            Settings.Settings.Add(settings);
            Save();
        }

        public Dictionary<string, CharacterSettingManager> GetAllDict()
        {
            var ans = new Dictionary<string, CharacterSettingManager>();
            foreach (var item in Settings.Settings)
            {
                ans.Add(item.SettingName, item);
            }
            return ans;
        }

        [MessagePackObject(true)]
        public class CharacterSettings
        {
            public List<CharacterSettingManager> Settings { get; set; } = new();
        }
    }


}
