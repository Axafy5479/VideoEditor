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


    public interface IComboboxModel : IDisposable
    {
        void Decided(string enumValue);
        IReadOnlyReactiveProperty<string> SelectedValue { get; }
        List<string> allFilePathes { get; }
    }

    public class EnumComboboxModel<T> : IComboboxModel where T:struct
    {
        public EnumComboboxModel()
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("入力された型" + typeof(T) + "は列挙型ではありません");
            }

            allFilePathes = new List<string>();
        }

        public void Initialize(List<ReactiveProperty<T>> parameters, Func<T, CommandBase>? commandOnDecided)
        {
            allFilePathes.Clear();

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                allFilePathes.Add(item.ToString());
                ValueEnumMap.Add(item.ToString(), item);
            }

            Parameters = parameters;
            CommandOnDecided = commandOnDecided;

            // 実際にパラメーターが変化した際に、表示も変更する
            foreach (var p in parameters)
            {
                p.Subscribe(_ =>
                {
                    int species = Parameters.ConvertAll(p => p.Value).Distinct().Count();
                    selectedValue.Value = species == 1 ? Parameters[0].Value.ToString() : "";
                }).AddTo(Disposables);
            }

            Decided(allFilePathes[0]);
        }

        private CompositeDisposable Disposables { get; } = new();



        // 表示する文字
        private ReactiveProperty<string> selectedValue = new();
        public IReadOnlyReactiveProperty<string> SelectedValue => selectedValue;

        // 全ての選択肢
        public List<string> allFilePathes { get; set; }
        public Dictionary<string, T> ValueEnumMap { get; } = new();


        private List<ReactiveProperty<T>> Parameters { get; set; }
        public Func<T, CommandBase>? CommandOnDecided { get; private set; }




        private T? IntToEnum(int n)
        {
            if (Enum.IsDefined(typeof(T), n))
            {
                return (T)Enum.ToObject(typeof(T), 1);
            }

            return null;
        }

        public void Decided(string enumValue)
        {
            selectedValue.Value = enumValue;

            if(ValueEnumMap.TryGetValue(enumValue, out T t))
            {
                foreach (var item in Parameters)
                {
                    item.Value = t;
                }

                if (CommandOnDecided != null)
                {
                    var command = CommandOnDecided(t);
                    CommandInvoker.Instance.Execute(command);
                }
            }

        }

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}
