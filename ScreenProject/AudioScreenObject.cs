using Data;
using NAudioProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeController;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class AudioScreenObject : IScreenObject
    {
        public AudioScreenObject(AudioObjViewModel audioItem, string filePath)
        {
            TlItemObjVM = audioItem;
            FilePath = filePath;
            AudioCtrler = new(filePath);
            audioItem.Volume.Subscribe(v_percent => {
                AudioCtrler.ChangeVolume(v_percent / 100);
            });

        }

        public ITimelineObjectViewModel TlItemObjVM { get; }
        public AudioObjViewModel AudioVM => (AudioObjViewModel)TlItemObjVM;

        public string FilePath { get; }
        public bool IsPlaying { get; private set; }
        private AudioController AudioCtrler { get; }
        public bool Disposed { get; set; } = false;

        public void Dispose()
        {
            AudioCtrler.Stop();
        }

        public void Show(int globalFrame, bool isPlaying)
        {
            var localFrame = globalFrame - AudioVM.Frame.Value;

            if (isPlaying && !AudioCtrler.IsPlaying)
            {
                AudioCtrler.Play(globalFrame - TlItemObjVM.Frame.Value + TlItemObjVM.OffsetFrame.Value, AudioVM.GetCurrentVolumeRatio(localFrame));
            }

            AudioCtrler.ChangeVolume(AudioVM.GetCurrentVolumeRatio(localFrame));

        }

        public void Stop()
        {
                if(AudioCtrler.IsPlaying)
            {
                AudioCtrler.Stop(); 
            }
        }
    }
}
