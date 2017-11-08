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
        }

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
