using System.Windows;
using System.Windows.Controls;
using TweetGazer.Behaviors;

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
                return null;
            
            switch (data.Type)
            {
                case StatusMediaType.Image:
                    result = media.FindResource("ImageTemplate") as DataTemplate;
                    break;
                case StatusMediaType.AnimationGif:
                    result = media.FindResource("GifTemplate") as DataTemplate;
                    break;
                case StatusMediaType.Video:
                    result = media.FindResource("VideoTemplate") as DataTemplate;
                    break;
            }

            return result;
        }
    }
}