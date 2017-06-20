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
        public TimelineNotice(string message, NoticeType type)
        {
            this.Message = message.TrimEnd('\n', '\r', ' ');
            this.Type = type;

            switch (type)
            {
                case NoticeType.Normal:
                    this.BackgroundColor = new SolidColorBrush(Colors.DodgerBlue);
                    break;
                case NoticeType.Success:
                    this.BackgroundColor = new SolidColorBrush(Colors.Green);
                    break;
                case NoticeType.Alert:
                    this.BackgroundColor = new SolidColorBrush(Colors.Orange);
                    break;
                case NoticeType.Error:
                    this.BackgroundColor = new SolidColorBrush(Colors.Red);
                    break;
            }
            this.BackgroundColor.Freeze();
        }

        public Brush BackgroundColor { get; }

        public string Message { get; }

        public NoticeType Type;
    }

    public enum NoticeType
    {
        Normal,
        Success,
        Alert,
        Error,
    }
}