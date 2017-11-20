using Livet;
using System.Windows.Media;

namespace TweetGazer.Models.MainWindow
{
    public class ToastNotification : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">通知内容</param>
        /// <param name="type"></param>
        public ToastNotification(string message, NotificationType type)
        {
            this.Message = message.TrimEnd('\n', '\r', ' ');

            switch (type)
            {
                case NotificationType.Normal:
                    this.Color = new SolidColorBrush(Colors.DodgerBlue);
                    break;
                case NotificationType.Success:
                    this.Color = new SolidColorBrush(Colors.Lime);
                    break;
                case NotificationType.Alert:
                    this.Color = new SolidColorBrush(Colors.Orange);
                    break;
                case NotificationType.Error:
                    this.Color = new SolidColorBrush(Colors.Red);
                    break;
            }
            this.Color.Freeze();
        }

        public Brush Color { get; }

        public string Message { get; }
    }

    public enum NotificationType
    {
        Normal,
        Success,
        Alert,
        Error,
    }
}
