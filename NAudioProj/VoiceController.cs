using NAudioProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquestalkProj
{
    public class VoiceController : IDisposable
    {
        public VoiceController(string pronuntiation, double pitch, double speed, VoiceType voiceType)
        {
            Player = new Talker(pronuntiation, pitch/100, speed/100, voiceType);
            Pronuntiation = pronuntiation;
        }

        public string FilePath { get; }
        public bool IsPlaying => Player.IsPlaying;
        private Talker Player { get; }
        public bool Disposed { get; private set; } = false;
        public string Pronuntiation { get; }

        public void ChangeVolume(double ratio)
        {
            ratio = Math.Min(Math.Max(ratio, 0.000001), 0.999999);

            if (Disposed)
            {
                throw new Exception("DisposeされたAudioControllerを操作することはできません");
            }
            Player.ChangeVolume(ratio);
        }

        public void Dispose()
        {
            Disposed = true;
            Player.Stop();
            //IsPlaying = false;
        }

        public void Play(int offsetFrame, double volume)
        {
            if (Disposed)
            {
                throw new Exception("DisposeされたAudioControllerを操作することはできません");
            }
            if (IsPlaying)
            {
                Stop();
            }
            //IsPlaying = true;
            Player.Play(offsetFrame / 60d, volume);
        }

        public void Stop()
        {
            //IsPlaying = false;
            Player.Stop();
        }
    }
}
