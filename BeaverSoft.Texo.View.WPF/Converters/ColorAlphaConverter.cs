using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BeaverSoft.Texo.View.WPF.Converters
{
    public class ColorAlphaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color;

            if (value is string strValue)
            {
                ColorConverter converter = new ColorConverter();
                color = (Color)converter.ConvertFromInvariantString(strValue);
            }
            else
            {
                color = (Color)value;
            }

            byte alpha = byte.Parse(parameter.ToString(), CultureInfo.InvariantCulture);
            var newBrush = Color.FromArgb(alpha, color.R, color.G, color.B);
            return newBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
