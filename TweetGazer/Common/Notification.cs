using CoreTweet;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Common
{
    /// <summary>
    /// 通知データクラス
    /// </summary>
    public class Notification
    {
        public Notification(User sentUser, User receiveUser, NotificationPropertiesType type, object parameter = null, long? id = null)
        {
            this.NotificationPropertiesType = type;
            this.SentUser = new UserOverviewProperties(sentUser);
            this.ReceiveUser = new UserOverviewProperties(receiveUser);

            if (parameter is string)
            {
                this.Text = parameter as string;
            }

            if (id != null)
            {
                this.Id = (long)id;
            }
        }

        public NotificationPropertiesType NotificationPropertiesType { get; }

        public UserOverviewProperties SentUser { get; }
        public UserOverviewProperties ReceiveUser { get; }

        public string Text { get; }

        public long Id { get; }
    }
}
