using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ParameterEditor
{
    public class PathImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double maxHeight = 120;
            double maxWidth = 120;


            var pathes = (ReactiveCollection<string>)value;
            List<object> images = new();
            foreach (var path in pathes)
            {
                if (File.Exists(path))
                {
                    var image = new Image();
                    var source = (BitmapSource)new BitmapImage(new Uri(path));

                    double ratio = Math.Min(maxWidth/source.Width, maxHeight/source.Height);

                    var scaledSource = new TransformedBitmap(source,
                        new ScaleTransform(ratio, ratio));

                    image.Source = scaledSource;
                    images.Add(image);
                }
                else
                {
                    images.Add(new TextBlock() { 
                        Text=path, 
                        Width=maxWidth, 
                        Height=maxHeight, 
                        TextAlignment= TextAlignment.Center,
                         VerticalAlignment = VerticalAlignment.Center,
                    });
                }
            }

            return images;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
