using Data;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ScreenProject
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditorScreen : UserControl
    {
        public EditorScreen()
        {
            InitializeComponent();
            Loaded += Initialize;
        }

        private EditorScreenViewModel ScreenData => (EditorScreenViewModel)DataContext;
        private Canvas RootGrid { get; set; }

        private bool loaded = false;
        private void Initialize(object sender, RoutedEventArgs e)
        {
            if (loaded) return;
            loaded = true;

            RootGrid = (Canvas)this.FindName("ScreenRootGrid");

            ScreenData.PixelWidth.Subscribe(_ => UpdateRatio());
            ScreenData.PixelHeight.Subscribe(_ => UpdateRatio());
            SizeChanged += (s, e) => UpdateRatio();

        }


        private void UpdateRatio()
        {
            double ratio = Math.Min(ActualHeight / ScreenData.PixelHeight.Value, ActualWidth / ScreenData.PixelWidth.Value);

            ScreenData.CenterX.Value = RootGrid.ActualWidth / 2;
            ScreenData.CenterY.Value = RootGrid.ActualHeight / 2;

            ScreenData.ScaleX.Value = ratio;
            ScreenData.ScaleY.Value = ratio;
        }

    }
}
