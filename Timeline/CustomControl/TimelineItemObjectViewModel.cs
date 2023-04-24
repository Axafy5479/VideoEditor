using CommandProject.Commands;
using Data;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using TimeController;
using Timeline.CustomControl.DragBehaviours;
using Timeline.Error;

namespace Timeline.CustomControl
{
    public abstract class TimelineItemObjectViewModel : IDisposable, ITimelineObjectViewModel
    {
        public TimelineItemObjectViewModel(TimelineItem tlItem, TimelineItemObjectModel model)
        {
            Model = model;

            ItemName = Model.ToReactivePropertyAsSynchronized(x => x.ItemName).AddTo(Disposable);
            Layer = Model.ToReactivePropertyAsSynchronized(x => x.Layer).AddTo(Disposable);
            Frame = Model.ToReactivePropertyAsSynchronized(x => x.Frame).AddTo(Disposable);
            Length = Model.ToReactivePropertyAsSynchronized(x => x.Length).AddTo(Disposable);
            OffsetFrame = Model.ToReactivePropertyAsSynchronized(x => x.OffsetFrame).AddTo(Disposable);


            Pointer.SelectedVMs.CollectionChanged += (o, e) =>
            {
                bool isSelected = Pointer.SelectedVMs.Contains(this);
                Alpha.Value = isSelected ? 0.6 : 1;
                Thickness.Value = isSelected ? 2 : 0;
                Radius.Value = isSelected ? 4 : 5;
            };

            //// 色の変更
            //var source = (Brush)ColorManager.Instance
            //TimelineItemObjVM.BGColor.Value = source;

            BGColor = ColorManager.Instance.TLItemTheme[tlItem.FileType];


        }

        public TimelineItemObjectModel Model { get; }
        public TimelineItem ItemData => Model.ItemData;


        private CompositeDisposable Disposable { get; } = new CompositeDisposable();


        #region Properties
        public ReactiveProperty<string> ItemName { get; }
        public ReactiveProperty<int> Layer { get; }
        public ReactiveProperty<int> Frame { get; }
        public ReactiveProperty<int> Length { get; }
        public ReactiveProperty<int> OffsetFrame { get; }
        #endregion

        TimelineItem IItemObjectViewModel.ItemData => Model.ItemData;

        public int R => Frame.Value + Length.Value;

        public void Dispose()
        {
            this.Disposable.Dispose();
        }




        public Pointer Pointer => Pointer.Instance;




        #region Drag
        private DragBase Drag { get; set; }

        public void UserControl_MouseDown((PressedPosition pressedPos, HashSet<ModifierKeys> modifierKeys) info, ITimelineItemObject itemObj)
        {
            (var pressedPos, var modifierKeys) = info;
            if (modifierKeys.Contains(ModifierKeys.Control))
            {
                Pointer.Add(this);
            }
            else
            {
                Pointer.ClearAndAdd(this);
            }

            if (pressedPos == PressedPosition.LeftEdge)
            {
                Drag = new ScaleDrag(Model, Pointer, false);
            }
            else if (pressedPos == PressedPosition.RightEdge)
            {
                Drag = new ScaleDrag(Model, Pointer, true);
            }
            else
            {
                if (modifierKeys.Contains(ModifierKeys.Alt))
                {
                    Drag = new AltDrag(Model, Pointer);
                }
                else
                {
                    Drag = new NormalDrag(Model, Pointer);
                }
            }
        }


        public bool UserControl_MouseMove((int layer, int distance, HashSet<ModifierKeys> modifierKeys) dragInfo)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                return true;
            }
            bool finished = !Drag.OnDrag(dragInfo);
            if (finished) {
                Drag = new NormalDrag(Model, Pointer);
            }

            return finished;

        }


        public void UserControl_MouseUp()
        {
            if (Pointer.SelectedVMs.Any())
            {
                int newLayer = Layer.Value;
                int newFrame = Frame.Value;
                int newLength = Length.Value;

                int oldLayer = Model.ItemData.Layer;
                int oldFrame = Model.ItemData.Frame;
                int oldLength = Model.ItemData.Length;

                bool different = oldLayer != newLayer || oldFrame != newFrame || oldLength != newLength;

                if (different)
                {
                    CommandInvoker.Instance.Execute(new Command_Move());
                }
            }
        }


        #endregion


        public ReactiveProperty<double> ItemWidth { get; } = new();
        public ReactiveProperty<double> ItemHeight { get; } = new(20);
        //public ReactiveProperty<double> Center { get; } = new();
        //public ReactiveProperty<Brush> BGColor { get; } = new(Brushes.White);
        //public ReactiveProperty<string> BGColorPath { get; } = new();

        //public ReactiveProperty<int> Layer { get; } = new(0);

        public ReactiveProperty<Brush> BGColor { get; }
        public ReactiveProperty<Brush> BorderColor { get; } = new(Brushes.Orange);
        public ReactiveProperty<double> Alpha { get; } = new(1);
        public ReactiveProperty<double> Thickness { get; } = new(0);
        public ReactiveProperty<double> Radius { get; } = new(5);


    }
}
