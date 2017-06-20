using Livet;
using System.Windows;

namespace TweetGazer.Models.Timeline
{
    public class TimelinePageData : NotificationObject
    {
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