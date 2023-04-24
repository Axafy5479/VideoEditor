using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace ParameterEditor.CustomControls
{
    public class DoubleBoxModel : IDisposable
    {
        public DoubleBoxModel(List<ReactiveProperty<double>> parameters, Func<double, CommandBase>? commandOnDecided)
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
                if (num is double d)
                {
                    foreach (var p in parameters)
                    {
                        p.Value = d;
                    }
                }

            }).AddTo(Disposables);
        }

        private CompositeDisposable Disposables { get; } = new();

        private ReactiveProperty<double?> number = new();
        public ReactiveProperty<double?> Number => number;

        private List<ReactiveProperty<double>> Parameters { get; }
        public Func<double, CommandBase>? CommandOnDecided { get; }

        public void Decided()
        {
            if (Number.Value is not double n) return;

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
