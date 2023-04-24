using CommandProject.Commands;
using Data;
using Reactive.Bindings;
using ScreenProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TimeController;
using Timeline.CustomControl.DragBehaviours;
using Timeline.Error;

namespace Timeline.CustomControl
{

    public class TimelineItemObject : Control , ITimelineItemObject
    {

        static TimelineItemObject()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineItemObject), new FrameworkPropertyMetadata(typeof(TimelineItemObject)));
        }

        public TimelineItemObject(TimelineItem tlItem)
        {
            TimelineItemObjVM = tlItem switch
            {
                VideoItem videoItem => new VideoObjViewModel(videoItem, new VideoScreenObjModel(videoItem)),
                AudioItem audioItem=>new AudioObjViewModel(audioItem, new AudioScreenObjModel(audioItem)),
                ImageItem imageItem=>new ImageObjViewModel(imageItem, new ImageScreenObjModel(imageItem)),
                TextItem textItem=>new TextObjViewModel(textItem, new TextScreenObjModel(textItem)),
                VoiceItem voiceItem=>new VoiceObjViewModel(voiceItem, new VoiceScreenObjModel(voiceItem)),
                _ => throw new Exception($"{tlItem.GetType()}に対応するViewModelの生成方法が定義されていません")
            };





            DataContext = TimelineItemObjVM;

            ItemData = tlItem;
            
            Loaded += OnLoad;

            MouseLeftButtonDown += MouseDownE;
            MouseMove += MouseMoveE;
            MouseLeftButtonUp += MouseUpE;
        }

        public TimelineItemObjectViewModel TimelineItemObjVM { get; }

        private bool loaded = false;

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                return;
            }
            loaded = true;


            PixelPerFrame.Subscribe(_ => AlignByItemData());

            //AlignByItemData();

            CursorTarget = this.GetChildOfType<CursorTarget>();
            if(ItemData.Cloned)
            {
                Pointer.Instance.Add((TimelineItemObjectViewModel)DataContext);
                initPos = (ItemData.Layer, this.PointToScreen(Mouse.GetPosition(this)).X);
                TLObjDataContext.UserControl_MouseDown((PressedPosition.Body, new()), this);
                this.CaptureMouse();
            }
            ItemData.Cloned = false;

            OnApplyTemplate();


        }



        public IReadOnlyReactiveProperty<double> PixelPerFrame => TimelineDataController.Instance.PixelPerFrame;

        private void AlignByItemData()
        {
            TLObjDataContext.Model.MoveLR(ItemData.Frame, ItemData.R, ItemData.Offset);
            TLObjDataContext.ItemName.Value = ItemData.ItemName;
        }

        internal (TimelineLayerObject?, ErrorInfo?) LayerBase
        {
            get
            {
                var tlBase = this.GetParentOfType<TimelineLayerObject>();
                if (tlBase == null)
                {
                    string message = this.IsLoaded ? "" : "Loadされる前に親オブジェクトの取得が行われました";
                    return (null, new ErrorInfo(ErrorCode.LayerForItemIsNotTimelineBaseObject, userMessage: message));
                }
                else
                {
                    return (tlBase, null);
                }
            }
        }


       // private Border ItemObject => (Border)this.GetTemplateChild("TimelineItemBorder");
        private CursorTarget CursorTarget { get; set; }



        public TimelineItem ItemData { get; }
        public TimelineItemObjectViewModel TLObjDataContext => (TimelineItemObjectViewModel)DataContext;

        public Pointer Pointer => Pointer.Instance;




        int ITimelineItemObject.Layer =>TLObjDataContext.Layer.Value;


        #region Dependency Properties


        public int L => TLObjDataContext.Frame.Value;
        public int R => L + TLObjDataContext.Length.Value;
        public int OffsetFrame => TLObjDataContext.OffsetFrame.Value;
        public int Length => TLObjDataContext.Length.Value;


        #endregion





        public const double EdgeThreshold = 7;



        private PressedPosition CheckEdgeDragging()
        {
            double x = Mouse.GetPosition(this).X - L * PixelPerFrame.Value;
            if (x < EdgeThreshold) return PressedPosition.LeftEdge;
            else if (Length*PixelPerFrame.Value - x < EdgeThreshold) return PressedPosition.RightEdge;
            else if (EdgeThreshold <= x && x <= Length * PixelPerFrame.Value - EdgeThreshold) return PressedPosition.Body;
            else return PressedPosition.Outside;
        }

        private readonly ModifierKeys[] allModifs = { ModifierKeys.Control, ModifierKeys.Shift, ModifierKeys.Alt };

        private HashSet<ModifierKeys> CheckModifierKeys()
        {
            var modif = new HashSet<ModifierKeys>();
            foreach (var mod in allModifs)
            {
                if (Keyboard.Modifiers.HasFlag(mod))
                {
                    modif.Add(mod);
                }
            }
            return modif;
        }

        private (int layer,double x) initPos;

        public ICommand Clicked { get; }
        public void MouseDownE(object sender, MouseButtonEventArgs? e = null)
        {
            if (e != null)
            {
                e.Handled = true;
            }
            var modkeys = CheckModifierKeys();
            var isEdgeDragging = CheckEdgeDragging();

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                initPos =(ItemData.Layer, this.PointToScreen(Mouse.GetPosition(this)).X);
                TLObjDataContext.UserControl_MouseDown((isEdgeDragging, modkeys), this);
                this.CaptureMouse();
            }
            else
            {
                this.ReleaseMouseCapture();
            }
        }

        private void MouseMoveE(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {

                (var layerBase, var e1) = LayerBase;
                if (layerBase == null)
                {
                    ErrorHandler.Instance.Add(e1);
                    return;
                }

                var tlBase = layerBase.TLBase;
                TimelineLayerObject tlObjTo = tlBase.GetCursorLayer(e);
                int layer = initPos.layer;

                if (tlObjTo != null)
                {
                    layer = tlBase.TimelineLayerObjects.IndexOf(tlObjTo);
                }

                double currentMouseScreenPosX = this.PointToScreen(Mouse.GetPosition(this)).X;
                int delta = (int)(Math.Round((currentMouseScreenPosX - initPos.x) / PixelPerFrame.Value));


                var finished = TLObjDataContext.UserControl_MouseMove((layer - initPos.layer, delta, CheckModifierKeys()));

                if (finished)
                {
                    TLObjDataContext.UserControl_MouseUp();
                }
            }
        }

        private void MouseUpE(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
                TLObjDataContext.UserControl_MouseUp();
            }

        }



    }
}
