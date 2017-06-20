using CoreTweet;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Common
{
    /// <summary>
    /// メンションデータクラス
    /// </summary>
    public class Mention
    {
        /// <summary>
        /// メンションを送ってきたユーザー
        /// </summary>
        public UserOverviewProperties User { get; set; }
        /// <summary>
        /// 当該ツイート
        /// </summary>
        public Status Status { get; set; }
    }
}