using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VolumeAutoLimiter.Views.Converter
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colors = parameter?.ToString()?.Split(';');
            var trueColor = (Color)ColorConverter.ConvertFromString(colors?[0] ?? "Transparent");
            var falseColor = (Color)ColorConverter.ConvertFromString(colors?[1] ?? "#77ff0000");
            if ((bool)value)
            {
                return new SolidColorBrush(trueColor);
            }
            else
            {
                return new SolidColorBrush(falseColor);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
