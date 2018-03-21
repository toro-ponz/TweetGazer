using Livet;

namespace TweetGazer.Models.Timeline
{
    public class TimelinePageData : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimelinePageData()
        {
            this._IsVisibleRetweet = false;
            this._IsVisibleReply = false;
            this._IsVisibleImagesStatus = false;
            this._IsVisibleGifStatus = false;
            this._IsVisibleVideoStatus = false;
            this._IsVisibleLinkStatus = false;
        }

        #region Title 変更通知プロパティ
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
                this.RaisePropertyChanged();
            }
        }
        private string _Title;
        #endregion

        #region VerticalOffset 変更通知プロパティ
        public double VerticalOffset
        {
            get
            {
                return this._VerticalOffset;
            }
            set
            {
                this._VerticalOffset = value;
                this.RaisePropertyChanged();
            }
        }
        private double _VerticalOffset;
        #endregion

        #region IsVisibleRetweet 変更通知プロパティ
        public bool IsVisibleRetweet
        {
            get
            {
                return this._IsVisibleRetweet;
            }
            set
            {
                this._IsVisibleRetweet = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleRetweet;
        #endregion

        #region IsVisibleReply 変更通知プロパティ
        public bool IsVisibleReply
        {
            get
            {
                return this._IsVisibleReply;
            }
            set
            {
                this._IsVisibleReply = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleReply;
        #endregion

        #region IsVisibleImagesStatus 変更通知プロパティ
        public bool IsVisibleImagesStatus
        {
            get
            {
                return this._IsVisibleImagesStatus;
            }
            set
            {
                this._IsVisibleImagesStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleImagesStatus;
        #endregion

        #region IsVisibleGifStatus 変更通知プロパティ
        public bool IsVisibleGifStatus
        {
            get
            {
                return this._IsVisibleGifStatus;
            }
            set
            {
                this._IsVisibleGifStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleGifStatus;
        #endregion

        #region IsVisibleVideoStatus 変更通知プロパティ
        public bool IsVisibleVideoStatus
        {
            get
            {
                return this._IsVisibleVideoStatus;
            }
            set
            {
                this._IsVisibleVideoStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleVideoStatus;
        #endregion

        #region IsVisibleLinkStatus 変更通知プロパティ
        public bool IsVisibleLinkStatus
        {
            get
            {
                return this._IsVisibleLinkStatus;
            }
            set
            {
                this._IsVisibleLinkStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleLinkStatus;
        #endregion

        public TimelineType TimelineType { get; set; }
        public UserTimelineTab UserTimelineTab { get; set; }
        public SearchTimelineTab SearchTimelineTab { get; set; }

        public string ListName { get; set; }
        public long? ListNumber { get; set; }
        public string TargetUserName { get; set; }
        public long? TargetUserId { get; set; }
        public string TargetUserScreenName { get; set; }
        public string SearchText { get; set; }
        public long? DirectMessageId { get; set; }
    }
}
