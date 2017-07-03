using CoreTweet;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TweetGazer.Models;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Common
{
    public static class NotificationsStack
    {
        /// <summary>
        /// 初期化
        /// </summary>
        public static void Initialize()
        {
            Notifications = new ObservableCollection<Notification>();
            BindingOperations.EnableCollectionSynchronization(Notifications, new object());
        }

        /// <summary>
        /// 通知をスタックに積む
        /// </summary>
        /// <param name="text">通知内容</param>
        public static void StackNotification(User sentUser, User receiveUser, NotificationPropertiesType type, object parameter = null, long? id = null)
        {
            if (Notifications == null)
            {
                Notifications = new ObservableCollection<Notification>();
                BindingOperations.EnableCollectionSynchronization(Notifications, new object());
            }
            Notifications.Add(new Notification(sentUser, receiveUser, type, parameter, id));
        }

        /// <summary>
        /// スタックをクリアする
        /// </summary>
        public static void Clear()
        {
            if (Notifications != null)
                Notifications.Clear();
        }
        
        public static ObservableCollection<Notification> Notifications { get; private set; }
    }
}