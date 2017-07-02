using CoreTweet;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TweetGazer.Common;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Models.ShowDialongs
{
    public class ShowStatusModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="id">ツイートのID</param>
        public ShowStatusModel(TimelineModel timelineModel, long id)
        {
            this.TimelineModel = timelineModel;
            this.Id = id;

            this.Statuses = new ObservableCollection<TimelineItemProperties>();
            BindingOperations.EnableCollectionSynchronization(this.Statuses, new object());
            this.IsLoadingReplies = false;
            this.IsLoadingRepliesToMainStatus = false;

            this.SetStatus();
        }

        /// <summary>
        /// リプライ先を読み込む
        /// </summary>
        /// <param name="id">リプライ先ツイートID</param>
        public async void ReadMoreReplies(long? id)
        {
            if (this.IsLoadingReplies)
                return;

            this.IsLoadingReplies = true;

            while (true)
            {
                if (this.IsLoadingRepliesToMainStatus)
                {
                    await Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(3000);
                    });
                }
                else
                    break;
            }

            long? replyId = id;
            for (int i = 0; i < 10; i++)
            {
                if (replyId == null)
                {
                    this.Statuses.First().LoadingProperties.Visibility = Visibility.Collapsed;
                    break;
                }

                StatusResponse mainStatusReplyStatus = null;
                try
                {
                    mainStatusReplyStatus = await AccountTokens.ShowStatusAsync(this.TimelineModel.TokenSuffix, (long)replyId);
                }
                catch (Exception e)
                {
                    Debug.Write(e);
                    this.Statuses.First().LoadingProperties.Visibility = Visibility.Collapsed;
                    break;
                }

                if (mainStatusReplyStatus == null)
                {
                    this.Statuses.First().LoadingProperties.Visibility = Visibility.Collapsed;
                    break;
                }

                var mainStatusReplyStatusProperties = new TimelineItemProperties(this.TimelineModel, mainStatusReplyStatus, StatusType.IndividualOther);
                this.Statuses.Insert(1, mainStatusReplyStatusProperties);
                replyId = mainStatusReplyStatus.InReplyToStatusId;
            }

            this.Statuses.RemoveAt(0);

            if (replyId != null)
                this.Statuses.Insert(0, new TimelineItemProperties(this.TimelineModel, LoadingType.ReadMoreRepliesButton, replyId));

            this.IsLoadingReplies = false;
        }

        /// <summary>
        /// 元ツイートに対する返信を取得する
        /// </summary>
        /// <param name="data">返信の取得情報</param>
        public async void ReadMoreRepliesToMainStatus(SearchRepliesProperties data)
        {
            if (this.IsLoadingRepliesToMainStatus)
                return;

            this.IsLoadingRepliesToMainStatus = true;

            while (true)
            {
                if (this.IsLoadingReplies)
                {
                    await Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(3000);
                    });
                    if (this.IsLoadingRepliesToMainStatus)
                        this.IsLoadingRepliesToMainStatus = false;
                }
                else
                    break;
            }

            this.IsLoadingRepliesToMainStatus = true;

            var searchQuery = "to:" + data.ScreenName;
            var mentionStatuses = await AccountTokens.LoadSearchTimelineAsync(this.TimelineModel.TokenSuffix, searchQuery, data.MaxId, data.SinceId, "false");

            while (true)
            {
                if (mentionStatuses != null && mentionStatuses.Count != 0)
                {
                    var replyIds = mentionStatuses.Where(x => x.InReplyToStatusId == Id).Select(x => x.Id).ToList();
                    var suffix = this.GetSuffix();

                    if (replyIds.Count != 0)
                    {
                        foreach (var mentionStatus in (await AccountTokens.LookupStatusAsync(this.TimelineModel.TokenSuffix, replyIds)).OrderByDescending(x => x.Id))
                        {
                            if (suffix != -1)
                                this.Statuses.Insert(suffix + 1, new TimelineItemProperties(this.TimelineModel, mentionStatus, StatusType.IndividualOther));
                        }
                    }

                    mentionStatuses = await AccountTokens.LoadSearchTimelineAsync(this.TimelineModel.TokenSuffix, searchQuery, mentionStatuses.Last().Id - 1, data.SinceId, "false");
                }
                else
                {
                    break;
                }
            }

            for (int i = 1; i < this.Statuses.Count; i++)
            {
                if (this.Statuses[i].Type == TimelineItemType.Button && this.Statuses[i].LoadingProperties != null)
                    this.Statuses.RemoveAt(i);
            }
            this.IsLoadingRepliesToMainStatus = false;
        }

        /// <summary>
        /// 元ツイート等の追加
        /// </summary>
        private async void SetStatus()
        {
            // 元ツイートの取得
            var mainStatus = await AccountTokens.ShowStatusAsync(this.TimelineModel.TokenSuffix, this.Id);
            if (mainStatus == null)
                return;
            var mainProperties = new TimelineItemProperties(this.TimelineModel, mainStatus, StatusType.IndividualMain);
            this.Statuses.Add(mainProperties);

            // リプライ先がある場合ProgressRingを追加
            if (mainStatus.InReplyToStatusId != null)
                this.Statuses.Insert(0, new TimelineItemProperties(this.TimelineModel, LoadingType.ReadMoreReplies, mainStatus.InReplyToStatusId));

            // 返信を取得するProgressRingを追加
            this.Statuses.Add(new TimelineItemProperties(this.TimelineModel, LoadingType.ReadMoreRepliesToMainStatus, new SearchRepliesProperties()
            {
                Id = mainStatus.Id,
                ScreenName = mainStatus.User.ScreenName,
                MaxId = null,
                SinceId = mainStatus.Id + 1
            }));
        }

        /// <summary>
        /// ProgressRingの位置を取得する
        /// </summary>
        /// <returns>ProgressRingの位置</returns>
        private int GetSuffix()
        {
            for (int i = 1; i < this.Statuses.Count; i++)
            {
                if (this.Statuses[i].Type == TimelineItemType.Button && this.Statuses[i].LoadingProperties != null)
                {
                    return i;
                }
            }
            return -1;
        }

        public ObservableCollection<TimelineItemProperties> Statuses { get; }

        private long Id;
        private bool IsLoadingReplies;
        private bool IsLoadingRepliesToMainStatus;

        private TimelineModel TimelineModel;
    }
}