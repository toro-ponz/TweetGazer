using System.Collections.Generic;
using System.Linq;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class Notifications : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Notifications() : base()
        {

        }

        /// <summary>
        /// Flyoutを開くとき
        /// </summary>
        public async override void Open()
        {
            if (this.Timeline == null)
            {
                this.Timeline = new TimelineModel(
                    new Timeline.TimelineData()
                    {
                        TokenSuffix = 0,
                        ColumnIndex = -1,
                        ScreenName = AccountTokens.Users.First().ScreenName,
                        UserId = AccountTokens.Users.First().Id,
                        Pages = new List<Timeline.TimelinePageData>()
                        {
                            new Timeline.TimelinePageData()
                            {
                                TimelineType = TimelineType.NotificationStack
                            }
                        }
                    }
                );
            }

            this.ScreenNames.Clear();
            foreach (var user in AccountTokens.Users)
                this.ScreenNames.Add("@" + user.ScreenName);

            this.Timeline.TokenSuffix = 0;
            await this.Timeline.Update();
        }

        /// <summary>
        /// Flyoutを開けるかどうかの判定
        /// </summary>
        /// <returns>Flyoutを開けるかどうか</returns>
        public override bool OpenConditions()
        {
            if (AccountTokens.TokensCount == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Flyoutを閉じるとき
        /// </summary>
        public override void Close()
        {
            base.Close();
            this.Timeline.Home();
        }

        /// <summary>
        /// 通知を削除する
        /// </summary>
        public void Delete()
        {
            while (this.Timeline.TimelineItems.Count != 0)
            {
                this.Timeline.TimelineItems.RemoveAt(0);
                if (NotificationsStack.Notifications.Count != 0)
                    NotificationsStack.Notifications.RemoveAt(0);
            }
        }

        public TimelineModel Timeline;
    }
}