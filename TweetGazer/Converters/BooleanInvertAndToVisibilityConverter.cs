using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TweetGazer.Converters
{
    /// <summary>
    /// bool値を反転させ、Visibility値に変換するコンバーター
    /// </summary>
    public class BooleanInvertAndToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool && (bool)value))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is Visibility && ((Visibility)value == Visibility.Visible));
        }
    }
}