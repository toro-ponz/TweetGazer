using CoreTweet;
using Livet;
using TweetGazer.Behaviors;

namespace TweetGazer.Models.Timeline
{
    public class UserOverviewProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="user">ユーザーデータ</param>
        public UserOverviewProperties(User user)
        {
            if (user == null || user.Id == null)
                return;

            this.Id = (long)user.Id;
            this.Name = user.Name;
            this.ScreenName = user.ScreenName;
            this.IsProtected = user.IsProtected;
            this.IsVerified = user.IsVerified;

            this.ProfileImageNormal = new ImageProperties(user.ProfileImageUrlHttps, true, 0, 0);
            this.ProfileImageBigger = new ImageProperties(user.ProfileImageUrlHttps.Replace("_normal.", "_bigger."), true, 0, 0);
            this.ProfileImageOriginal = new ImageProperties(user.ProfileImageUrlHttps.Replace("_normal.", "."), true, 0, 0);
        }

        public ImageProperties ProfileImageNormal { get; }
        public ImageProperties ProfileImageBigger { get; }
        public ImageProperties ProfileImageOriginal { get; }

        public long Id { get; }
        public string Name { get; }
        public string ScreenName { get; }
        public bool IsProtected { get; }
        public bool IsVerified { get; }
    }
}