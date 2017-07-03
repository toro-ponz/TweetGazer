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
            this._IsVisibleRetweet = true;
            this._IsVisibleReply = true;
            this._IsVisibleIncludeImagesStatus = true;
            this._IsVisibleIncludeGifStatus = true;
            this._IsVisibleIncludeVideoStatus = true;
            this._IsVisibleIncludeLinkStatus = true;
            this._IsVisibleOtherStatus = true;
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

        #region IsVisibleIncludeImagesStatus 変更通知プロパティ
        public bool IsVisibleIncludeImagesStatus
        {
            get
            {
                return this._IsVisibleIncludeImagesStatus;
            }
            set
            {
                this._IsVisibleIncludeImagesStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleIncludeImagesStatus;
        #endregion

        #region IsVisibleIncludeGifStatus 変更通知プロパティ
        public bool IsVisibleIncludeGifStatus
        {
            get
            {
                return this._IsVisibleIncludeGifStatus;
            }
            set
            {
                this._IsVisibleIncludeGifStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleIncludeGifStatus;
        #endregion

        #region IsVisibleIncludeVideoStatus 変更通知プロパティ
        public bool IsVisibleIncludeVideoStatus
        {
            get
            {
                return this._IsVisibleIncludeVideoStatus;
            }
            set
            {
                this._IsVisibleIncludeVideoStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleIncludeVideoStatus;
        #endregion

        #region IsVisibleIncludeLinkStatus 変更通知プロパティ
        public bool IsVisibleIncludeLinkStatus
        {
            get
            {
                return this._IsVisibleIncludeLinkStatus;
            }
            set
            {
                this._IsVisibleIncludeLinkStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleIncludeLinkStatus;
        #endregion

        #region IsVisibleOtherStatus 変更通知プロパティ
        public bool IsVisibleOtherStatus
        {
            get
            {
                return this._IsVisibleOtherStatus;
            }
            set
            {
                this._IsVisibleOtherStatus = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleOtherStatus;
        #endregion

        public TimelineType TimelineType { get; set; }
        public UserTimelineTab UserTimelineTab { get; set; }

        public string ListName { get; set; }
        public long? ListNumber { get; set; }
        public string TargetUserName { get; set; }
        public long? TargetUserId { get; set; }
        public string TargetUserScreenName { get; set; }
        public string SearchText { get; set; }
        public long? DirectMessageId { get; set; }
    }
}