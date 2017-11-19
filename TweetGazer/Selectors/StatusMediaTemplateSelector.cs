using System.Windows;
using System.Windows.Controls;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Selectors
{
    public class StatusMediaTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;

            var media = container as FrameworkElement;
            var data = item as MediaProperties;
            if (media == null || data == null)
            {
                return null;
            }

            switch (data.Type)
            {
                case StatusMediaType.Image:
                    result = media.FindResource("StatusImageTemplate") as DataTemplate;
                    break;
                case StatusMediaType.AnimationGif:
                    result = media.FindResource("StatusGifTemplate") as DataTemplate;
                    break;
                case StatusMediaType.Video:
                    result = media.FindResource("StatusVideoTemplate") as DataTemplate;
                    break;
            }

            return result;
        }
    }
}
