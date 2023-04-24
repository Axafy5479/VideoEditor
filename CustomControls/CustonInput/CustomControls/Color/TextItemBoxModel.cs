using CommandProject.Commands;
using Data;
using OpenCvSharp;
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
using System.Windows.Media;

namespace ParameterEditor
{
    public class ColorCodeBoxModel : IDisposable
    {
        public ColorCodeBoxModel(List<ReactiveProperty<SolidColorBrush>> parameters, Func<SolidColorBrush, CommandBase>? commandOnDecided)
        {
            Parameters = parameters;
            CommandOnDecided = commandOnDecided;
            //foreach (var p in parameters)
            //{

            //    p.Subscribe(_ => 
            //    {
            //        int species = Parameters.ConvertAll(p => p.Value.Color.ToColorCode()).Distinct().Count();
            //        text.Value = species == 1 ? Parameters[0].Value.Color.ToColorCode() : null;
            //    }).AddTo(Disposables);
            //}

            text.Subscribe(t => {
                SolidColorBrush b = t.ToBrushOrNull();
                if (b == null) return;

                foreach (var p in parameters)
                {
                    p.Value = b;
                }
            }).AddTo(Disposables);

            text.Value = parameters[0].Value.Color.ToColorCode();
            oldText = text.Value;
        }

        private CompositeDisposable Disposables { get; } = new();

        private ReactiveProperty<string> text = new();
        public ReactiveProperty<string> Text => text;

        public List<ReactiveProperty<SolidColorBrush>> Parameters { get; }
        public Func<SolidColorBrush, CommandBase>? CommandOnDecided { get; }

        private string oldText;
        public void DecidedFromText()
        {

            if (Text.Value.ToBrushOrNull() is not SolidColorBrush b)
            {
                text.Value = oldText;
                return;
            }

            oldText = Text.Value;

            foreach (var p in Parameters)
            {
                p.Value = b;
            }

            if (CommandOnDecided != null)
            {
                var command = CommandOnDecided(b);
                CommandInvoker.Instance.Execute(command);
            }
        }

        public void DecidedFromBrush()
        {
            oldText = Parameters[0].Value.Color.ToColorCode();
            text.Value = oldText;

            if (CommandOnDecided != null)
            {
                var command = CommandOnDecided(Parameters[0].Value);
                CommandInvoker.Instance.Execute(command);
            }
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
