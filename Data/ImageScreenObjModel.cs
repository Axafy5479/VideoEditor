using CommunityToolkit.Mvvm.ComponentModel;
using Data;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class ImageScreenObjModel : TimelineItemObjectModel, IDisposable
    {
        public void Dispose()
        {
            Disposables.Dispose();
        }

        public ImageScreenObjModel(ImageItem imageItem) : base(imageItem)
        {
            ImageItem = imageItem;

            #region Observe Rendering Properties
            ImageItem.ObserveProperty(x => x.Zoom).Subscribe(z => Zoom.Value = z).AddTo(Disposables); ;
            ImageItem.ObserveProperty(x => x.X).Subscribe(z => X.Value = z).AddTo(Disposables); ;
            ImageItem.ObserveProperty(x => x.Y).Subscribe(z => Y.Value = z).AddTo(Disposables); ;
            ImageItem.ObserveProperty(x => x.FadeIn).Subscribe(z => FadeIn.Value = z).AddTo(Disposables);
            ImageItem.ObserveProperty(x => x.FadeOut).Subscribe(z => FadeOut.Value = z).AddTo(Disposables);
            #endregion
        }

        private CompositeDisposable Disposables { get; } = new();

        #region Rendering Properties
        public ReactiveProperty<double> Zoom { get; } = new();
        public ReactiveProperty<double> X { get; } = new();
        public ReactiveProperty<double> Y { get; } = new();
        public ReactiveProperty<double> FadeIn { get; } = new();
        public ReactiveProperty<double> FadeOut { get; } = new();
        #endregion

        public ImageItem ImageItem { get; } = new();
    }
}
