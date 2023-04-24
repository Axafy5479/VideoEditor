using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ParameterEditor
{
    public class CharaImageComboboxViewModel : IDisposable
    {
        private CompositeDisposable disposables = new();


        public void Initialize(List<string> allSelection,List<ReactiveProperty<string>> parameters, Func<string, CommandBase>? commandOnDecided)
        {
            var model = new CharaImageComboboxModel();
            model.Initialize(allSelection, parameters, commandOnDecided);
            Model = model;

            foreach (var item in Model.allFilePathes)
            {
                Items.Add(item);
            }

            Model.SelectedValue.Subscribe(value =>
            {
                List<string> allFileName = Model.allFilePathes.ConvertAll(x=>Path.GetFileNameWithoutExtension(x));
                string selectedFileName = Path.GetFileNameWithoutExtension(value);
                ShowingIndex.Value = allFileName.IndexOf(selectedFileName);
            }).AddTo(disposables);

            // コンボボックスUIから変更通知があった際に呼ばれる
            ShowingIndex.Where(n=>n>=0).Subscribe(n =>
            {
                Model.Decided(Model.allFilePathes[n]);
            }).AddTo(disposables);

            ShowingIndex.Value = Items.IndexOf(model.SelectedValue.Value);
            model.Decided(parameters[0].Value);
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
