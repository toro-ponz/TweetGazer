using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class DirectMessages : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DirectMessages()
        {
            DirectMessagesStack.Initialize();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public override void Close()
        {
            DirectMessagesStack.Clear();
        }
    }
}