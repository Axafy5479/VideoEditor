using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Timeline.Converter
{
    public abstract class BasicConverterBase : DependencyObject, IValueConverter
    {
        #region IValueConverter Members

        public abstract object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        #endregion


        protected double GetDoubleValue(object parameter, double defaultValue)
        {
            double a;
            if (parameter != null)
                try
                {
                    a = System.Convert.ToDouble(parameter);
                }
                catch
                {
                    a = defaultValue;
                }
            else
                a = defaultValue;
            return a;
        }
    }

    public abstract class LinerConverterBase : BasicConverterBase
    {
        public abstract double A { get; }
        public abstract double B { get; }

        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double a = GetDoubleValue(parameter, A);

            double x = GetDoubleValue(value, 0.0);

            return (a * x) + B;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double a = GetDoubleValue(parameter, A);

            double y = GetDoubleValue(value, 0.0);

            return (y-B) / a;
        }

        #endregion
    }

}
