using Livet;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class SearchTimelineTabProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="tab">タブ</param>
        public SearchTimelineTabProperties(TimelineModel timelineModel, SearchTimelineTab tab)
        {
            this.TimelineModel = timelineModel;
            this.Tab = tab;

            this.TopCommand = new RelayCommand(this.Top);
            this.LatestCommand = new RelayCommand(this.Latest);
            this.StreamingCommand = new RelayCommand(this.Streaming);
        }

        /// <summary>
        /// トップボタンを押したとき
        /// </summary>
        private void Top()
        {
            this.Tab = SearchTimelineTab.Top;
            this.TimelineModel.ChangeSearchTimelineTab(this.Tab);
        }

        /// <summary>
        /// 最新ボタンを押したとき
        /// </summary>
        private void Latest()
        {
            this.Tab = SearchTimelineTab.Latest;
            this.TimelineModel.ChangeSearchTimelineTab(this.Tab);
        }

        /// <summary>
        /// ストリーミングボタンを押したとき
        /// </summary>
        private void Streaming()
        {
            this.Tab = SearchTimelineTab.Streaming;
            this.TimelineModel.ChangeSearchTimelineTab(this.Tab);
        }

        #region Tab 変更通知プロパティ
        public SearchTimelineTab Tab
        {
            get
            {
                return this._Tab;
            }
            set
            {
                this._Tab = value;
                this.RaisePropertyChanged(nameof(this.IsTop));
                this.RaisePropertyChanged(nameof(this.IsLatest));
                this.RaisePropertyChanged(nameof(this.IsStreaming));
            }
        }
        private SearchTimelineTab _Tab;
        #endregion

        #region IsTop 変更通知プロパティ
        public bool IsTop
        {
            get
            {
                if (this.Tab == SearchTimelineTab.Top)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region IsLatest 変更通知プロパティ
        public bool IsLatest
        {
            get
            {
                if (this.Tab == SearchTimelineTab.Latest)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region IsTop 変更通知プロパティ
        public bool IsStreaming
        {
            get
            {
                if (this.Tab == SearchTimelineTab.Streaming)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        public ICommand TopCommand { get; }
        public ICommand LatestCommand { get; }
        public ICommand StreamingCommand { get; }

        private TimelineModel TimelineModel;
    }
}
