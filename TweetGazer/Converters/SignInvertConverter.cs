using System;
using System.Globalization;
using System.Windows.Data;

namespace TweetGazer.Converters
{
    /// <summary>
    /// 数値の正負を反転させるコンバーター
    /// </summary>
    public class SignInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return -(double)value;
            }
            else if (value is float)
            {
                return -(float)value;
            }

            return -(int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return -(double)value;
            }
            else if (value is float)
            {
                return -(float)value;
            }

            return -(int)value;
        }
    }
}
