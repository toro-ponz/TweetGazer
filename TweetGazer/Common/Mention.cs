using CoreTweet;

namespace TweetGazer.Common
{
    /// <summary>
    /// メンションデータクラス
    /// </summary>
    public class Mention
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="status">メンション</param>
        public Mention(Status status)
        {
            this.Status = status;
        }
        
        public Status Status { get; }
    }
}
