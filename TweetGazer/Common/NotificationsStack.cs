using System.Collections.ObjectModel;
using System.Windows.Data;

namespace TweetGazer.Common
{
    public static class NotificationsStack
    {
        /// <summary>
        /// コンストラクタ
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
        public static void StackNotification(string text)
        {
            if (Notifications == null)
            {
                Notifications = new ObservableCollection<Notification>();
                BindingOperations.EnableCollectionSynchronization(Notifications, new object());
            }
            Notifications.Add(new Notification()
            {
                Text = text
            });
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