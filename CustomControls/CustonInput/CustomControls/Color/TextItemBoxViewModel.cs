using CommandProject.Commands;
using Data;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ParameterEditor
{
    public class ColorCodeBoxViewModel : IDisposable
    {
        private const int MoveTreshold = 2;
        private CompositeDisposable disposables = new();

        public ColorCodeBoxViewModel(List<ReactiveProperty<SolidColorBrush>> parameters, Func<SolidColorBrush, CommandBase>? commandOnDecided)
        {
            Model = new ColorCodeBoxModel(parameters, commandOnDecided);
            ShowingText = Model.Text;
            
        }
        public IReadOnlyReactiveProperty<string> ShowingText { get; }
        public ColorCodeBoxModel Model { get; }

        internal void OnLostKeyboardFocus()
        {
            Model.DecidedFromText();
        }

        public void Dispose()
        {
            Model.Dispose();
            disposables.Dispose();
        }


    }
}
