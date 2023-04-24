using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Data
{
    public static class Utility
    {



        public static string ToColorCode(this Color color)
        {
            return string.Format(
            "#{0:X2}{1:X2}{2:X2}",
            color.R,
            color.G,
            color.B);
        }

        public static List<T> FindAll<T>(this IEnumerable<T> collection, Predicate<T> judge)
        {
            List<T> list = new List<T>();
            foreach (var item in collection)
            {
                if(judge(item))list.Add(item);
            }
            return list;
        }

        public static List<To> ConvertAll<From,To>(this IEnumerable<From> collection, Func<From,To> converter)
        {
            List<To> list = new List<To>();
            foreach (var item in collection)
            {
                list.Add(converter(item));
            }
            return list;
        }

        public static List<T> NonNull<T>(this IEnumerable<T?> collection) where T:class
        {
            List<T> list = new();
            foreach (var item in collection)
            {
                if(item!=null)list.Add(item);
            }
            return list;
        }

        public static T? Find<T>(this IEnumerable<T> collection, Predicate<T> judge) where T : class
        {
            foreach (var item in collection)
            {
                if (judge(item)) return item;
            }

            return null;
        }

        public static ControlTemplate GetRoundedTextBoxTemplate(int radius)
        {
            ControlTemplate template = new ControlTemplate(typeof(TextBoxBase));
            FrameworkElementFactory elemFactory = new FrameworkElementFactory(typeof(Border));
            elemFactory.Name = "Border";
            elemFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(radius));
            elemFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Black));
            elemFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(TextBox.BackgroundProperty));
            elemFactory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(TextBox.BorderThicknessProperty));
            elemFactory.SetValue(Border.SnapsToDevicePixelsProperty, true);
            template.VisualTree = elemFactory;


            FrameworkElementFactory scrollViewerElementFactory = new FrameworkElementFactory(typeof(ScrollViewer));
            scrollViewerElementFactory.Name = "PART_ContentHost";
            elemFactory.AppendChild(scrollViewerElementFactory);


            Trigger isEnabledTrigger = new Trigger();
            isEnabledTrigger.Property = TextBox.IsEnabledProperty;
            isEnabledTrigger.Value = false;

            Setter backgroundSetter = new Setter();
            backgroundSetter.TargetName = "Border";
            backgroundSetter.Property = TextBox.BackgroundProperty;
            backgroundSetter.Value = SystemColors.ControlBrush;

            Setter foregroundSetter = new Setter();
            foregroundSetter.Property = TextBox.ForegroundProperty;
            foregroundSetter.Value = SystemColors.GrayTextBrush;

            isEnabledTrigger.Setters.Add(backgroundSetter);
            isEnabledTrigger.Setters.Add(foregroundSetter);


            template.Triggers.Add(isEnabledTrigger);

            return template;
        }

        public static SolidColorBrush ToSolidColorBrush(this Color mColor, bool isFreeze = false) =>
    isFreeze
    ? new SolidColorBrush(mColor)
    : new SolidColorBrush(mColor).WithFreeze();

        public static T WithFreeze<T>(this T freezableObj) where T : Freezable
        {
            freezableObj.Freeze();
            return freezableObj;
        }

        public static Color ToColorOrDefault(this string code) => ToColorOrNull(code) ?? default;

        public static Color? ToColorOrNull(this string code)
        {
            if (code == null) return null;

            try
            {
                return (Color)ColorConverter.ConvertFromString(code);
            }
            catch (FormatException _)
            {
                return null;
            }
        }

        public static SolidColorBrush? ToBrushOrNull(this string code)
        {
            if (code == null) return null;
            try
            {
                return code.ToColorOrNull()?.ToSolidColorBrush();
            }
            catch (FormatException _)
            {
                return null;
            }
        }
    }
}
