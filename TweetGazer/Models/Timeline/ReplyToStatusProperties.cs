using CoreTweet;
using Livet;
using System.Windows;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class ReplyToStatusProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ReplyToStatusProperties()
        {
            this._Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="id">ツイートID</param>
        public ReplyToStatusProperties(TimelineModel timelineModel, long id)
        {
            this._Visibility = Visibility.Visible;
            this._TextVisibility = Visibility.Collapsed;
            this.ToggleOpenCommand = new RelayCommand(this.ToggleOpen);
            this.TimelineModel = timelineModel;
            this.Initialize(id);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="id">ツイートID</param>
        private async void Initialize(long id)
        {
            this.Initialize(await AccountTokens.ShowStatusAsync(this.TimelineModel.TokenSuffix, id));
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="status">ツイート内容</param>
        private void Initialize(Status status)
        {
            if (status == null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }

            this.User = new UserOverviewProperties(status.User);

            if (status.ExtendedTweet != null && status.ExtendedTweet.FullText != null)
                this.Text = status.ExtendedTweet.FullText;
            else if (status.Text != null)
                this.Text = status.Text;
            else if (status.FullText != null)
                this.Text = status.FullText;
            else
                this.Text = "";

            this.Id = status.Id;
        }

        /// <summary>
        /// リプライ先の開閉
        /// </summary>
        private void ToggleOpen()
        {
            if (this.TextVisibility == Visibility.Collapsed)
                this.TextVisibility = Visibility.Visible;
            else
                this.TextVisibility = Visibility.Collapsed;
        }

        #region Visibility 変更通知プロパティ
        public Visibility Visibility
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                this._Visibility = value;
                this.RaisePropertyChanged();
            }
        }
        private Visibility _Visibility;
        #endregion

        #region TextVisibility 変更通知プロパティ
        public Visibility TextVisibility
        {
            get
            {
                return this._TextVisibility;
            }
            set
            {
                this._TextVisibility = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.OpenButtonText));
            }
        }
        private Visibility _TextVisibility;
        #endregion

        #region User 変更通知プロパティ
        public UserOverviewProperties User
        {
            get
            {
                return this._User;
            }
            set
            {
                this._User = value;
                this.RaisePropertyChanged();
            }
        }
        private UserOverviewProperties _User;
        #endregion

        #region Text 変更通知プロパティ
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
                this.RaisePropertyChanged();
            }
        }
        private string _Text;
        #endregion

        public ICommand ToggleOpenCommand { get; }

        public string OpenButtonText
        {
            get
            {
                if (this.TextVisibility == Visibility.Collapsed)
                    return "開く";
                return "閉じる";
            }
        }

        public long? Id { get; set; }

        private TimelineModel TimelineModel;
    }
}