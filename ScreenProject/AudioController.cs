using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeController;
using Timeline.CustomControl;

namespace NAudioProj
{
    public class AudioController : IDisposable
    {
        public AudioController(string filePath)
        {
            FilePath = filePath;
            Player = new(filePath);
        }

        public string FilePath { get; }
        public bool IsPlaying { get; private set; }
        private AudioPlayer Player { get; }
        public bool Disposed { get; private set; } = false;

        public void ChangeVolume(double ratio)
        {
            ratio = Math.Min(Math.Max(ratio, 0.000001), 0.999999);

            if(Disposed)
                {
                throw new Exception("DisposeされたAudioControllerを操作することはできません");
            }
            Player.ChangeVolume(ratio);
        }

        public void Dispose()
        {
            Disposed = true;
            Player.Dispose();
            IsPlaying = false;
        }

        public async Task Play(int offsetFrame, double volume)
        {
            if (Disposed)
            {
                throw new Exception("DisposeされたAudioControllerを操作することはできません");
            }
            if (IsPlaying)
            {
                Stop();
            }
            IsPlaying = true;
            await Player.Play(offsetFrame / 60d, 0, volume);
        }

        public void Stop()
        {
            IsPlaying = false;
            Player.Dispose();
        }
    }
}
