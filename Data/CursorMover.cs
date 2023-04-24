using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Timeline
{
    internal class CursorMover
    {
        private DateTime TimerStart { get; set; }
        private Timer Timer { get; set; }
        private int InitialFrame { get; set; }
        private TimelineDataController tlData => TimelineDataController.Instance;

        internal CursorMover()
        {
            tlData.Playing.Subscribe(play =>
            {
                if (play)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            });
        }

        private void Start()
        {
            InitialFrame = tlData.CurrentFrame.Value;
            TimerStart = DateTime.Now;
            Timer = new Timer(_Start, null,0,4);
        }

        private int prevFrame = -1;
        private void _Start(object? state)
        {
            var currentValue = DateTime.Now - TimerStart;
            var frame = (int)(Math.Round(currentValue.TotalSeconds * 60));

            if (frame != prevFrame)
            {

                    if (tlData.Length.Value == frame)
                    {
                        Stop();
                        return;
                    }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // ここで再生中か否かを確認
                    // もし直前にStop()が呼ばれていた場合、おかしな位置にカーソルがワープしてしまう
                    if (tlData.Playing.Value)
                    {
                        TimelineDataController.Instance.ChangeCurrentFrame(InitialFrame + frame, tlData.Playing.Value);
                    }
                });
            }

            prevFrame = frame;
        }

        private void Stop()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Timer?.Dispose();
                InitialFrame = -1;
                tlData.Playing.Value = false;
            });
        }

        
    }
}
