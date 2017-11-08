using CoreTweet;

namespace TweetGazer.Models.Timeline
{
    public class TimelineItemProperties
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimelineItemProperties()
        {
            this.TimelineItemType = TimelineItemType.Undefined;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="status">ツイート内容</param>
        /// <param name="type">ツイートの表示タイプ</param>
        public TimelineItemProperties(TimelineModel timelineModel, Status status, StatusType type = StatusType.Timeline)
        {
            this.TimelineItemType = TimelineItemType.Status;
            this.StatusProperties = new StatusProperties(timelineModel, status, type);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="user">ユーザーデータ</param>
        public TimelineItemProperties(TimelineModel timelineModel, User user)
        {
            this.TimelineItemType = TimelineItemType.UserOverview;
            this.UserProperties = new UserProperties(timelineModel, user);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="trend">トレンドデータ</param>
        /// <param name="rank">順位</param>
        public TimelineItemProperties(TimelineModel timelineModel, Trend trend, int rank)
        {
            this.TimelineItemType = TimelineItemType.Trend;
            this.TrendProperties = new TrendProperties(timelineModel, trend, rank);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="tab">タブ</param>
        public TimelineItemProperties(TimelineModel timelineModel, UserTimelineTab tab)
        {
            this.TimelineItemType = TimelineItemType.UserTimelineTab;
            this.UserTimelineTabProperties = new UserTimelineTabProperties(timelineModel, tab);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="tab">タブ</param>
        public TimelineItemProperties(TimelineModel timelineModel, SearchTimelineTab tab)
        {
            this.TimelineItemType = TimelineItemType.SearchTimelineTab;
            this.SearchTimelineTabProperties = new SearchTimelineTabProperties(timelineModel, tab);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="type">ロードタイプ</param>
        /// <param name="parameter">パラメーター</param>
        public TimelineItemProperties(TimelineModel timelineModel, LoadingType type, object parameter = null)
        {
            this.TimelineItemType = TimelineItemType.Button;
            this.LoadingProperties = new LoadingProperties(timelineModel, type, parameter);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel"></param>
        /// <param name="sentUser"></param>
        /// <param name="receiveUser"></param>
        /// <param name="type"></param>
        /// <param name="parameter"></param>
        public TimelineItemProperties(TimelineModel timelineModel, UserOverviewProperties sentUser, UserOverviewProperties receiveUser, NotificationPropertiesType type, object parameter = null, long? id = null)
        {
            this.TimelineItemType = TimelineItemType.Notification;
            this.NotificationProperties = new NotificationProperties(timelineModel, sentUser, receiveUser, type, parameter, id);
        }

        public TimelineItemType TimelineItemType { get; }

        public StatusProperties StatusProperties { get; }
        public UserProperties UserProperties { get; }
        public TrendProperties TrendProperties { get; }
        public UserTimelineTabProperties UserTimelineTabProperties { get; }
        public SearchTimelineTabProperties SearchTimelineTabProperties { get; }
        public LoadingProperties LoadingProperties { get; }
        public NotificationProperties NotificationProperties { get; }
    }

    public enum TimelineItemType
    {
        Undefined,
        Status,
        UserOverview,
        Trend,
        UserTimelineTab,
        SearchTimelineTab,
        Notification,
        Button
    }
}
