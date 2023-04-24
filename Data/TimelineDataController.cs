using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Standard;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TimeController;
using Timeline;
using Timeline.Error;

namespace Data
{
    public class TimelineDataController
    {
        private static TimelineDataController? instance;
        public static TimelineDataController Instance => instance ??= new();

        private TimelineDataController()
        {
            pixelPerFrame = DataCore.ToReactivePropertyAsSynchronized(x => x.PixelPerFrame);
            currentFrame = DataCore.ToReactivePropertyAsSynchronized(x => x.CurrentFrame);
            CurrentLayer = DataCore.ToReactivePropertyAsSynchronized(x => x.CurrentLayer);
            Length = DataCore.ToReactivePropertyAsSynchronized(x => x.Length);
            MaxLayer = DataCore.ToReactivePropertyAsSynchronized(x => x.MaxLayer);
            PixelWidth = DataCore.ObserveProperty(x=>x.PixelWidth).ToReactiveProperty();
            PixelHeight = DataCore.ObserveProperty(x=>x.PixelHeight).ToReactiveProperty();
        }

        private ReactiveProperty<double> pixelPerFrame { get; }
        public IReadOnlyReactiveProperty<double> PixelPerFrame => pixelPerFrame;

        private ReactiveProperty<(int frame, HashSet<ITimelineObjectViewModel>? items, bool isPlaying)> _ShowingItems { get; } = new();
        public IReadOnlyReactiveProperty<(int frame, HashSet<ITimelineObjectViewModel>? items, bool isPlaying)> ShowingItems => _ShowingItems;

        /// <summary>
        /// 全てのタイムラインオブジェクトのVM
        /// </summary>
        public HashSet<ITimelineObjectViewModel> TLItemObjVMs { get; } = new();
        public HashSet<ITimelineObjectViewModel> GetItemObjMVsOnCursor(int frame)
        {
            return new(TLItemObjVMs.FindAll(d => d.Frame.Value <= frame && d.R > frame));
        }

        public void ChangePixelPerFrame(double value)
        {
            pixelPerFrame.Value = value;
            AfterAnyChange();
        }

        private ReactiveProperty<int> currentFrame { get; }
        public IReadOnlyReactiveProperty<int> CurrentFrame => currentFrame;
        public IReadOnlyReactiveProperty<int> PixelWidth { get; }
        public IReadOnlyReactiveProperty<int> PixelHeight { get; }

        public void ChangeCurrentFrame(int frame, bool isPlaying)
        {
            currentFrame.Value = frame;
            var itemObjMVOnCursor = GetItemObjMVsOnCursor(frame);
            _ShowingItems.Value = (frame, itemObjMVOnCursor, isPlaying);

        }

        private void UpdateCurrentFrame()
        {
            var itemObjMVOnCursor = GetItemObjMVsOnCursor(currentFrame.Value);
            _ShowingItems.Value = (currentFrame.Value, itemObjMVOnCursor, false);
        }

        public ReactiveProperty<int> CurrentLayer { get; }
        public ReactiveProperty<int> Length { get; }
        public ReactiveProperty<int> MaxLayer { get; }

        public (int l, int? r)? GetBlank(int layer, int center)
        {
            List<(int l, int r)> usedAreas = new();

            foreach (var item in DataCore.Items)
            {
                if (item.Layer == layer)
                {
                    usedAreas.Add((item.Frame, item.R));
                }
            }

            usedAreas.Sort((a, b) => a.l - b.l);

            int prevR = 0;
            for (int i = 0; i < usedAreas.Count; i++)
            {
                int l = usedAreas[i].l;
                int r = usedAreas[i].r;

                if (prevR <= center && center < l)
                {
                    return (prevR, l);
                }

                if (l <= center && center < r)
                {
                    return null;
                }

                prevR = r;
            }

            return (prevR, null);
        }

        public (TimelineItem?, ErrorInfo?) FindItemByPosition(int layer, int l, int r)
        {
            var ans = DataCore.Items.Find(d => d.Layer == layer && d.Frame == l && d.Length == r - l);
            if (ans == null)
            {
                return (null, new ErrorInfo(ErrorCode.TimelineItemNotFoundAtSpecificPosition, userMessage: $"layer={layer}, l={l}, r={r}"));
            }
            else
            {
                return (ans, null);
            }
        }

        public HashSet<TimelineItem> GetItemsOnFrame(int frame)
        {
            return new(DataCore.Items.FindAll(d => d.Frame <= frame && d.R > frame));
        }

        public List<TimelineItem> GetTimelineAtFrame(int l, int r)
        {
            return DataCore.Items.FindAll(d => d.Frame <= r || d.R > l);
        }


        public Subject<TimelineItem> OnAdded { get; } = new();
        public Subject<TimelineItem> OnChanged { get; } = new();
        public Subject<TimelineItem> OnRemoved { get; } = new();
        public Subject<Unit> AfterAnyChanged { get; } = new();

        public ErrorInfo? RemoveItem(int layer, int frame, int length)
        {
            var target = DataCore.Items.Find(item => item.Layer == layer && item.Frame == frame && item.Length == length);
            if (target != null)
            {
                DataCore.Items.Remove(target);
                OnRemoved.OnNext(target);
                AfterAnyChange();
                return null;
            }
            else
            {
                return new(ErrorCode.RemovingItemNotFound);
            }

        }

        public void AddItem(TimelineItem newItem)
        {
            DataCore.Items.Add(newItem);
            OnAdded.OnNext(newItem);
            AfterAnyChange();
        }

        public void TLObjectMoved(object obj)
        {
            AfterAnyChange();
        }

        public void Changed(TimelineItem newItem)
        {
            OnChanged.OnNext(newItem);
            AfterAnyChange();
        }

        private void AfterAnyChange()
        {
            Length.Value = Items.ConvertAll(d => d.R).DefaultIfEmpty().Max();
            UpdateCurrentFrame();
            AfterAnyChanged.OnNext(Unit.Default);
        }


        public bool CanUseFrameRange(int layer, int l, int r, HashSet<TimelineItem>? excepts)
        {
            excepts ??= new();

            if (l < 0)
            {
                return false;
            }

            bool ans = true;

            List<TimelineItem> tlItems = DataCore.Items.FindAll(item => item.Layer == layer);

            foreach (var item in tlItems)
            {
                if (item.Frame < r && item.R > l && !excepts.Contains(item))
                {
                    ans = false;
                    break;
                }
            }

            return ans;
        }

        public ReadOnlyCollection<TimelineItem> Items => DataCore.Items.AsReadOnly();

        public List<KeyCharacterBind> KeyCharacterBinds => DataCore.KeyCharacterBinds;

        private TimelineDataCore DataCore { get; } = new();



        public ReactiveProperty<bool> Playing { get; } = new();
    }
}
