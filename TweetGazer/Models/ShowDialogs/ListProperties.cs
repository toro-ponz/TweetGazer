using CoreTweet;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Models.ShowDialogs
{
    public class ListProperties
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="list">リスト</param>
        /// <param name="isAdded">リストに操作対象のユーザーが既に追加されているか否か</param>
        public ListProperties(List list, bool isAdded = false)
        {
            this.Id = list.Id;
            this.Name = list.Name;
            this.Owner = new UserOverviewProperties(list.User);
            this.MemberCount = list.MemberCount;
            this.IsAdded = isAdded;
            this._IsAdded = isAdded;
        }

        public UserOverviewProperties Owner { get; }

        public long Id;
        public string Name { get; }
        public int MemberCount { get; }
        public bool IsAdded { get; set; }
        public bool _IsAdded;
        public bool IsChanged
        {
            get
            {
                return this.IsAdded != this._IsAdded;
            }
        }
    }
}