using System.Windows;
using System.Windows.Controls;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Selectors
{
    public class ShowStatusItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;

            var showStatus = container as FrameworkElement;
            var data = item as TimelineItemProperties;
            if (showStatus == null || data == null)
                return null;
            switch (data.Type)
            {
                case TimelineItemType.Status:
                    {
                        switch (data.StatusProperties.Type)
                        {
                            case StatusType.IndividualMain:
                                result = showStatus.FindResource("MainStatus") as DataTemplate;
                                break;
                            case StatusType.IndividualOther:
                                result = showStatus.FindResource("ReplyStatus") as DataTemplate;
                                break;
                        }
                        break;
                    }
                case TimelineItemType.Button:
                    {
                        switch (data.LoadingProperties.Type)
                        {
                            case LoadingType.ReadMoreReplies:
                                result = showStatus.FindResource("ReadMoreReplies") as DataTemplate;
                                break;
                            case LoadingType.ReadMoreRepliesButton:
                                result = showStatus.FindResource("ReadMoreRepliesButton") as DataTemplate;
                                break;
                            case LoadingType.ReadMoreRepliesToMainStatus:
                                result = showStatus.FindResource("ReadMoreRepliesToMainStatus") as DataTemplate;
                                break;
                        }
                        break;
                    }
            }

            return result;
        }
    }
}