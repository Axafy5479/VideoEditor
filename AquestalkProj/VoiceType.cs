using System;
using System.Reflection;
using System.Windows.Documents;

namespace AquestalkProj
{
    public enum VoiceType
    {
        [Alias("女声1")]
        F1,

        [Alias("女声2")]
        F2,

        [Alias("男声1")]
        M1,

        [Alias("男声2")]
        M2,

        [Alias("機械1")]
        Mc1,

        [Alias("機械2")]
        Mc2,

        [Alias("ロボット")]
        Rb,
    }



    public static class EnumExtensions
    {
        // 列挙値に付与されている別名の取得
        public static string ToAliasString(this Enum target)
        {
            var attribute = target.GetType().GetMember(target.ToString())[0].GetCustomAttribute(typeof(AliasAttribute));

            return attribute == null ? null : ((AliasAttribute)attribute).Text;
        }
        // 列挙体に付与されている別名の取得
        public static string ToAliasEnumString(this Enum target)
        {
            Attribute attribute = target.GetType().GetCustomAttribute(typeof(AliasAttribute));

            return attribute == null ? null : ((AliasAttribute)attribute).Text;
        }

        public static bool TryParseFromAlias<TEnum>(string s, out object? wd) where TEnum : struct
        {
            var enumValues = Enum.GetValues(typeof(TEnum));
            foreach (Enum item in enumValues)
            {
                var type = item.GetType();

                FieldInfo field = item.GetType().GetField(item.ToString());
                if (field.GetCustomAttribute<AliasAttribute>() is AliasAttribute att)
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
