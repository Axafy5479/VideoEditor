using CommandProject.Commands;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using MahApps.Metro.Controls;
using ParameterEditor.CustomControls;

namespace ParameterEditor
{
    public class Parameter_EnumCombo : Control, IDisposable
    {
        static Parameter_EnumCombo()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Parameter_EnumCombo), new FrameworkPropertyMetadata(typeof(Parameter_EnumCombo)));
        }

        public Parameter_EnumCombo()
        {
        }

        public void Dispose()
        {
            EnumBox.Dispose();
        }

        private EnumCombobox EnumBox { get; set; }
        public ReactiveProperty<string> Title { get; } = new();

        public void Initialize<T>(List<ReactiveProperty<T>> properties,  Func<T, CommandBase>? commandOnDecided, string title) where T : struct
        {
            if (IsLoaded) _Initialize(properties, commandOnDecided, title);
            else Loaded+=(s,e)=> _Initialize(properties, commandOnDecided, title);

        }

        public void _Initialize<T>(List<ReactiveProperty<T>> properties, Func<T, CommandBase>? commandOnDecided, string title) where T:struct
        {
            EnumBox = (EnumCombobox)this.Template.FindName("EnumCombobox",this);
            EnumBox.Initialize(properties, commandOnDecided);
            Title.Value = title;
        }
    }
}
