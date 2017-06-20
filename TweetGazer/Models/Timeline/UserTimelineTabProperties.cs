using Livet;
using System.Windows;
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
                this.RaisePropertyChanged(nameof(TweetsTabSelectedVisibility));
                this.RaisePropertyChanged(nameof(TweetsAndRepliesTabSelectedVisibility));
                this.RaisePropertyChanged(nameof(MediaTabSelectedVisibility));
                this.RaisePropertyChanged(nameof(FavoritesTabSelectedVisibility));
            }
        }
        private UserTimelineTab _Tab;
        #endregion

        #region TweetsTabSelectedVisibility 変更通知プロパティ
        public Visibility TweetsTabSelectedVisibility
        {
            get
            {
                if (this.Tab == UserTimelineTab.Tweets)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        #endregion

        #region TweetsAndRepliesTabSelectedVisibility 変更通知プロパティ
        public Visibility TweetsAndRepliesTabSelectedVisibility
        {
            get
            {
                if (this.Tab == UserTimelineTab.TweetsAndReplies)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        #endregion

        #region MediaTabSelectedVisibility 変更通知プロパティ
        public Visibility MediaTabSelectedVisibility
        {
            get
            {
                if (this.Tab == UserTimelineTab.Media)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        #endregion

        #region FavoritesTabSelectedVisibility 変更通知プロパティ
        public Visibility FavoritesTabSelectedVisibility
        {
            get
            {
                if (this.Tab == UserTimelineTab.Favorites)
                    return Visibility.Visible;
                return Visibility.Collapsed;
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