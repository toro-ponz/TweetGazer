using Livet;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace TweetGazer.Models.Timeline
{
    public class TimelineData : NotificationObject
    {
        #region CurrentPage 変更通知プロパティ
        public TimelinePageData CurrentPage
        {
            get
            {
                return this.Pages[this.PageSuffix];
            }
        }
        #endregion

        #region PageSuffix 変更通知プロパティ
        public int PageSuffix
        {
            get
            {
                return this._PageSuffix;
            }
            set
            {
                this._PageSuffix = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.CurrentPage));
                this.RaisePropertyChanged(nameof(this.IsVisibleBackButton));
            }
        }
        private int _PageSuffix;
        #endregion

        #region GridWidth 変更通知プロパティ
        [System.Xml.Serialization.XmlIgnore]
        public GridLength GridWidth
        {
            get
            {
                if (this._GridWidth == GridLength.Auto)
                {
                    return new GridLength(1000.0, GridUnitType.Star);
                }

                return this._GridWidth;
            }
            set
            {
                if (value == GridLength.Auto)
                {
                    this._GridWidth = new GridLength(1000.0, GridUnitType.Star);
                }
                else
                {
                    this._GridWidth = value;
                }

                this.RaisePropertyChanged();
            }
        }
        private GridLength _GridWidth;
        #endregion

        #region IsNotification 変更通知プロパティ
        public bool IsNotification
        {
            get
            {
                return this._IsNotification && Properties.Settings.Default.IsNotify;
            }
            set
            {
                this._IsNotification = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsNotification;
        #endregion

        #region IsNotificationSoundPlay 変更通知プロパティ
        public bool IsNotificationSoundPlay
        {
            get
            {
                return this._IsNotificationSoundPlay && Properties.Settings.Default.IsNotify;
            }
            set
            {
                this._IsNotificationSoundPlay = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsNotificationSoundPlay;
        #endregion

        #region IsVisibleBackButton 変更通知プロパティ
        public bool IsVisibleBackButton
        {
            get
            {
                if (this.PageSuffix == 0)
                {
                    return false;
                }

                return true;
            }
        }
        #endregion

        public List<TimelinePageData> Pages { get; set; }

        public string ScreenName { get; set; }
        public string GridWidthString
        {
            get
            {
                return this.GridWidth.Value.ToString(CultureInfo.CurrentCulture); ;
            }
            set
            {
                this.GridWidth = new GridLength(double.Parse(value, CultureInfo.CurrentCulture), GridUnitType.Star);
            }
        }

        public int TokenSuffix { get; set; }
        public int ColumnIndex { get; set; }

        public long? UserId { get; set; }
        public long? SinceId { get; set; }
    }
}
