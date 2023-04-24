using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ParameterEditor
{
    public class TextItemBoxModel : IDisposable
    {
        public TextItemBoxModel(List<ReactiveProperty<string>> parameters, Func<string,CommandBase>? commandOnDecided)
        {
            Parameters = parameters;
            CommandOnDecided = commandOnDecided;
            foreach (var p in parameters)
            {
                p.Subscribe(_ => 
                {
                    int species = Parameters.ConvertAll(p => p.Value).Distinct().Count();
                    text.Value = species == 1 ? Parameters[0].Value : null;
                }).AddTo(Disposables);
            }

            text.Subscribe(t => {
                foreach (var p in parameters)
                {
                    p.Value = t;
                }
            }).AddTo(Disposables);
        }

        private CompositeDisposable Disposables { get; } = new();

        private ReactiveProperty<string> text = new();
        public ReactiveProperty<string> Text => text;

        private List<ReactiveProperty<string>> Parameters { get; }
        public Func<string, CommandBase>? CommandOnDecided { get; }

        public void Decided()
        {
            if (CommandOnDecided != null)
            {
                var command = CommandOnDecided(Text.Value);
                CommandInvoker.Instance.Execute(command);
            }
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
