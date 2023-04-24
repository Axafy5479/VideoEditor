using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Timeline
{
    internal static class Utility
    {
        public static T? GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static T? GetParentOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            var parent = VisualTreeHelper.GetParent(depObj);
            var result = (parent as T) ?? GetParentOfType<T>(parent);
            if (result != null) return result;

            return null;
        }

        public static ModifierKeys KeyToModif(Key key)
        {
            if(key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                return ModifierKeys.Control;
            }

            if (key == Key.LeftShift || key == Key.RightShift)
            {
                return ModifierKeys.Shift;
            }

            if(key==Key.LeftAlt || key == Key.RightAlt)
            {
                return ModifierKeys.Alt;
            }

            if(key==Key.LWin || key == Key.RWin)
            {
                return ModifierKeys.Windows;
            }

            return ModifierKeys.None;
        }


        public static ModifierKeys CurrentModifierKeys()
        {
            ModifierKeys ans = ModifierKeys.None;
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                ans |= ModifierKeys.Alt;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                ans |= ModifierKeys.Control;
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                ans |= ModifierKeys.Shift;
            }
            if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
            {
                ans |= ModifierKeys.Windows;
            }
            return ans;
        }
    }

    public class DependencyPropertyChanged<T> : DependencyObject
    {
        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(T), typeof(DependencyPropertyChanged<T>),
                new PropertyMetadata(default(T), (obj, e) => (obj as DependencyPropertyChanged<T>).OnValueChanged()));

        static ConditionalWeakTable<DependencyObject, List<DependencyObject>> _instances = new ConditionalWeakTable<DependencyObject, List<DependencyObject>>();
        Action<T> _changed;
        public DependencyPropertyChanged(DependencyObject target, string path, Action<T> changed)
        {
            _changed = changed;
            _instances.GetOrCreateValue(target).Add(this);
            BindingOperations.SetBinding(this, DependencyPropertyChanged<double>.ValueProperty,
                new Binding(path) { Source = target, Mode = BindingMode.OneWay });
        }
        void OnValueChanged() =>
            _changed(Value);
    }
}
