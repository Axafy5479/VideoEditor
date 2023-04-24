using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Timeline.CustomControl;

namespace Timeline.Converter
{
    public class FrameXPositionConverter : IMultiValueConverter
    {

        public object Convert(
    object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int d1 = (int)values[0];
            double d2 = values[1] is double d ? d:0;
            return d1 * d2;
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }


    public class FrameWidthPositionConverter : IMultiValueConverter
    {

        public object Convert(
    object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var length = (int)values[0];
            var pixelPerFrame = (double)values[1];

            return length * pixelPerFrame;
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
