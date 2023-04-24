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
    public class EnumCombobox : ComboBox, IDisposable
    {
        static EnumCombobox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumCombobox), new FrameworkPropertyMetadata(typeof(EnumCombobox)));
        }

        public EnumCombobox()
        {
            ViewModel = new EnumComboboxViewModel();
            DataContext = ViewModel;

            Background = Brushes.AliceBlue;

            SelectionChanged += (s, e) =>
            {
                ViewModel.ShowingIndex.Value = SelectedIndex;
            };
        }

        public void Initialize<T>(List<ReactiveProperty<T>> properties, Func<T, CommandBase>? commandOnDecided) where T:struct
        {
            ViewModel.Initialize(properties, commandOnDecided);
            //ItemsSource = ViewModel.Items;
            SelectedIndex = ViewModel.ShowingIndex.Value;
        }

        public EnumComboboxViewModel ViewModel { get; }
        private CompositeDisposable disposables{get;} = new();


        public void Dispose()
        {
            ViewModel?.Dispose();
            disposables.Dispose();
        }

    }
}
