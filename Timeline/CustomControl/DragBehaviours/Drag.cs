using CommandProject.Commands;
using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeController;
using Timeline.Commands;
using Timeline.Error;
using static System.Formats.Asn1.AsnWriter;

namespace Timeline.CustomControl.DragBehaviours
{
    internal abstract class DragBase
    {
        public const double AttractingPixel = 10;

        internal DragBase(TimelineItemObjectModel mainTarget, Pointer pointer)
        {
            MainTarget = mainTarget;
            Pointer = pointer;
            InitialFrameMap = pointer.SelectedVMs.ToDictionary(o => o, o => (o.Layer.Value, o.Frame.Value));
            MovingItems = new(Pointer.SelectedVMs.ToList().ConvertAll(d => d.ItemData));
        }

        public bool IsChanged { get; private set; } = false;
        public TimelineItemObjectModel MainTarget { get; }
        public Pointer Pointer { get; }
        protected int Diff { get; private set; }
        private int Prev { get; set; } = 0;
        protected HashSet<TimelineItem> MovingItems { get; }
        protected Dictionary<ITimelineObjectViewModel, (int layer,int l)> InitialFrameMap { get; }

        internal bool OnDrag((int layer, int distanceX, HashSet<ModifierKeys> modifierKeys) dragInfo)
        {
            Diff = dragInfo.distanceX - Prev;
            Prev = dragInfo.distanceX;
            bool ans = _OnDrag(dragInfo);
            return ans;
        }

        protected abstract bool _OnDrag((int distanceLayer, int distanceX, HashSet<ModifierKeys> modifierKeys) dragInfo);


        public (int frame, bool right)? CheckAttracted(bool moveRight, bool moveLeft)
        {
            TimelineItemObjectModel target = MainTarget;

            int score = (int)(AttractingPixel / Pointer.PixelPerFrame.Value);
            int closestFrame = int.MaxValue;
            bool right = false;

            if (score > Math.Abs(target.Frame - Pointer.CursorFrame.Value) && moveLeft)
            {
                score = Math.Abs(target.Frame - Pointer.CursorFrame.Value);
                closestFrame = Pointer.CursorFrame.Value;
                right = false;
            }

            if (score > Math.Abs(target.R - Pointer.CursorFrame.Value) && moveRight)
            {
                score = Math.Abs(target.R - Pointer.CursorFrame.Value);
                closestFrame = Pointer.CursorFrame.Value;
                right = true;
            }

            foreach (var item in TimelineDataController.Instance.Items)
            {
                if (MovingItems.Contains(item)) continue;




                if (score > Math.Abs(target.Frame - item.Frame) && moveLeft)
                {
                    score = Math.Abs(target.Frame - item.Frame);
                    closestFrame = item.Frame;
                    right = false;
                }

                if (score > Math.Abs(target.Frame - item.R) && moveLeft)
                {
                    score = Math.Abs(target.Frame - item.R);
                    closestFrame = item.R;
                    right = false;
                }

                score = Math.Min(score, Math.Abs(target.R - closestFrame));



                if (score > Math.Abs(target.R - item.Frame) && moveRight)
                {
                    score = Math.Abs(target.R - item.Frame);
                    closestFrame = item.Frame;
                    right = true;
                }

                if (score > Math.Abs(target.R - item.R) && moveRight)
                {
                    score = Math.Abs(target.R - item.R);
                    closestFrame = item.R;
                    right = true;
                }

            }

            return closestFrame == int.MaxValue ? null : (closestFrame,right);
        }

        internal void Move(Dictionary<ITimelineObjectViewModel, (int layer, int l)> target)
        {
            foreach (var item in target)
            {
                var obj = item.Key;
                var l = item.Value.l;

                ((TimelineItemObjectViewModel)obj).Model.Move(l);
                ((TimelineItemObjectViewModel)obj).Layer.Value = item.Value.layer;
            }

        }

        internal void Scale(Dictionary<ITimelineObjectViewModel, (int l, int r, int offset)> target)
        {
            foreach (var item in target)
            {
                var obj = item.Key;
                var l = item.Value.l;
                var r = item.Value.r;
                var offset = item.Value.offset;

                ((TimelineItemObjectViewModel)obj).Model.MoveLR(l, r, offset);
            }
        }

        internal ErrorInfo? CreateClone(Dictionary<ITimelineObjectViewModel, (int layer, int l)> target)
        {
            try
            {
                Pointer.Clear();

                List<TimelineItem> newTLItems = new();
                foreach (var item in target)
                {
                    var obj = item.Key;
                    var l = item.Value.l;
                    var layer = item.Value.layer;

                    (var clonedItem, var e) = obj.ItemData.CreateClone(layer, l);

                    if (clonedItem != null && e == null)
                    {
                        newTLItems.Add(clonedItem);
                    }
                    else
                    {
                        return e;
                    }
                }

                CommandInvoker.Instance.Execute(new Command_AddItem(newTLItems));
                return null;
            }
            catch (Exception e)
            {
                return new(ErrorCode.UnknownError, e);
            }
        }
    }

    internal class NormalDrag : DragBase
    {
        public NormalDrag(TimelineItemObjectModel mainTarget, Pointer pointer) : base(mainTarget,pointer)
        {
        }

