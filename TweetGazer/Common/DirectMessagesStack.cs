using CoreTweet;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Common
{
    public class DirectMessagesStack
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public static void Initialize()
        {
            if (DirectMessages == null)
            {
                DirectMessages = new ObservableCollection<DirectMessage>();
                BindingOperations.EnableCollectionSynchronization(DirectMessages, new object());
            }
        }

        /// <summary>
        /// ダイレクトメッセージをスタックに積む
        /// </summary>
        /// <param name="user">送ってきたユーザー</param>
        /// <param name="message">送られてきたダイレクトメッセージ</param>
        public static void StackMention(User user, CoreTweet.DirectMessage message)
        {
            if (DirectMessages == null)
            {
                DirectMessages = new ObservableCollection<DirectMessage>();
                BindingOperations.EnableCollectionSynchronization(DirectMessages, new object());
            }
            DirectMessages.Add(new DirectMessage()
            {
                User = new UserOverviewProperties(user),
                Message = message
            });
        }

        /// <summary>
        /// スタックをクリアする
        /// </summary>
        public static void Clear()
        {
            if (DirectMessages != null)
            {
                DirectMessages.Clear();
            }
        }

        public static ObservableCollection<DirectMessage> DirectMessages { get; private set; }
    }
}
