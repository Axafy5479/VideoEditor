using CommandProject.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParameterEditor.Commands
{
    public abstract class ScreenCommandBase<T> : RedoableCommandBase where T : class
    {
        public ScreenCommandBase(ReadOnlyCollection<T> selectedItem)
        {
            SelectedItem = selectedItem;
            N = SelectedItem.Count;
        }

        public int N { get; }
        public ReadOnlyCollection<T> SelectedItem { get; }
    }
}
