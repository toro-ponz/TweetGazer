using System;
using System.Globalization;
using System.Windows.Data;

namespace TweetGazer.Converters
{
    /// <summary>
    /// 数値の正負を反転させ値を半分にするコンバーター
    /// </summary>
    public class SignInvertAndHalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return -(double)value / 2;
            }
            else if (value is float)
            {
                return -(float)value / 2;
            }

            return -(int)value / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return -(double)value * 2;
            }
            else if (value is float)
            {
                return -(float)value * 2;
            }

            return -(int)value * 2;
        }
    }
}
