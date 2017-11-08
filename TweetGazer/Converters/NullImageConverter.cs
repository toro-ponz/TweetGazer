using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TweetGazer.Converters
{
    /// <summary>
    /// ImageコントロールのSourceにnullや空のUriがバインドされたときに無視するコンバーター
    /// </summary>
    public class NullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else
            {
                var url = value as Uri;
                if (url == null || string.IsNullOrEmpty(url.OriginalString))
                {
                    return DependencyProperty.UnsetValue;
                }
                else
                {
                    return value;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
