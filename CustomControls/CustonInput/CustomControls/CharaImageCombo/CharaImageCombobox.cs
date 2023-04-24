using CommandProject.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ParameterEditor
{
    public class CharaImageCombobox : ComboBox, IDisposable
    {
        static CharaImageCombobox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CharaImageCombobox), new FrameworkPropertyMetadata(typeof(CharaImageCombobox)));
        }

        public CharaImageCombobox()
        {
            ViewModel = new CharaImageComboboxViewModel();
            DataContext = ViewModel;

            Background = Brushes.AliceBlue;

            ViewModel.ShowingIndex.Subscribe(i =>
            {
                SelectedIndex = i;
            });

            // コンボボックスUIから変更通知があった際に、Modelの値を変更する
            SelectionChanged += (s, e) =>
            {
                ViewModel.ShowingIndex.Value = SelectedIndex;
            };
        }

        public void Initialize(List<string> allSelection,List<ReactiveProperty<string>> properties, Func<string, CommandBase>? commandOnDecided)
        {
            ViewModel.Initialize(allSelection,properties, commandOnDecided);
            //ItemsSource = ViewModel.Items;
            SelectedIndex = ViewModel.ShowingIndex.Value;
        }

        public CharaImageComboboxViewModel ViewModel { get; }
        private CompositeDisposable disposables { get; } = new();


        public void Dispose()
        {
            ViewModel?.Dispose();
            disposables.Dispose();
        }

    }
}
