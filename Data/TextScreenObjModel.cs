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
using System.Windows.Media;
using Timeline.CustomControl;

namespace ScreenProject
{
    public class TextScreenObjModel : TimelineItemObjectModel, IDisposable
    {
        public void Dispose()
        {
            Disposables.Dispose();
        }

        public TextScreenObjModel(TextItem textItem) : base(textItem)
        {
            TextItem = textItem;

            #region Observe Rendering Properties
            TextItem.ObserveProperty(x => x.Zoom).Subscribe(z => Zoom.Value = z).AddTo(Disposables); 
            TextItem.ObserveProperty(x => x.FadeIn).Subscribe(z => FadeIn.Value = z).AddTo(Disposables); 
            TextItem.ObserveProperty(x => x.FadeOut).Subscribe(z => FadeOut.Value = z).AddTo(Disposables); 
            TextItem.ObserveProperty(x => x.X).Subscribe(z => X.Value = z).AddTo(Disposables); 
            TextItem.ObserveProperty(x => x.Y).Subscribe(z => Y.Value = z).AddTo(Disposables); 
            #endregion

            #region Observe Audio Properties
            TextItem.ObserveProperty(x => x.Text).Subscribe(z => Text.Value = z).AddTo(Disposables);
            TextItem.ObserveProperty(x => x.Color).Subscribe(z => Color.Value = z.ToBrushOrNull()).AddTo(Disposables);
            TextItem.ObserveProperty(x => x.Color2).Subscribe(z => Color2.Value = z.ToBrushOrNull()).AddTo(Disposables);
            TextItem.ObserveProperty(x => x.FontSize).Subscribe(z => FontSize.Value = z).AddTo(Disposables);
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

        #region Text Properties
        public ReactiveProperty<double> FontSize { get; } = new();
        public ReactiveProperty<SolidColorBrush> Color { get; } = new();
        public ReactiveProperty<SolidColorBrush> Color2 { get; } = new();
        public ReactiveProperty<string> Text { get; } = new();
        #endregion

        public TextItem TextItem { get; } = new();
    }
}
