using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ParameterEditor
{
    public class TextItemBoxViewModel : IDisposable
    {
        private const int MoveTreshold = 2;
        private CompositeDisposable disposables = new();

        public TextItemBoxViewModel(List<ReactiveProperty<string>> parameters, Func<string,CommandBase>? commandOnDecided)
        {

            Model = new TextItemBoxModel(parameters, commandOnDecided);
            Model.Text.Subscribe(value =>
            {
                ShowingText.Value = value;
            }).AddTo(disposables);

            ShowingText.Subscribe(text =>
            {
                if(text != Model.Text.Value)
                {
                    Model.Text.Value = text;
                }
            }).AddTo(disposables);
        }
        public ReactiveProperty<string> ShowingText { get; } = new();
        public TextItemBoxModel Model { get; }

        internal void OnLostKeyboardFocus()
        {
            Model.Decided();
        }

        public void Dispose()
        {
            Model.Dispose();
            disposables.Dispose();
        }


    }
}
