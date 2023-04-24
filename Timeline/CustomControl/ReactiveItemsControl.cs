using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace Timeline.CustomControl
{
    public class ReactiveItemsControl : Grid
    {
        private ItemContainerGenerator Generator { get; }
        internal Subject<(TimelineItemObject added, TimelineItemObject removed)> OnChildrenChanged { get; } = new();

        //public ReactiveItemsControl()
        //{
        //    ((INotifyCollectionChanged)this.Items).CollectionChanged+=Test;
        //    Generator = this.ItemContainerGenerator;
        //}

        private void Test(object? sender, NotifyCollectionChangedEventArgs e)
        {

        }

        public Subject<(DependencyObject, DependencyObject)> ChildrenChanged = new();


        protected override void OnVisualChildrenChanged(DependencyObject added, DependencyObject removed)
        {
            base.OnVisualChildrenChanged(added, removed);

            if (added is ContentPresenter added_cp)
            {
                added_cp.Loaded += (o, s) =>
                {
                    var child = VisualTreeHelper.GetChild(added_cp, 0) as TimelineItemObject;

                    if (child != null)
                    {
                        OnChildrenChanged.OnNext((child, null));
                    }
                    else
                    {
                        Debug.WriteLine("childがnullです");
                    }
                };
            }

            else if(removed is ContentPresenter removed_cp)
            {
                var child = VisualTreeHelper.GetChild(removed_cp, 0) as TimelineItemObject;

                if (child != null)
                {
                    OnChildrenChanged.OnNext((null, child));
                }
                else
                {
                    Debug.WriteLine("childがnullです");
                }
            }


        }


    }
}
