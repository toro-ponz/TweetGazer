using TweetGazer.Models.Timeline;

namespace TweetGazer.Common
{
    public class DirectMessage
    {
        /// <summary>
        /// ダイレクトメッセージを送ってきたユーザー
        /// </summary>
        public UserOverviewProperties User { get; set; }
        /// <summary>
        /// 当該ダイレクトメッセージ
        /// </summary>
        public CoreTweet.DirectMessage Message { get; set; }
    }
}