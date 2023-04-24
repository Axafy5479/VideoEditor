using Reactive.Bindings;
using Reactive.Bindings.ObjectExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WB.Pool;

namespace Timeline.CustomControl
{
    public class TickLayer : Grid
    {
        public TickLayer()
        {
            Loaded += Initialize;
            TickPool = WBPool.MakeUIElementPool(400, ()=>new Tick());
            RentedTick = new();
        }

        public readonly int[] thresholds = {1, 3, 6, 15, 30, 60, 120, 300, 600, 1800, 3600,
                                            7200, 18000, 36000, 72000};
        public readonly int[] boldTickes = {12, 10, 10, 8, 10, 10, 10, 10, 10, 10, 10,
                                            10, 10, 10, 10};

        bool isLoaded = false;
        private UIElementPool<Tick> TickPool { get; }
        private List<Tick> RentedTick { get; }
        private ScrollViewer scroll;


        private void Initialize(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }
            isLoaded = true;

            scroll = this.GetParentOfType<ScrollViewer>(); 

            MaxFrame.Subscribe(_ => Update());
            PixelPerFrame.Subscribe(_ => Update());
            scroll.ScrollChanged+=(s,e) => Update();
        }


        private void Update()
        {
            RentedTick.ForEach(t => t.Return());
            RentedTick.Clear();

            Width = MaxFrame.Value * PixelPerFrame.Value;
            double ratio = scroll.ActualWidth / Width;

            int l = (int)(Math.Round(scroll.ContentHorizontalOffset / PixelPerFrame.Value));
            int r = l + (int)(Math.Round(scroll.ActualWidth / PixelPerFrame.Value));

            double minPixelPerTick = 10;

            int framePerTick = 360000;
            int boldTick = 3600000;
            for (int i = 0; i < thresholds.Length; i++)
            {
                var framePerTickTemp = thresholds[i];
                var pixelPerTick = PixelPerFrame.Value * framePerTickTemp;
                if (pixelPerTick >= minPixelPerTick)
                {
                    framePerTick = framePerTickTemp;
                    boldTick = boldTickes[i];
                    break;
                }
            }

            l -= (l % (boldTick * framePerTick));

            for (int tick = 0; tick < 200; tick++)
            {
                int frame = framePerTick * tick + l;
                var t = TickPool.Rent();
                RentedTick.Add(t);

                if((frame % (boldTick * framePerTick)) == 0)
                {
                    t.Color = Brushes.White;
                    t.Y2 = 20;
                    TimeSpan ts = TimeSpan.FromMilliseconds((frame * 1000)/60);
                    t.Text = ts.ToString(@"hh\:mm\:ss\.ff");
                    t.Thickness = 3;
                }
                else
                {
                    t.Color = Brushes.White;
                    t.Y2 = 10;
                    t.Text = "";
                    t.Thickness = 1;
                }

                t.RenderTransform = new TranslateTransform(frame * PixelPerFrame.Value, 0);
                this.Children.Add(t);
            }
        }

        public static readonly DependencyProperty PixelPerFrameProperty = DependencyProperty.Register(
"PixelPerFrame", // プロパティ名を指定
typeof(IReadOnlyReactiveProperty<double>), // プロパティの型を指定
typeof(TickLayer), // プロパティを所有する型を指定
new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる

        public IReadOnlyReactiveProperty<double> PixelPerFrame
        {
            get => (IReadOnlyReactiveProperty<double>)(this.GetValue(PixelPerFrameProperty));
            set => this.SetValue(PixelPerFrameProperty, value);
        }

        public static readonly DependencyProperty MaxFrameProperty = DependencyProperty.Register(
"MaxFrame", // プロパティ名を指定
typeof(IReadOnlyReactiveProperty<int>), // プロパティの型を指定
typeof(TickLayer), // プロパティを所有する型を指定
new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる

        public IReadOnlyReactiveProperty<int> MaxFrame
        {
            get => (IReadOnlyReactiveProperty<int>)(this.GetValue(MaxFrameProperty));
            set => this.SetValue(MaxFrameProperty, value);
        }

//        public static readonly DependencyProperty ShowingRangeProperty = DependencyProperty.Register(
//"ShowingRange", // プロパティ名を指定
//typeof(IReadOnlyReactiveProperty<int>), // プロパティの型を指定
//typeof(TickLayer), // プロパティを所有する型を指定
//new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる

//        public IReadOnlyReactiveProperty<(int l, int r)> ShowingRange
//        {
//            get => (IReadOnlyReactiveProperty<(int l, int r)>)(this.GetValue(ShowingRangeProperty));
//            set => this.SetValue(ShowingRangeProperty, value);
//        }

    }
}
