using System.Windows;
using System.Windows.Controls;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.Selectors
{
    public class MainWindowNotificationTrayTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;

            var itemsControl = container as FrameworkElement;
            var data = item as TrayNotification;
            if (itemsControl == null || data == null)
            {
                return null;
            }

            switch (data.Type)
            {
                case NotificationType.Alert:
                    result = itemsControl.FindResource("NotificationAlertTray") as DataTemplate;
                    break;
                case NotificationType.Error:
                    result = itemsControl.FindResource("NotificationErrorTray") as DataTemplate;
                    break;
                case NotificationType.Normal:
                    result = itemsControl.FindResource("NotificationNormalTray") as DataTemplate;
                    break;
                case NotificationType.Success:
                    result = itemsControl.FindResource("NotificationSuccessTray") as DataTemplate;
                    break;
            }

            return result;
        }
    }
}
