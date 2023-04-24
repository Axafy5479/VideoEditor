using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AquestalkProj;
using System.Diagnostics;

namespace NAudioProj
{
    public class Talker
    {

        public Talker(string pronuntiation,double pitch, double speed, VoiceType voiceType)
        {
            if(speed<0.000001)
            {
                throw new Exception("再生速度が小さすぎます");
            }

            if (pitch < 0.000001)
            {
                throw new Exception("ピッチが小さすぎます");
            }

            Pronuntiation = pronuntiation;
            Pitch = pitch;
            Speed = speed;
            VoiceType = voiceType;
            var soundData = CreateSoundData();

            WaveAnalyzer.ChangeSamplingRate(soundData, 8000, pitch);

            using (var ms = new MemoryStream(soundData))
            {
                var audioStream = new WaveFileReader(ms);

                ByteParSec = (audioStream.WaveFormat.BitsPerSample / 8) * audioStream.WaveFormat.SampleRate * audioStream.WaveFormat.Channels;
                AllLengthSec = ((double)audioStream.Length) / ByteParSec;
                audioStream.Dispose();
            }

        }

        public int ByteParSec { get; }
        public double AllLengthSec { get; }
        public string Pronuntiation { get; }
        public double Pitch { get; }
        public double Speed { get; }
        public VoiceType VoiceType { get; }
        private WaveOut OutputDevice { get; } = new();

        /// <summary>
        /// 音声の非同期再生
        /// </summary>
        /// <param name="playFromSec">再生開始時間</param>
        /// <param name="delaySec"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Play(double playFromSec, double volume)
        {
            if (IsPlaying) throw new Exception("再生中です");
            IsPlaying = true;

            var soundData = CreateSoundData();
            var ms = new MemoryStream(soundData);
            var afr = new WaveFileReader(ms);
            afr.Position = 0;

            if (playFromSec < 0)
            {
                //再生開始位置が負の場合、その分待機する
                int delay = (int)(-playFromSec * 1e3);
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
            else
            {
                //再生開始位置が正の場合、その分シークする
                afr.Position = (long)(ByteParSec * playFromSec);
            }

            //afr.Seek((long)(ByteParSec * playFromSec), SeekOrigin.Begin);
            OutputDevice.Init(afr);
            OutputDevice.Volume = (float)volume;
            OutputDevice.Play();

            OutputDevice.Play();

            while (OutputDevice.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(10);
            }

            ms.Dispose();
            afr.Dispose();
            IsPlaying = false;
        }




        public void ChangeVolume(double volume)
        {
            if (OutputDevice != null)
            {
                OutputDevice.Volume = (float)volume;
            }
        }

        private byte[] CreateSoundData()
        {
            var soundData = CreateVoice.Create(Pronuntiation, VoiceType, Speed, Pitch);
            WaveAnalyzer.ChangeSamplingRate(soundData, 8000, Pitch);

            return soundData;
        }

        public void SaveToFile(string fileName)
        {
            var soundData = CreateSoundData();
            using var ms = new MemoryStream(soundData);
            var afr = new WaveFileReader(ms);
            WaveFileWriter.CreateWaveFile(fileName, afr);

        }

        public bool IsPlaying { get; private set; }

        public void Stop()
        {
            OutputDevice?.Stop();
        }

    }
}
