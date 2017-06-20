using Livet;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models;
using TweetGazer.Models.ShowDialongs;
using TweetGazer.Models.Timeline;

namespace TweetGazer.ViewModels.ShowDialogs
{
    public class ShowStatusViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">遷移元のTimelineModel</param>
        /// <param name="id">ツイートID</param>
        public ShowStatusViewModel(TimelineModel timelineModel, long id)
        {
            this.ShowStatus = new ShowStatusModel(timelineModel, id);

            this.Statuses = this.ShowStatus.Statuses;

            this.ReadMoreRepliesCommand = new RelayCommand<long?>(this.ReadMoreReplies);
            this.ReadMoreRepliesToMainStatusCommand = new RelayCommand<SearchRepliesProperties>(this.ReadMoreRepliesToMainStatus);
        }

        /// <summary>
        /// 指定ツイートの返信先ツイートを読み込む
        /// </summary>
        /// <param name="id">ツイートID</param>
        private void ReadMoreReplies(long? id)
        {
            this.ShowStatus.ReadMoreReplies(id);
        }

        /// <summary>
        /// 元ツイートに対する返信を取得する
        /// </summary>
        /// <param name="data">返信の取得情報</param>
        private void ReadMoreRepliesToMainStatus(SearchRepliesProperties data)
        {
            this.ShowStatus.ReadMoreRepliesToMainStatus(data);
        }

        public ICommand ReadMoreRepliesCommand { get; }
        public ICommand ReadMoreRepliesToMainStatusCommand { get; }

        public ObservableCollection<TimelineItemProperties> Statuses { get; }

        private ShowStatusModel ShowStatus;
    }
}