        protected override bool _OnDrag((int distanceLayer, int distanceX, HashSet<ModifierKeys> modifierKeys) dragInfo)
        {
            var target = TryMove(dragInfo);

            if (target != null)
            {
                Move(target);
            }

            var attracting = CheckAttracted(true, true);
            if (attracting is ValueTuple<int, bool> itemInfo)
            {
                var targetFrame = itemInfo.Item1;
                var right = itemInfo.Item2;

                int distanceX = right ?
                    targetFrame - MainTarget.ItemData.Frame - MainTarget.ItemData.Length :
                    targetFrame - MainTarget.ItemData.Frame;

                var attractedTarget = TryMove((dragInfo.distanceLayer, distanceX, dragInfo.modifierKeys));
                if (attractedTarget != null)
                {
                    Move(attractedTarget);
                }
            }


            return true;
        }

        public Dictionary<ITimelineObjectViewModel, (int layer, int l)>? TryMove(
            (int distanceLayer, int distanceX, HashSet<ModifierKeys> modifierKeys) dragInfo)
        {
            HashSet<TimelineItem> set = new(InitialFrameMap.Keys.ToList().ConvertAll(x => x.ItemData));
            Dictionary<ITimelineObjectViewModel, (int layer, int l)> target = new();

            bool canMove = true;
            foreach (var item in InitialFrameMap)
            {
                var obj = item.Key;
                var l = item.Value.l + dragInfo.distanceX;
                var layer = item.Value.layer + dragInfo.distanceLayer;
                var r = l + obj.Length.Value;

                if (layer < 0)
                {
                    canMove = false;
                    break;
                }

                canMove = TimelineDataController.Instance.CanUseFrameRange(layer, l, r, set);
                if (!canMove)
                {
                    break;
                }

                target.Add(item.Key, (layer, l));
            }

            return canMove ? target : null;
        }

    }

    internal class ScaleDrag : DragBase
    {
        public ScaleDrag(TimelineItemObjectModel mainTarget, Pointer pointer, bool isRight) : base(mainTarget,pointer)
        {
            IsRight = isRight;
        }

        public bool IsRight { get; }

        protected override bool _OnDrag((int distanceLayer, int distanceX, HashSet<ModifierKeys> modifierKeys) dragInfo)
        {
            var target = TryScale(dragInfo);

            if (target is not null)
            {
                Scale(target);
            }

            var attracting = CheckAttracted(IsRight, !IsRight);
            if (attracting is ValueTuple<int, bool> itemInfo)
            {
                if (itemInfo.Item2 == IsRight)
                {

                    var targetFrame = itemInfo.Item1;
                    var right = itemInfo.Item2;

                    int distanceX = right ?
                        targetFrame - MainTarget.ItemData.Frame - MainTarget.ItemData.Length :
                        targetFrame - MainTarget.ItemData.Frame;
                    var attractedTarget = TryScale((dragInfo.distanceLayer, distanceX, dragInfo.modifierKeys));
                    if (attractedTarget != null)
                    {
                        Scale(attractedTarget);
                    }
                }
            }

            return true;
        }

        public Dictionary<ITimelineObjectViewModel, (int layer, int l, int offset)>? TryScale(
            (int distanceLayer, int distanceX, HashSet<ModifierKeys> modifierKeys) dragInfo)
        {
            Dictionary<ITimelineObjectViewModel, (int l, int r, int offset)> target_edge_map = new();
            HashSet<TimelineItem> tagetItemSet = new(InitialFrameMap.Keys.ToList().ConvertAll(x => x.ItemData));
            bool canMove = true;
            foreach (var item in InitialFrameMap)
            {
                var tlItemObj = item.Key;
                var layer = item.Value.layer;
                var r = tlItemObj.ItemData.R;
                var l = tlItemObj.ItemData.Frame;
                var offset = tlItemObj.ItemData.Offset;

                if (IsRight)
                {
                    var temp_r = r + dragInfo.distanceX;
                    if (temp_r <= l)
                    {
                        canMove = false;
                        break;
                    }
                    r = temp_r;
                }
                else
                {
                    var temp_l = l + dragInfo.distanceX;
                    if (temp_l >= r || temp_l < 0)
                    {
                        canMove = false;
                        break;
                    }
                    l = temp_l;
                    offset += dragInfo.distanceX;
                }

                canMove = TimelineDataController.Instance.CanUseFrameRange(layer, l, r, new() { tlItemObj.ItemData });
                if (!canMove)
                {
                    break;
                }

                target_edge_map.Add(item.Key, (l, r, offset));
            }

            return canMove ? target_edge_map : null;
        }

    }

    internal class AltDrag : DragBase
    {
        public AltDrag(TimelineItemObjectModel mainTarget, Pointer pointer) : base(mainTarget, pointer)
        {
        }

        protected override bool _OnDrag((int distanceLayer, int distanceX, HashSet<ModifierKeys> modifierKeys) dragInfo)
        {
            if(!dragInfo.modifierKeys.Contains(ModifierKeys.Alt))
            {
                return false;
            }

            Dictionary<ITimelineObjectViewModel, (int layer,int l)> target = new();
            HashSet<TimelineItem> set = new();
            bool canMove = true;
            foreach (var item in InitialFrameMap)
            {
                var obj = item.Key;
                var l = item.Value.l + dragInfo.distanceX;
                var layer = item.Value.layer + dragInfo.distanceLayer;
                var r = l + obj.Length.Value;

                canMove = TimelineDataController.Instance.CanUseFrameRange(layer, l, r, set);
                if (!canMove)
                {
                    break;
                }

                target.Add(item.Key, (layer, l));
            }

            if (canMove)
            {
                CreateClone(target);
                return false;
            }

            return true;
        }

    }
}
