using Livet;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class UserTimelineTabProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="tab">タブ</param>
        public UserTimelineTabProperties(TimelineModel timelineModel, UserTimelineTab tab = UserTimelineTab.Tweets)
        {
            this.TimelineModel = timelineModel;
            this._Tab = tab;

            this.TweetsTabButtonCommand = new RelayCommand(this.TweetsTabButton);
            this.TweetsAndRepliesTabButtonCommand = new RelayCommand(this.TweetsAndRepliesTabButton);
            this.MediaTabButtonCommand = new RelayCommand(this.MediaTabButton);
            this.FavoritesTabButtonCommand = new RelayCommand(this.FavoritesTabButton);
        }

        /// <summary>
        /// 「ツイート」タブをクリックしたとき
        /// </summary>
        private void TweetsTabButton()
        {
            this.Tab = UserTimelineTab.Tweets;
            this.TimelineModel.ChangeUserTimelineTab(this.Tab);
        }

        /// <summary>
        /// 「ツイートと返信」タブをクリックしたとき
        /// </summary>
        private void TweetsAndRepliesTabButton()
        {
            this.Tab = UserTimelineTab.TweetsAndReplies;
            this.TimelineModel.ChangeUserTimelineTab(this.Tab);
        }

        /// <summary>
        /// 「メディア」タブをクリックしたとき
        /// </summary>
        private void MediaTabButton()
        {
            this.Tab = UserTimelineTab.Media;
            this.TimelineModel.ChangeUserTimelineTab(this.Tab);
        }

        /// <summary>
        /// 「いいね」タブをクリックしたとき
        /// </summary>
        private void FavoritesTabButton()
        {
            this.Tab = UserTimelineTab.Favorites;
            this.TimelineModel.ChangeUserTimelineTab(this.Tab);
        }

        #region Tab 変更通知プロパティ
        public UserTimelineTab Tab
        {
            get
            {
                return this._Tab;
            }
            set
            {
                this._Tab = value;
                this.RaisePropertyChanged(nameof(this.IsTweetsTab));
                this.RaisePropertyChanged(nameof(this.IsTweetsAndRepliesTab));
                this.RaisePropertyChanged(nameof(this.IsMediaTab));
                this.RaisePropertyChanged(nameof(this.IsFavoritesTab));
            }
        }
        private UserTimelineTab _Tab;
        #endregion

        #region IsTweetsTab 変更通知プロパティ
        public bool IsTweetsTab
        {
            get
            {
                return this.Tab == UserTimelineTab.Tweets;
            }
        }
        #endregion

        #region IsTweetsAndRepliesTab 変更通知プロパティ
        public bool IsTweetsAndRepliesTab
        {
            get
            {
                return this.Tab == UserTimelineTab.TweetsAndReplies;
            }
        }
        #endregion

        #region IsMediaTab 変更通知プロパティ
        public bool IsMediaTab
        {
            get
            {
                return this.Tab == UserTimelineTab.Media;
            }
        }
        #endregion

        #region IsFavoritesTab 変更通知プロパティ
        public bool IsFavoritesTab
        {
            get
            {
                return this.Tab == UserTimelineTab.Favorites;
            }
        }
        #endregion

        public ICommand TweetsTabButtonCommand { get; }
        public ICommand TweetsAndRepliesTabButtonCommand { get; }
        public ICommand MediaTabButtonCommand { get; }
        public ICommand FavoritesTabButtonCommand { get; }

        private TimelineModel TimelineModel;
    }
}
