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
    public class NumberBoxModel : IDisposable
    {
        public NumberBoxModel(List<ReactiveProperty<int>> parameters, Func<int,CommandBase>? commandOnDecided)
        {
            Parameters = parameters;
            CommandOnDecided = commandOnDecided;
            foreach (var p in parameters)
            {
                p.Subscribe(_ => 
                {
                    int species = Parameters.ConvertAll(p => p.Value).Distinct().Count();
                    number.Value = species == 1 ? Parameters[0].Value : null;
                }).AddTo(Disposables);
            }

            number.Subscribe(num => {
                if (num is not int n) return;
                foreach (var p in parameters)
                {
                    p.Value = n;
                }
            }).AddTo(Disposables);
        }

        private CompositeDisposable Disposables { get; } = new();

        private ReactiveProperty<int?> number = new();
        public ReactiveProperty<int?> Number => number;

        private List<ReactiveProperty<int>> Parameters { get; }
        public Func<int, CommandBase>? CommandOnDecided { get; }

        public void Decided()
        {
            if (Number.Value is not int n) return;

            if (CommandOnDecided != null)
            {
                var command = CommandOnDecided(n);
                CommandInvoker.Instance.Execute(command);
            }
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
