using CommandProject.Commands;
using ControlzEx.Standard;
using Data;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TimeController;
using Timeline.Commands;
using Timeline.CustomControl;
using Timeline.Error;

namespace Timeline
{
    public class TimelineBase : WrapPanel
    {
        public TimelineBase()
        {
            //var pointer = new Pointer();
            //Pointer = new(pointer);
            

            TimelineDataController.Instance.OnAdded.Subscribe(OnItemAdded);
            TimelineDataController.Instance.OnRemoved.Subscribe(OnItemRemoved);
            TimelineDataController.Instance.AfterAnyChanged.Subscribe(AfterAnyChanged);
            TimelineDataController.Instance.CurrentFrame.Subscribe(OnCursorFrameChanged);

            SizeChanged += OnSizeChanged;

            Loaded += (s,e)=>Initialize();
        }


        private int CurrentFrame => TimelineDataController.Instance.CurrentFrame.Value;
        private double PixelPerFrame => TimelineDataController.Instance.PixelPerFrame.Value;

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var layer in TimelineLayerObjects)
            {
                foreach (var item in layer.TLLayerDataContext.Items)
                {
                    item.TLObjDataContext.Model.OnTLSizeChanged();
                }
                
            }
        }

        private void AfterAnyChanged(Unit obj)
        {
            //TimeCtrl.Instance.Update(CurrentFrame, GetOnCursorItemObjs(CurrentFrame), CursorMover.IsRunning);
        }

        private Dictionary<TimelineItemObject, List<IDisposable>> DisposablesMap { get; } = new();
        private void OnItemAdded(TimelineItem newItem)
        {
            TimelineItemObject newTLObj = new(newItem);
            ItemObjects.Add(newTLObj);
            TimelineDataController.Instance.TLItemObjVMs.Add(newTLObj.TimelineItemObjVM);
            //TimelineLayerObjects[newItem.Layer].Children.Add(newTLObj);
            TimelineLayerObjects[newItem.Layer].TLLayerDataContext.Items.Add(newTLObj);

            var disposable = newTLObj.TLObjDataContext.Layer.Subscribe(l =>
            {
                foreach (var layer in TimelineLayerObjects)
                {
                    if(layer.TLLayerDataContext.Items.Contains(newTLObj))
                    {
                        layer.TLLayerDataContext.Items.Remove(newTLObj);
                        break;
                    }
                }

                TimelineLayerObjects[l].TLLayerDataContext.Items.Add(newTLObj);
            });

            if (DisposablesMap.ContainsKey(newTLObj))
            {
                DisposablesMap[newTLObj].Add(disposable);
            }
            else
            {
                DisposablesMap.Add(newTLObj, new List<IDisposable>() { disposable});
            }


        }

        public List<TimelineItemObject> ItemObjects { get; } = new();
        public List<TimelineLayerObject> TimelineLayerObjects
        {
            get
            {
                List<TimelineLayerObject> timelineBaseObjects = new();

                ItemsControl itemsCtrl = ((ItemsControl)this.Children[0]);
                for (int i = 0; i < itemsCtrl.Items.Count; i++)
                {
                    ContentPresenter c = (ContentPresenter)itemsCtrl.ItemContainerGenerator.ContainerFromIndex(i);
                    var c1 = VisualTreeHelper.GetChild(c, 0);
                    timelineBaseObjects.Add(c1 as TimelineLayerObject);
                }
                return timelineBaseObjects;
            }
        }
        public TimelineBaseViewModel TLDataContext => (TimelineBaseViewModel)DataContext;
        public double ScrollMovingThreshold = 0.05;


        public HashSet<ITimelineItemObject> GetOnCursorItemObjs(int frame)
        {
            var temp = ItemObjects.FindAll(d => d.L <= frame && d.R > frame);
            return new(temp);
        }







        public void InsertTLBaseObject(int layer)
        {
            CommandInvoker.Instance.Execute(new Command_InsertLayer(this, layer));
        }

        private void OnItemRemoved(TimelineItem target)
        {
            int layer = target.Layer;

            var tlItemObject = TimelineLayerObjects[layer].TLLayerDataContext.Items.Find(o => o.ItemData == target);


            TimelineLayerObjects[layer].TLLayerDataContext.Items.Remove(tlItemObject);
            ItemObjects.Remove(tlItemObject);
            TimelineDataController.Instance.TLItemObjVMs.Remove(tlItemObject.TimelineItemObjVM);
            DisposablesMap.Remove(tlItemObject);

        }


        private void OnCursorFrameChanged(int CursorFrame)
        {
            if (!IsLoaded) return;

            (int l, int r) = ShowingArea();
            if ((r - CursorFrame) < ScrollMovingThreshold * (r - l))
            {
                if (!TimelineDataController.Instance.Playing.Value)
                {
                    int frameThreshold = (int)(r - (r - l) * ScrollMovingThreshold);

                    int target = (frameThreshold + CursorFrame) / 2;
                    int newFrameL = (int)(target - (r - l) * (1 - ScrollMovingThreshold));
                    MainScrollViewer.ScrollToHorizontalOffset(newFrameL * PixelPerFrame);
                }
                else
                {
                    int newFrameL = (int)(CursorFrame - (r - l) * ScrollMovingThreshold);
                    MainScrollViewer.ScrollToHorizontalOffset(newFrameL * PixelPerFrame);
                }
            }
            else if ((CursorFrame - l) < ScrollMovingThreshold * (r - l))
            {
                if (!TimelineDataController.Instance.Playing.Value)
                {
                    int frameThreshold = (int)(l + (r - l) * ScrollMovingThreshold);

                    int target = (frameThreshold + CursorFrame) / 2;
                    int newFrameL = (int)(target - (r - l) * ScrollMovingThreshold);
                    MainScrollViewer.ScrollToHorizontalOffset(newFrameL * PixelPerFrame);
                }
            }



        }

        public (int l, int r) ShowingArea()
        {
            double pixelL = MainScrollViewer.HorizontalOffset;
            double pixelR = pixelL + MainScrollViewer.ActualWidth;
            int L = (int)(Math.Round(pixelL / PixelPerFrame));
            int R = (int)(Math.Round(pixelR / PixelPerFrame));

            return (L, R);
        }


        private bool isLoaded = false;
        private void Initialize()
        {
            Pointer Pointer = Pointer.Instance;

            if (isLoaded)
            {
                return;
            }
            isLoaded = true;

     

            TLDataContext.AddCommand(KeyboardListener);
            MainScrollViewer.ScrollChanged += OnScrollChanged;

        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            int l = (int)(Math.Round(e.VerticalOffset / PixelPerFrame));
            int r = l + (int)(Math.Round(MainScrollViewer.Width / PixelPerFrame));
            ShowingFrameArea.Value = (l, r);
        }

        public TimelineLayerObject GetTLBaseObject(int layer)=> TimelineLayerObjects[layer];

        public TimelineLayerObject? GetCursorLayer(MouseEventArgs e)
        {
            _hitResults.Clear();

            Point position = e.GetPosition(this);
            VisualTreeHelper.HitTest(this, null
                , new HitTestResultCallback(OnHitTestResultCallback)
                , new PointHitTestParameters(position));

            return _hitResults.Find(h => h is TimelineLayerObject) as TimelineLayerObject;
        }
        public TimelineLayerObject? GetCursorLayer(DragEventArgs e)
        {
            _hitResults.Clear();

            Point position = e.GetPosition(this);
            VisualTreeHelper.HitTest(this, null
                , new HitTestResultCallback(OnHitTestResultCallback)
                , new PointHitTestParameters(position));

            return _hitResults.Find(h => h is TimelineLayerObject) as TimelineLayerObject;
        }
        public TimelineLayerObject? GetCursorLayer(Point position)
        {
            _hitResults.Clear();

            VisualTreeHelper.HitTest(this, null
                , new HitTestResultCallback(OnHitTestResultCallback)
                , new PointHitTestParameters(position));

            return _hitResults.Find(h => h is TimelineLayerObject) as TimelineLayerObject;
        }


        private readonly List<DependencyObject> _hitResults = new List<DependencyObject>();
        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            _hitResults.Add(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }

        public ReactiveProperty<(int l, int r)> ShowingFrameArea { get; set; } = new();


        #region Dependency Properties
        public static readonly DependencyProperty MainDataContextProperty = DependencyProperty.Register(
            "MainDataContext", // プロパティ名を指定
            typeof(TimelineLayoutViewModel), // プロパティの型を指定
            typeof(TimelineBase), // プロパティを所有する型を指定
            new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる

        public TimelineLayoutViewModel MainDataContext
        {
            get => (TimelineLayoutViewModel)(this.GetValue(MainDataContextProperty));
            set => this.SetValue(MainDataContextProperty, value);
        }

        private static readonly DependencyProperty KeyboardListenerProperty = DependencyProperty.Register(
"KeyboardListener", // プロパティ名を指定
typeof(TimelineKeyboard), // プロパティの型を指定
typeof(TimelineBase), // プロパティを所有する型を指定
new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる
        public TimelineKeyboard KeyboardListener
        {
            get => (TimelineKeyboard)(this.GetValue(KeyboardListenerProperty));
            set => this.SetValue(KeyboardListenerProperty, value);
        }

        private static readonly DependencyProperty MainScrollViewerProperty = DependencyProperty.Register(
"MainScrollViewer", // プロパティ名を指定
typeof(ScrollViewer), // プロパティの型を指定
typeof(TimelineBase), // プロパティを所有する型を指定
new PropertyMetadata(null)); // メタデータを指定。ここではデフォルト値を設定してる
        public ScrollViewer MainScrollViewer
        {
            get => (ScrollViewer)(this.GetValue(MainScrollViewerProperty));
            set => this.SetValue(MainScrollViewerProperty, value);
        }
        #endregion
    }


}
