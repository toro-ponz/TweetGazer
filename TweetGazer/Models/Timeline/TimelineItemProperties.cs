using CoreTweet;
using TweetGazer.ViewModels;

namespace TweetGazer.Models.Timeline
{
    public class TimelineItemProperties
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimelineItemProperties()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="status">ツイート内容</param>
        /// <param name="type">ツイートの表示タイプ</param>
        public TimelineItemProperties(TimelineModel timelineModel, Status status, StatusType type = StatusType.Timeline)
        {
            Type = TimelineItemType.Status;
            StatusProperties = new StatusProperties(timelineModel, status, type);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="user">ユーザーデータ</param>
        public TimelineItemProperties(TimelineModel timelineModel, User user)
        {
            Type = TimelineItemType.UserOverview;
            UserProperties = new UserProperties(timelineModel, user);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="trend">トレンドデータ</param>
        /// <param name="rank">順位</param>
        public TimelineItemProperties(TimelineModel timelineModel, Trend trend, int rank)
        {
            Type = TimelineItemType.Trend;
            TrendProperties = new TrendProperties(timelineModel, trend, rank);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="tab">タブ</param>
        public TimelineItemProperties(TimelineModel timelineModel, UserTimelineTab tab)
        {
            Type = TimelineItemType.TabButton;
            TabProperties = new UserTimelineTabProperties(timelineModel, tab);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="type">ロードタイプ</param>
        /// <param name="parameter">パラメーター</param>
        public TimelineItemProperties(TimelineModel timelineModel, LoadingType type, object parameter = null)
        {
            Type = TimelineItemType.Button;
            LoadingProperties = new LoadingProperties(timelineModel, type, parameter);
        }

        public TimelineItemType Type { get; }

        public StatusProperties StatusProperties { get; }
        public UserProperties UserProperties { get; }
        public TrendProperties TrendProperties { get; }
        public UserTimelineTabProperties TabProperties { get; }
        public LoadingProperties LoadingProperties { get; }
    }

    public enum TimelineItemType
    {
        Status,
        UserOverview,
        Trend,
        TabButton,
        Button
    }
}