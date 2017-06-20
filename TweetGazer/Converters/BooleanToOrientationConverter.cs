using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TweetGazer.Converters
{
    /// <summary>
    /// bool値を反転させ、Orientation値に変換するコンバーター
    /// </summary>
    public class BooleanToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool && (bool)value))
                return Orientation.Vertical;
            return Orientation.Horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is Orientation && ((Orientation)value == Orientation.Vertical));
        }
    }
}