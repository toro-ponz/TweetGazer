using System;
using System.Globalization;
using System.Windows.Data;

namespace TweetGazer.Converters
{
    /// <summary>
    /// bool値を反転させるコンバーター
    /// </summary>
    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return false;
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool && !(bool)value);
        }
    }
}
