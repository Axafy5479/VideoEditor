using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AquestalkProj
{
    public static class Kanji2Koe
    {
        private static string? key = null;
        private static string Key
        {
            get
            {
                if (key == null)
                {
                    string? obtained = Environment.GetEnvironmentVariable("Kanji2KoeKey", EnvironmentVariableTarget.User);
                    if (obtained != null)
                    {
                        key = obtained;
                    }
                    else
                    {
                        throw new Exception("Kanji2Koeのプロダクトキーの取得に失敗しました");
                    }
                }

                return key;
            }
        }

        public static string Convert(string serif)
        {
            int err_code = 0;
            IntPtr handl = AqKanji2Koe_Create(libPath, ref err_code);
            AqKanji2Koe_SetDevKey(Key);

            //成功判定
            if (handl == IntPtr.Zero)
            {
                return null;
            }

            StringBuilder pronunciation = new StringBuilder();
            AqKanji2Koe_Convert_sjis(handl, serif, pronunciation, 1024);
            AqKanji2Koe_Release(handl);
            return pronunciation.ToString();
        }



        const string dllName = "lib\\AqKanji2Koe.dll";
        const string libPath = @"lib\aq_dic\";

        [DllImport(dllName,CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        extern static int AqKanji2Koe_Convert_sjis(IntPtr handl,string kanji,StringBuilder koe, Int32 nBufKoe);

        [DllImport(dllName)]
        extern static void AqKanji2Koe_SetDevKey(string license_key);

        [DllImport(dllName)]
        extern static IntPtr AqKanji2Koe_Create(string pathDic, ref int wavPtr);

        [DllImport(dllName)]
        extern static void AqKanji2Koe_Release(IntPtr handl);
    }
}
