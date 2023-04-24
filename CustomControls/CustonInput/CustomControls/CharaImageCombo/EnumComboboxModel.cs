using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;

namespace ParameterEditor
{

    public class CharaImageComboboxModel : IComboboxModel
    {
        public CharaImageComboboxModel()
        {
            allFilePathes = new List<string>();
        }

        public void Initialize(List<string> allSelection,List<ReactiveProperty<string>> parameters, Func<string, CommandBase>? commandOnDecided)
        {
            allFilePathes = allSelection;

            Parameters = parameters;
            CommandOnDecided = commandOnDecided;

            // 実際にパラメーターが変化した際に、表示も変更する
            foreach (var p in parameters)
            {
                if (p.Value == null)
                {
                    p.Value = allSelection[0];
                }

                p.Subscribe(path =>
                {
                    int species = Parameters.ConvertAll(p => p.Value).Distinct().Count();
                    selectedValue.Value = species == 1 ? Parameters[0].Value : "";
                }).AddTo(Disposables);
            }

            if (allSelection.Any())
            {
                Decided(parameters[0].Value);
            }
        }

        private CompositeDisposable Disposables { get; } = new();



        // 表示する文字
        private ReactiveProperty<string> selectedValue = new();
        public IReadOnlyReactiveProperty<string> SelectedValue => selectedValue;

        // 全ての選択肢
        public List<string> allFilePathes { get; set; }


        private List<ReactiveProperty<string>> Parameters { get; set; }
        public Func<string, CommandBase>? CommandOnDecided { get; private set; }


        public void Decided(string enumValue)
        {
            selectedValue.Value = enumValue;

            foreach (var item in Parameters)
            {
                item.Value = enumValue;
            }

            if (CommandOnDecided != null)
            {
                var command = CommandOnDecided(enumValue);
                CommandInvoker.Instance.Execute(command);
            }
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
