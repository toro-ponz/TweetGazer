using System.Windows;
using System.Windows.Controls;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Selectors
{
    public class TimelineItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;

            var timeline = container as FrameworkElement;
            var data = item as TimelineItemProperties;
            if (timeline == null || data == null)
                return null;

            switch (data.TimelineItemType)
            {
                case TimelineItemType.Status:
                    result = timeline.FindResource("Status") as DataTemplate;
                    break;
                case TimelineItemType.UserOverview:
                    result = timeline.FindResource("User") as DataTemplate;
                    break;
                case TimelineItemType.Trend:
                    result = timeline.FindResource("Trend") as DataTemplate;
                    break;
                case TimelineItemType.UserTimelineTab:
                    result = timeline.FindResource("UserTimelineTab") as DataTemplate;
                    break;
                case TimelineItemType.SearchTimelineTab:
                    result = timeline.FindResource("SearchTimelineTab") as DataTemplate;
                    break;
                case TimelineItemType.Button:
                    result = timeline.FindResource("Loading") as DataTemplate;
                    break;
                case TimelineItemType.Notification:
                    result = timeline.FindResource("Notification") as DataTemplate;
                    break;
            }

            return result;
        }
    }
}