using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class Mentions : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Mentions()
        {
            MentionsStack.Initialize();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public override void Close()
        {
            MentionsStack.Clear();
        }
    }
}