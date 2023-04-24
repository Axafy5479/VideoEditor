using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Timeline.CustomControl
{
    public class TimelineScrollKnob : Canvas
    {
        public TimelineScrollKnob()
        {
            MouseLeftButtonDown += OnMouseLeftDown;
            MouseMove += OnMove;
            MouseUp += OnMouseLeftUp;
            Loaded += Initialize;
        }

        private Thumb thumb;
        private Track track;
        private ScrollViewer scrollViewer;
        private void Initialize(object sender, RoutedEventArgs e)
        {


            track = this.GetParentOfType<Track>();
            scrollViewer = this.GetParentOfType<ScrollViewer>();
            thumb = this.GetParentOfType<Thumb>();
            var bar = this.GetParentOfType<ScrollBar>();

            track.IsVisibleChanged += (s, e) => track.Visibility = Visibility.Visible;
            thumb.IsVisibleChanged += (s, e) => thumb.Visibility = Visibility.Visible;

            if (bar.Orientation == Orientation.Vertical)
            {
                ((Grid)this.Parent).Children.Remove(this);
            }
        }

        public bool IsRight { get; set; }

        private double initialCursorPoint;
        private double initialPixelPerFrame;


        private double trackWidth;
        private double contentWidth;
        private double scrollViewerWidth;
        private double thumbWidth;
        private int offsetFrame;
        private int rightFrame;


        private void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            initialCursorPoint = e.GetPosition(track).X;
            initialPixelPerFrame = PixelPerFrame.Value;


             trackWidth = track.ActualWidth;
             contentWidth = ((Grid)scrollViewer.Content).ActualWidth;
             scrollViewerWidth = scrollViewer.ActualWidth;
             thumbWidth = thumb.ActualWidth;
            offsetFrame = (int)(Math.Round(scrollViewer.HorizontalOffset/PixelPerFrame.Value));
            rightFrame = offsetFrame + (int)(Math.Round(scrollViewerWidth / PixelPerFrame.Value));

            e.Handled = true;
            this.CaptureMouse();
        }

        private void OnMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                var diff = initialCursorPoint - e.GetPosition(track).X;

                if (IsRight)
                {
                    var d = Math.Max((thumbWidth + diff), 1);
                    var x = (scrollViewerWidth / contentWidth) * (trackWidth / d);


                    var temp = contentWidth * x;
                    if (temp > scrollViewerWidth)
                    {
                        TimelineDataController.Instance.ChangePixelPerFrame(initialPixelPerFrame * x);
                    }

                    scrollViewer.ScrollToHorizontalOffset(offsetFrame * PixelPerFrame.Value);
                }
                else
                {
                    var d = Math.Max((thumbWidth - diff), 1);
                    var x = (scrollViewerWidth / contentWidth) * (trackWidth / d);


                    var temp = contentWidth * x;
                    if (temp > scrollViewerWidth)
                    {
                        TimelineDataController.Instance.ChangePixelPerFrame(initialPixelPerFrame * x);
                    }

                    var left = rightFrame - (int)(Math.Round(scrollViewerWidth / PixelPerFrame.Value));
                    scrollViewer.ScrollToHorizontalOffset(left * PixelPerFrame.Value);
                }
            }
        }

        private void OnMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }
        }


        private IReadOnlyReactiveProperty<double> PixelPerFrame => TimelineDataController.Instance.PixelPerFrame;

        public static readonly DependencyProperty MainDataContextProperty = DependencyProperty.Register(
"LayoutDataContext", // プロパティ名を指定
typeof(TimelineLayoutViewModel), // プロパティの型を指定
typeof(TimelineScrollKnob), // プロパティを所有する型を指定
new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる

        public TimelineLayoutViewModel LayoutDataContext
        {
            get => (TimelineLayoutViewModel)(this.GetValue(MainDataContextProperty));
            set => this.SetValue(MainDataContextProperty, value);
        }

    }
}
