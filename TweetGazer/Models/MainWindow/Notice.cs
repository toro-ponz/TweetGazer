using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class Notice : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Notice()
        {
            NotificationsStack.Initialize();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public override void Close()
        {
            NotificationsStack.Notifications.Clear();
        }
    }
}