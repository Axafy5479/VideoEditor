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
    public class EnumComboboxViewModel : IDisposable
    {
        private CompositeDisposable disposables = new();


        public void Initialize<T>(List<ReactiveProperty<T>> parameters, Func<T, CommandBase>? commandOnDecided)
            where T:struct
        {
            var model = new EnumComboboxModel<T>();
            model.Initialize(parameters, commandOnDecided);
            Model = model;

            foreach (var item in Model.allFilePathes)
            {
                Items.Add(item);
            }

            Model.SelectedValue.Subscribe(value =>
            {
                ShowingIndex.Value = Model.allFilePathes.IndexOf(value);
            }).AddTo(disposables);

            ShowingIndex.Subscribe(n =>
            {
                Model.Decided(Model.allFilePathes[n]);
            }).AddTo(disposables);

            ShowingIndex.Value = Items.IndexOf(model.SelectedValue.Value);
        }


        public ReactiveCollection<string> Items { get; } = new();
        public ReactiveProperty<int> ShowingIndex { get; } = new();
        public IComboboxModel Model { get; private set; }


        public List<object> ItemsObject { get; set; }


        public void Dispose()
        {
            Model.Dispose();
            disposables.Dispose();
        }


    }
}
