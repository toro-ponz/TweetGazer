using Livet;
using System.Windows.Media;

namespace TweetGazer.Models.Timeline
{
    public class TimelineNotice : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">通知内容</param>
        /// <param name="type"></param>
        public TimelineNotice(string message, NotificationType type)
        {
            this.Message = message.TrimEnd('\n', '\r', ' ');
            this.Type = type;

            switch (type)
            {
                case NotificationType.Normal:
                    this.BackgroundColor = new SolidColorBrush(Colors.DodgerBlue);
                    break;
                case NotificationType.Success:
                    this.BackgroundColor = new SolidColorBrush(Colors.Green);
                    break;
                case NotificationType.Alert:
                    this.BackgroundColor = new SolidColorBrush(Colors.Orange);
                    break;
                case NotificationType.Error:
                    this.BackgroundColor = new SolidColorBrush(Colors.Red);
                    break;
            }
            this.BackgroundColor.Freeze();
        }

        public Brush BackgroundColor { get; }

        public string Message { get; }

        public NotificationType Type;
    }

    public enum NotificationType
    {
        Normal,
        Success,
        Alert,
        Error,
    }
}