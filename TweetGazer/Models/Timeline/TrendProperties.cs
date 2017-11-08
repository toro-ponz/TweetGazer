using CoreTweet;
using Livet;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class TrendProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="trend">トレンドデータ</param>
        /// <param name="rank">順位</param>
        public TrendProperties(TimelineModel timelineModel, Trend trend, int rank)
        {
            this.TimelineModel = timelineModel;
            if (trend != null)
            {
                this._TrendName = trend.Name;
                this.TrendQuery = trend.Query;
                if (trend.TweetVolume != null)
                {
                    this._TrendCount = (int)trend.TweetVolume;
                }
            }
            this.TrendRank = rank;

            this.SelectCommand = new RelayCommand(this.Select);
        }

        /// <summary>
        /// トレンドを選んだとき
        /// </summary>
        private void Select()
        {
            this.TimelineModel.ShowSearchTimeline(this.TrendName);
        }

        #region TrendCountVisibility 変更通知プロパティ
        public Visibility TrendCountVisibility
        {
            get
            {
                if (this.TrendCount == null)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        #endregion

        #region TrendName 変更通知プロパティ
        public string TrendName
        {
            get
            {
                return this._TrendName;
            }
            set
            {
                this._TrendName = value;
                this.RaisePropertyChanged();
            }
        }
        private string _TrendName;
        #endregion

        #region TrendCount 変更通知プロパティ
        public string TrendCount
        {
            get
            {
                if (this._TrendCount == null || this._TrendCount == 0)
                {
                    return null;
                }
                else
                {
                    return ((int)this._TrendCount).ToString("N0", CultureInfo.CurrentCulture) + "件のツイート";
                }
            }
            set
            {
                this._TrendCount = int.Parse(value, CultureInfo.CurrentCulture);
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TrendCountVisibility));
            }
        }
        private int? _TrendCount;
        #endregion

        public ICommand SelectCommand { get; }

        public string TrendQuery { get; }

        public int TrendRank { get; }

        private TimelineModel TimelineModel;
    }
}
