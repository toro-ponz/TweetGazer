using Livet;
using System.Windows.Media;

namespace TweetGazer.Models.MainWindow
{
    public class TrayNotification : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">通知内容</param>
        /// <param name="type"></param>
        public TrayNotification(string message, NotificationType type)
        {
            this.Message = message.TrimEnd('\n', '\r', ' ');
            this.Type = type;
        }

        public NotificationType Type;

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