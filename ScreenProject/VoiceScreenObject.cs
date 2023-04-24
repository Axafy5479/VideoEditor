using AquestalkProj;
using Data;
using NAudioProj;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class VoiceScreenObject : Control, IScreenObject
    {
        static VoiceScreenObject()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VoiceScreenObject), new FrameworkPropertyMetadata(typeof(VoiceScreenObject)));
        }

        public VoiceScreenObject(VoiceObjViewModel vm)
        {
            TlItemObjVM = vm;
            ViewModel = vm;
            DataContext = vm;
            VoiceController = new(vm.Pronuntition.Value, vm.Pitch.Value,vm.Speed.Value, vm.VoiceType.Value);
        }


        public VoiceObjViewModel ViewModel { get; }


        public Control Control => this;

        public ITimelineObjectViewModel TlItemObjVM { get; }

        private VoiceController VoiceController { get; }




        public void Show(int globalFrame, bool isPlaying)
        {
            var localFrame = globalFrame - ViewModel.Frame.Value;

            if (isPlaying && !VoiceController.IsPlaying)
            {
                VoiceController.Play(globalFrame - TlItemObjVM.Frame.Value + TlItemObjVM.OffsetFrame.Value, ViewModel.GetCurrentVolumeRatio(localFrame));
            }

            VoiceController.ChangeVolume(ViewModel.GetCurrentVolumeRatio(localFrame));

        }

        public void Stop()
        {
            if (VoiceController.IsPlaying)
            {
                VoiceController.Stop();
            }
        }

        public void Dispose()
        {
        }


        public bool IsPlaying => VoiceController.IsPlaying;
    }
}
