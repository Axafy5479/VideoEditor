using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YukkuriCharacterSettingsProject
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)] // 適用対象指定
    public class CharaAliasAttribute : Attribute                          // Attributeを継承
    {
        private string _text;                                        // バックストア
        private string _folder;                                        // バックストア
        public string Text => _text;                                 // 文字列取得用プロパティ
        public string Folder => _folder;                                 // 文字列取得用プロパティ
        public CharaAliasAttribute(string text, string? folder = null)
        {
            _text = text; 
            _folder = folder ?? text;
        }         // 文字列を１つとるコンストラクタ 
    }

    public static class YukkuriCharaUtility
    {
        public static List<PartsType> GetAllPartsTypes()
        {
            var ans = new List<PartsType>();
            foreach (PartsType item in Enum.GetValues(typeof(PartsType)))
            {
                ans.Add(item);
            }
            return ans;
        }


        // 列挙体に付与されている別名の取得
        public static (string name, string dirName) ToAliasEnumString(this Enum target)
        {
            FieldInfo field = target.GetType().GetField(target.ToString());
            var attribute = field.GetCustomAttribute<CharaAliasAttribute>();
            return attribute == null ? ("", "") : (attribute.Text, attribute.Folder);

        }

        public static bool TryParseFromAlias<TEnum>(string s, out object? wd) where TEnum : struct
        {
            var enumValues = Enum.GetValues(typeof(TEnum));
            foreach (Enum item in enumValues)
            {
                var type = item.GetType();

                FieldInfo field = item.GetType().GetField(item.ToString());
                if (field.GetCustomAttribute<CharaAliasAttribute>() is CharaAliasAttribute att)
                {

                    var aliasName = att.Text;
                    if (aliasName != null && aliasName == s)
                    {
                        wd = item;
                        return true;
                    }
                }
            }

            wd = null;
            return false;
        }
    }
}
