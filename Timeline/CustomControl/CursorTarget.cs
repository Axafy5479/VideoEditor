using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Timeline.Error;

namespace Timeline.CustomControl
{
    public enum PressedPosition
    {
        Outside = -1,
        Body = 0,
        LeftEdge = 1,
        RightEdge = 2
    }

    public class CursorTarget : Border
    {
        


        public CursorTarget()
        {
            //Loaded += _Loaded;
        }

        private bool isLoaded = false;



        public new Subject<(PressedPosition, HashSet<ModifierKeys>)> OnMouseDown { get; } = new();
        public Subject<(int layer, int distance,HashSet<ModifierKeys> modifierKeys)> OnMove { get; } = new();
        public Subject<(PressedPosition pressedPos, int distance,HashSet<ModifierKeys> modifierKeys)> OnScale { get; } = new();
        public new Subject<Unit> OnMouseUp { get; } = new();
        private TimelineItem ItemData { get; set; }



        internal void Initialize(TimelineItem itemData)
        {
            ItemData = itemData;
        }

        //#region Drag
        //private double initMouseScreenPosX;
        //private int initLayer;
        //private PressedPosition pressedPosition;

        //public void UserControl_MouseDown(HashSet<ModifierKeys>? set = null, PressedPosition isEdgeDragging = PressedPosition.Body)
        //{
        //    if (Mouse.LeftButton == MouseButtonState.Pressed)
        //    {
        //        initMouseScreenPosX = this.PointToScreen(Mouse.GetPosition(this)).X;
        //        OnMouseDown.OnNext((isEdgeDragging, set ?? CheckModifierKeys()));
        //        initLayer = ItemData.Layer;
        //        pressedPosition = isEdgeDragging;
        //        this.CaptureMouse();
        //    }
        //    else
        //    {
        //        this.ReleaseMouseCapture();
        //    }
        //}
        //private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (this.IsMouseOver)
        //    {
        //        e.Handled = true;
        //        UserControl_MouseDown(isEdgeDragging:CheckEdgeDragging());
        //    }
        //}

        //private void UserControl_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (this.IsMouseCaptured)
        //    {
                
        //        var tlObjFrom = VisualTreeHelper.GetParent(this) as TimelineItemObject;
        //        if (tlObjFrom == null)
        //        {
        //            ErrorHandler.Instance.Add(ErrorCode.ParentOfCursorTargetIsNotTLItemObject);
        //            return;
        //        }

        //        (var layerBase, var e1) = tlObjFrom.LayerBase;
        //        if (layerBase == null)
        //        {
        //            ErrorHandler.Instance.Add(e1);
        //            return;
        //        }

        //        var tlBase = layerBase.TLBase;
        //        TimelineLayerObject tlObjTo = tlBase.GetCursorLayer(e);
        //        int layer = initLayer;

        //        if (tlObjTo != null)
        //        {
        //            layer = tlBase.TimelineLayerObjects.IndexOf(tlObjTo);
        //        }

        //        double currentMouseScreenPosX = this.PointToScreen(Mouse.GetPosition(this)).X;
        //        int delta = (int)(Math.Round((currentMouseScreenPosX - initMouseScreenPosX) / PixelPerFrame));

        //        //Debug.WriteLine(currentMouseScreenPosX +"," + initMouseScreenPosX +","+ PixelPerFrame);
        //        OnMove.OnNext((layer - initLayer, delta, CheckModifierKeys()));

        //    }
        //}

        //private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    if(this.IsMouseCaptured)
        //    {
        //        this.ReleaseMouseCapture();
        //        OnMouseUp.OnNext(Unit.Default);
        //    }

        //}

        //#endregion
    }
}
