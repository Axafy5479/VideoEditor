using CommandProject;
using CommandProject.Commands;
using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timeline.Commands;
using Timeline.CustomControl;
using Timeline.Error;

namespace Timeline
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

            SizeChanged += OnSizeChanged;

        }

        private TimelineBase? tlBase;
        private TimelineBase TLBase
        {
            get
            {
                if (tlBase == null)
                {
                    TimelineBase? temp = this.GetChildOfType<TimelineBase>();
                    if(temp == null)
                    {
                        ErrorHandler.Instance.Add(ErrorCode.UnknownError);
                        throw new Exception("子オブジェクトにTimelineBaseが見つかりません");
                    }
                    tlBase = temp;
                }
                return tlBase;
            }
        }


        public static readonly DependencyProperty Color0Property =
            DependencyProperty.Register("Color0", typeof(Brush), typeof(UserControl1));
        public Brush Color0
        {
            get
            {
                return (Brush)this.GetValue(Color0Property) ;
            }
            set
            {
                this.SetValue(Color0Property, value);
            }
        }



        public static readonly DependencyProperty Color1Property =
            DependencyProperty.Register("Color1", typeof(Brush), typeof(UserControl1));
        public Brush Color1
        {
            get
            {
                return (Brush)this.GetValue(Color1Property);
            }
            set
            {
                this.SetValue(Color1Property, value);
            }
        }



        private void SchedulerScrolViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LayerLabelsLayoutScroll.ScrollToVerticalOffset(e.VerticalOffset);
            TimeTicksScroll.ScrollToHorizontalOffset(e.HorizontalOffset);
        }


        private void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            ((TimelineLayoutViewModel)DataContext).RightClickScreenPoint = this.PointToScreen(Mouse.GetPosition(this));
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TLWindowWidth.Value = this.ActualWidth;
        }

        public ReactiveProperty<double> TLWindowWidth { get; set; } = new();

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var name in fileNames)
                {
                    int frame = TimelineDataController.Instance.CurrentFrame.Value;
                    int layer = Pointer.Instance.SelectedBlank.Value.layer;
                    if(layer<0)
                    {
                        layer = TLBase.GetCursorLayer(e)?.Layer ?? 0;
                    }

                    (var item, var e2) = TimelineItem.CrerateItemFromFile(name, layer, frame);
                    if(item is null)
                    {
                        ErrorHandler.Instance.Add(e2);
                        return;
                    }

                    CommandInvoker.Instance.Execute(new Command_AddItem(new() { item }));
                }
            }
        }


        public static readonly DependencyProperty TLWidthProperty =
    DependencyProperty.Register("TLWidth", typeof(double), typeof(UserControl1), new PropertyMetadata(1000d));
        public double TLWidth
        {
            get
            {
                return (double)this.GetValue(TLWidthProperty);
            }
            set
            {
                this.SetValue(TLWidthProperty, value);
            }
        }

        private void ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            CommandProject.CommandManager.Instance.ContextMenuOpening();
        }
    }


}