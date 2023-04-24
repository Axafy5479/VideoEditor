using ControlzEx.Standard;
using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Timeline.CustomControl
{
    public class TimelineLayerObject : Grid
    {
        static TimelineLayerObject()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineLayerObject), new FrameworkPropertyMetadata(typeof(TimelineLayerObject)));
        }

        public TimelineLayerObject()
        {
            this.MouseLeftButtonDown += UserControl_MouseDown;
            this.MouseMove += UserControl_MouseMove;
            this.MouseUp += UserControl_MouseUp;
            this.Loaded += _Loaded;
        }

        public TimelineLayerObjectViewModel TLLayerDataContext =>(TimelineLayerObjectViewModel)DataContext;

        private Pointer Pointer => Pointer.Instance;

        internal BlankRect BlankRect { get; private set; }

        private bool isLoaded { get; set; }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                BlankRect = new();
                isLoaded = true;
                Layer = TLBase.TimelineLayerObjects.IndexOf(this);
                BlankRect = new BlankRect() {
                    TLBase = TLBase
                };
                Pointer.SelectedBlank.Subscribe(x => {
                    if (x.layer == Layer)
                    {
                        BlankRect.Visibility = Visibility.Visible;
                        BlankRect.Move(Layer, x.l, x.r);
                    }
                    else
                    {
                        BlankRect.Visibility = Visibility.Hidden;
                    }
                });
            }
        }

        public int Layer { get; set; } = -1;

        private static readonly DependencyProperty TLBaseProperty = DependencyProperty.Register(
"TLBase", // プロパティ名を指定
typeof(TimelineBase), // プロパティの型を指定
typeof(TimelineLayerObject), // プロパティを所有する型を指定
new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる
        public TimelineBase TLBase
        {
            get => (TimelineBase)(this.GetValue(TLBaseProperty));
            set => this.SetValue(TLBaseProperty, value);
        }

        private int ScreenPosToFrame(double windowPosX)
        {
            var fromCenter = this.PointFromScreen(new Point(windowPosX, 0)).X;
            var frame = (int)Math.Round(fromCenter / TimelineDataController.Instance.PixelPerFrame.Value);

            frame = Math.Min(frame, TimelineDataController.Instance.Length.Value);
            frame = Math.Max(frame, 0);
            return frame;
        }


        private int CurrentFrame_Waiting { get; set; } = -1;
        private bool IsRunning { get; set; } = false;
        private async Task TryChangeFrame()
        {
            IsRunning = true;
            Application.Current.Dispatcher.Invoke(() =>
            {
                TimelineDataController.Instance.ChangeCurrentFrame(CurrentFrame_Waiting, false);
                CurrentFrame_Waiting = -1;
            });

            await Task.Delay(30);

            if(CurrentFrame_Waiting != -1)
            {
                await TryChangeFrame();
            }
            IsRunning = false;
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = this.PointToScreen(e.GetPosition(this));
            TimelineDataController.Instance.ChangeCurrentFrame(ScreenPosToFrame(pos.X),false);
            ShowBlankArea(e);
            this.CaptureMouse();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                var pos = this.PointToScreen(e.GetPosition(this));
                int target = ScreenPosToFrame(pos.X);
                CurrentFrame_Waiting = target;
                if (!IsRunning) Task.Run(() => TryChangeFrame());
                ShowBlankArea(e);


            }
        }


        internal void ShowBlankArea(MouseEventArgs? e)
        {
            if (e == null)
            {
                Pointer.AddSelectedBlank(-1, -1, -1);
                return;
            }

            var tlBaseObj = TLBase.GetCursorLayer(e);
            if (tlBaseObj is null)
            {
                Pointer.AddSelectedBlank(-1, -1, -1);
                return;
            }

            var ansTemp = TimelineDataController.Instance.GetBlank(tlBaseObj.Layer, TimelineDataController.Instance.CurrentFrame.Value);
            if (ansTemp == null)
            {
                Pointer.AddSelectedBlank(-1, -1, -1);
                return;
            }
            (int l, int? r) ans = (ValueTuple<int, int?>)ansTemp;
            int l = ans.l;
            int r = ans.r ?? TimelineDataController.Instance.Length.Value;

            Pointer.AddSelectedBlank(tlBaseObj.Layer, l, r);
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
        }

    }
}

