using Data;
using Microsoft.VisualBasic.Devices;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TimeController;
using Timeline;
using Timeline.Error;

namespace Data
{
    public class Pointer : BindableBase, IReadOnlyPointer
    {
        private static Pointer? instance;
        public static Pointer Instance=>instance??=new();
        private CursorMover cursorMover;

        private Pointer()
        {
            cursorMover = new();

            SelectedVMs.CollectionChanged += (o, e) =>
            {
                SelectedItem.Value = new(SelectedVMs.ToList());
            };

            PixelPerFrame = TimelineDataController.Instance.PixelPerFrame;
            CursorFrame = TimelineDataController.Instance.CurrentFrame;
        }

        public ReactiveCollection<ITimelineObjectViewModel> SelectedVMs { get; } = new();
        public ReactiveProperty<List<ITimelineObjectViewModel>> SelectedItem { get; } = new();

        private ReactiveProperty<(int layer, int l, int r)> _SelectedBlank = new((-1, -1, -1));
        public IReadOnlyReactiveProperty<(int layer, int l, int r)> SelectedBlank => _SelectedBlank;
        public List<byte[]> CopiedItemData { get; } = new();


        public ErrorInfo? Copy()
        {
            if(!SelectedVMs.Any())
            {
                return null;
            }
            CopiedItemData.Clear();
            foreach (var item in SelectedVMs)
            {
                (var serializedData, var e) = item.ItemData.Serialize();
                if(serializedData == null)
                {
                    
                    CopiedItemData.Clear();
                    return e;
                }
                CopiedItemData.Add(serializedData);
            }
            return null;
        }

        public void AddSelectedBlank(int layer, int l, int r)
        {
            Clear();
            _SelectedBlank.Value = (layer,l,r);
        }

        public void Add(ITimelineObjectViewModel tlObj)
        {
            if (SelectedVMs.Contains(tlObj)) return;
            SelectedVMs.Add(tlObj);
            _SelectedBlank.Value = (-1, -1, -1);
        }

        public void ClearAndAdd(ITimelineObjectViewModel tlObj)
        {
            SelectedVMs.Clear();
            Add(tlObj);
            _SelectedBlank.Value = (-1, -1, -1);
        }

        public void Clear()
        {
            SelectedVMs.Clear();
            _SelectedBlank.Value = (-1, -1, -1);
        }

        public IReadOnlyReactiveProperty<int> CursorFrame { get; }
        public IReadOnlyReactiveProperty<double> PixelPerFrame { get; }



    }
}
