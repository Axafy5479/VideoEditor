
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;

namespace AquestalkProj
{

    public static class CreateVoice
    {
        private static Dictionary<VoiceType, VoiceMakerBase>? voiceMakerMap;
        private static Dictionary<VoiceType, VoiceMakerBase> VoiceMakerMap
        {
            get
            {
                if(voiceMakerMap == null)
                {
                    voiceMakerMap = new Dictionary<VoiceType, VoiceMakerBase>();
                    voiceMakerMap.Add(VoiceType.F1, new F1Voice());
                    voiceMakerMap.Add(VoiceType.F2, new F2Voice());
                    voiceMakerMap.Add(VoiceType.M1, new M1Voice());
                    voiceMakerMap.Add(VoiceType.M2, new M2Voice());
                    voiceMakerMap.Add(VoiceType.Mc1, new Mc1Voice());
                    voiceMakerMap.Add(VoiceType.Mc2, new Mc2Voice());
                    voiceMakerMap.Add(VoiceType.Rb, new RbVoice());
                }
                return voiceMakerMap;
            }
        }


        public static byte[] Create(string pronunciation, VoiceType voiceType,double speed,double pitch)
        {
            return VoiceMakerMap[voiceType].CreateWav(pronunciation,speed,pitch);
        }


    }

    internal abstract class VoiceMakerBase
    {
        internal byte[] CreateWav(string pronunciation,double speed,double pitch)
        {

            int speedPercent = (int)(speed * 100);
            int size = 0;

            //音声ファイルとしてそのまま保存可能なバイト列の先頭ポイントを取得
            IntPtr wavPtr = GetWavPtr(pronunciation, speedPercent, ref size);

            //成功判定
            if (wavPtr == IntPtr.Zero)
            {
                return null;
            }

            //C#で扱えるようにマネージド側へコピー
            byte[] wav = new byte[size];
            Marshal.Copy(wavPtr, wav, 0, size);

            //アンマネージドポインタは用が無くなった瞬間に解放
            Release(wavPtr);

            //そのまま再生
            return wav;
        }

        protected abstract IntPtr GetWavPtr(string pronunciation,int speed, ref int size);
        protected abstract void Release(IntPtr ptr);

    }
    internal class F1Voice: VoiceMakerBase
    {

        const string dllName_f1 = @"lib\aq1\f1\AquesTalk.dll";

        protected override IntPtr GetWavPtr(string pronunciation, int speed, ref int size)
        {
            return AquesTalk_Synthe(pronunciation, speed, ref size);
        }

        protected override void Release(IntPtr ptr)
        {
            AquesTalk_FreeWave(ptr);
        }

        [DllImport(dllName_f1)]
        extern static IntPtr AquesTalk_Synthe(string koe, int speed, ref int size);

        [DllImport(dllName_f1)]
        extern static void AquesTalk_FreeWave(IntPtr wavPtr);
    }
    internal class F2Voice : VoiceMakerBase
    {

        const string dllName = @"lib\aq1\f2\AquesTalk.dll";


        protected override IntPtr GetWavPtr(string pronunciation, int speed, ref int size)
        {
            return AquesTalk_Synthe(pronunciation, speed, ref size);
        }

        protected override void Release(IntPtr ptr)
        {
            AquesTalk_FreeWave(ptr);
        }

        [DllImport(dllName)]
        extern static IntPtr AquesTalk_Synthe(string koe, int speed, ref int size);

        [DllImport(dllName)]
        extern static void AquesTalk_FreeWave(IntPtr wavPtr);
    }
    internal class M1Voice : VoiceMakerBase
    {

        const string dllName = @"lib\aq1\m1\AquesTalk.dll";

        protected override IntPtr GetWavPtr(string pronunciation, int speed, ref int size)
        {
            return AquesTalk_Synthe(pronunciation, speed, ref size);
        }

        protected override void Release(IntPtr ptr)
        {
            AquesTalk_FreeWave(ptr);
        }

        [DllImport(dllName)]
        extern static IntPtr AquesTalk_Synthe(string koe, int speed, ref int size);

        [DllImport(dllName)]
        extern static void AquesTalk_FreeWave(IntPtr wavPtr);
    }
    internal class M2Voice : VoiceMakerBase
    {

        const string dllName = @"lib\aq1\m2\AquesTalk.dll";

        protected override IntPtr GetWavPtr(string pronunciation, int speed, ref int size)
        {
            return AquesTalk_Synthe(pronunciation, speed, ref size);
        }

        protected override void Release(IntPtr ptr)
        {
            AquesTalk_FreeWave(ptr);
        }

        [DllImport(dllName)]
        extern static IntPtr AquesTalk_Synthe(string koe, int speed, ref int size);

        [DllImport(dllName)]
        extern static void AquesTalk_FreeWave(IntPtr wavPtr);
    }
    internal class Mc1Voice : VoiceMakerBase
    {

        const string dllName = @"lib\aq1\jgr\AquesTalk.dll";

        protected override IntPtr GetWavPtr(string pronunciation, int speed, ref int size)
        {
            return AquesTalk_Synthe(pronunciation, speed, ref size);
        }

        protected override void Release(IntPtr ptr)
        {
            AquesTalk_FreeWave(ptr);
        }

        [DllImport(dllName)]
        extern static IntPtr AquesTalk_Synthe(string koe, int speed, ref int size);

        [DllImport(dllName)]
        extern static void AquesTalk_FreeWave(IntPtr wavPtr);
    }
    internal class Mc2Voice : VoiceMakerBase
    {

        const string dllName = @"lib\aq1\dvd\AquesTalk.dll";

        protected override IntPtr GetWavPtr(string pronunciation, int speed, ref int size)
        {
            return AquesTalk_Synthe(pronunciation, speed, ref size);
        }

        protected override void Release(IntPtr ptr)
        {
            AquesTalk_FreeWave(ptr);
        }

        [DllImport(dllName)]
        extern static IntPtr AquesTalk_Synthe(string koe, int speed, ref int size);

        [DllImport(dllName)]
        extern static void AquesTalk_FreeWave(IntPtr wavPtr);
    }
    internal class RbVoice : VoiceMakerBase
    {

        const string dllName = @"lib\aq1\r1\AquesTalk.dll";


        protected override IntPtr GetWavPtr(string pronunciation, int speed, ref int size)
        {
            return AquesTalk_Synthe(pronunciation, speed, ref size);
        }

        protected override void Release(IntPtr ptr)
        {
            AquesTalk_FreeWave(ptr);
        }

        [DllImport(dllName)]
        extern static IntPtr AquesTalk_Synthe(string koe, int speed, ref int size);

        [DllImport(dllName)]
        extern static void AquesTalk_FreeWave(IntPtr wavPtr);
    }
}
