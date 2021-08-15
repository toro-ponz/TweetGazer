using System.Collections.Generic;
using System.Linq;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class Search : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Search() : base()
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
                                TimelineType = TimelineType.TrendStack
                            }
                        }
                    }
                );
            }

            this.ScreenNames.Clear();
            foreach (var user in AccountTokens.Users)
            {
                this.ScreenNames.Add("@" + user.ScreenName);
            }

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
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Flyoutを閉じるとき
        /// </summary>
        public override void Close()
        {
            this.SearchText = "";
            base.Close();
            this.Timeline.Home();
        }

        /// <summary>
        /// 検索
        /// </summary>
        public void Searching()
        {
            this.Timeline.ShowSearchTimeline(this.SearchText);
        }

        #region SearchText 変更通知プロパティ
        public string SearchText
        {
            get
            {
                return this._SearchText;
            }
            set
            {
                this._SearchText = value;
                this.RaisePropertyChanged();
            }
        }
        private string _SearchText;
        #endregion

        public TimelineModel Timeline;
    }
}
