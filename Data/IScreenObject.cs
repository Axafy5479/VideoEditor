using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TimeController;

namespace ScreenProject
{
    public interface IScreenObject
    {
        public ITimelineObjectViewModel TlItemObjVM { get; }
        public bool IsPlaying { get; }
        public void Show(int globalFrame, bool isPlaying);
        public void Stop();
        public void Dispose();
    }
}
