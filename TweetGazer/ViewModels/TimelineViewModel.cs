using CoreTweet.Streaming;
using Livet;
using Livet.EventListeners;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TweetGazer.Models;
using TweetGazer.Models.Timeline;

namespace TweetGazer.ViewModels
{
    public class TimelineViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">タイムラインデータ</param>
        public TimelineViewModel(TimelineData data)
        {
            this.Timeline = new TimelineModel(data);

            this.TimelineNotice = this.Timeline.TimelineNotice;
            this._Message = this.Timeline.Message;
            this._Title = this.Timeline.Data.CurrentPage.Title;
            this.ScreenName = this.Timeline.ScreenName;

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Timeline, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Timeline.Message):
                            this.Message = this.Timeline.Message;
                            break;
                        case nameof(this.Timeline.ProgressRingVisibility):
                            this.RaisePropertyChanged(() => this.ProgressRingVisibility);
                            break;
                        case nameof(this.Timeline.Title):
                            this.Title = this.Timeline.Title;
                            break;
                        case nameof(this.Timeline.VerticalOffset):
                            this.RaisePropertyChanged(() => this.VerticalOffset);
                            this.RaisePropertyChanged(() => this.UpButtonVisibility);
                            break;
                        case nameof(this.Timeline.IsVisibleSettings):
                            this.RaisePropertyChanged(() => this.IsVisibleSettings);
                            break;
                        case nameof(this.Timeline.IsFiltered):
                            this.RaisePropertyChanged(() => this.IsFiltered);
                            break;
                        case nameof(this.Timeline.IsVisibleRetweet):
                            this.RaisePropertyChanged(() => this.IsVisibleRetweet);
                            break;
                        case nameof(this.Timeline.IsVisibleReply):
                            this.RaisePropertyChanged(() => this.IsVisibleReply);
                            break;
                        case nameof(this.Timeline.IsVisibleImagesStatus):
                            this.RaisePropertyChanged(() => this.IsVisibleImagesStatus);
                            break;
                        case nameof(this.Timeline.IsVisibleGifStatus):
                            this.RaisePropertyChanged(() => this.IsVisibleGifStatus);
                            break;
                        case nameof(this.Timeline.IsVisibleVideoStatus):
                            this.RaisePropertyChanged(() => this.IsVisibleVideoStatus);
                            break;
                        case nameof(this.Timeline.IsVisibleLinkStatus):
                            this.RaisePropertyChanged(() => this.IsVisibleLinkStatus);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Timeline.Data, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Timeline.Data.IsNotification):
                            this.RaisePropertyChanged(() => this.IsNotification);
                            break;
                        case nameof(this.Timeline.Data.IsNotificationSoundPlay):
                            this.RaisePropertyChanged(() => this.IsNotificationSoundPlay);
                            break;
                        case nameof(this.Timeline.Data.IsVisibleBackButton):
                            this.RaisePropertyChanged(() => this.IsVisibleBackButton);
                            break;
                        case nameof(this.Timeline.Data.PageSuffix):
                            this.RaisePropertyChanged(() => this.IsVisibleBackButton);
                            this.RaisePropertyChanged(() => this.VerticalOffset);
                            this.RaisePropertyChanged(() => this.UpButtonVisibility);
                            this.RaisePropertyChanged(() => this.IsFiltered);
                            this.RaisePropertyChanged(() => this.IsVisibleRetweet);
                            this.RaisePropertyChanged(() => this.IsVisibleReply);
                            this.RaisePropertyChanged(() => this.IsVisibleImagesStatus);
                            this.RaisePropertyChanged(() => this.IsVisibleGifStatus);
                            this.RaisePropertyChanged(() => this.IsVisibleVideoStatus);
                            this.RaisePropertyChanged(() => this.IsVisibleLinkStatus);
                            break;
                        case nameof(this.Timeline.Data.GridWidth):
                            this.RaisePropertyChanged(() => this.GridWidth);
                            break;
                    }
                })
            );

            this.UpdateCommand = new Common.RelayCommand(async () =>
            {
                await this.Update();
            });
            this.ResetFilterCommand = new Common.RelayCommand(this.ResetFilter);

            this.CompositeDisposable.Add(this.Timeline);
        }
        
        /// <summary>
        /// TLを最初のページの最上部にする
        /// </summary>
        public void Home()
        {
            this.Timeline.Home();
        }

        /// <summary>
        /// 更新する
        /// </summary>
        public async Task Update()
        {
            await this.Timeline.Update();
        }

        /// <summary>
        /// 1つ前のページへ戻る
        /// </summary>
        public void Back()
        {
            this.Timeline.Back();
        }

        /// <summary>
        /// 最上部へスクロール
        /// </summary>
        public void Up()
        {
            this.Timeline.Up();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>
        /// 設定画面の開閉
        /// </summary>
        public void ToggleOpenSettings()
        {
            this.Timeline.ToggleOpenSettings();
        }

        /// <summary>
        /// フィルターのリセット
        /// </summary>
        public void ResetFilter()
        {
            this.Timeline.ResetFilter();
        }

        /// <summary>
        /// タイムラインデータをシリアライズする
        /// </summary>
        /// <returns>タイムラインデータ</returns>
        public string Serialize()
        {
            return this.Timeline.Serialize();
        }

        /// <summary>
        /// カラム番号を(再)設定する
        /// </summary>
        /// <param name="columnIndex">カラム番号</param>
        public void SetColumnIndex(int columnIndex)
        {
            this.Timeline.ColumnIndex = columnIndex;
        }

        /// <summary>
        /// カラム幅を設定する
        /// </summary>
        /// <param name="width">カラム幅</param>
        public void SetGridWidth(GridLength width)
        {
            this.GridWidth = width;
        }

        /// <summary>
        /// ストリーミングで流れてきたツイートを流す
        /// </summary>
        /// <param name="statusMessage">ツイート</param>
        /// <param name="userId">受け取ったユーザーID</param>
        public void StreamStatusMessage(StatusMessage statusMessage, long? userId)
        {
            this.Timeline.StreamStatusMessage(statusMessage, userId);
        }

        /// <summary>
        /// ストリーミングで流れてきた削除されたツイートを削除する
        /// </summary>
        /// <param name="statusMessage">削除されたツイート</param>
        /// <param name="userId">受け取ったユーザーID</param>
        public void StreamDeleteMessage(DeleteMessage deleteMessage, long? userId)
        {
            this.Timeline.StreamDeleteMessage(deleteMessage, userId);
        }

        /// <summary>
        /// タイムラインをクリアする
        /// </summary>
        public void Clear()
        {
            this.Timeline.Clear();
        }

        #region UpButtonVisibility 変更通知プロパティ
        public Visibility UpButtonVisibility
        {
            get
            {
                if (this.VerticalOffset > 0.0d)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }
        #endregion

        #region ProgressRingVisibility 変更通知プロパティ
        public Visibility ProgressRingVisibility
        {
            get
            {
                return this.Timeline.ProgressRingVisibility;
            }
        }
        #endregion

        #region Message 変更通知プロパティ
        public string Message
        {
            get
            {
                return this._Message;
            }
            set
            {
                this._Message = value;
                this.RaisePropertyChanged();
            }
        }
        private string _Message;
        #endregion

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

        #region IsNotification 変更通知プロパティ
        public bool IsNotification
        {
            get
            {
                return this.Timeline.Data.IsNotification;
            }
            set
            {
                this.Timeline.Data.IsNotification = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsNotificationSoundPlay 変更通知プロパティ
        public bool IsNotificationSoundPlay
        {
            get
            {
                return this.Timeline.Data.IsNotificationSoundPlay;
            }
            set
            {
                this.Timeline.Data.IsNotificationSoundPlay = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsVisibleBackButton 変更通知プロパティ
        public bool IsVisibleBackButton
        {
            get
            {
                return this.Timeline.Data.IsVisibleBackButton;
            }
        }
        #endregion

        #region VerticalOffset 変更通知プロパティ
        public double VerticalOffset
        {
            get
            {
                return this.Timeline.VerticalOffset;
            }
            set
            {
                this.Timeline.VerticalOffset = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.UpButtonVisibility));
            }
        }
        #endregion

        #region GridWidth 変更通知プロパティ
        public GridLength GridWidth
        {
            get
            {
                return this.Timeline.Data.GridWidth;
            }
            set
            {
                this.Timeline.Data.GridWidth = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsVisibleSettings 変更通知プロパティ
        public bool IsVisibleSettings
        {
            get
            {
                return this.Timeline.IsVisibleSettings;
            }
        }
        #endregion

        #region IsFiltered 変更通知プロパティ
        public bool IsFiltered
        {
            get
            {
                return this.Timeline.IsFiltered;
            }
        }
        #endregion

        #region IsVisibleRetweet 変更通知プロパティ
        public bool IsVisibleRetweet
        {
            get
            {
                return this.Timeline.IsVisibleRetweet;
            }
            set
            {
                this.Timeline.IsVisibleRetweet = value;
            }
        }
        #endregion

        #region IsVisibleReply 変更通知プロパティ
        public bool IsVisibleReply
        {
            get
            {
                return this.Timeline.IsVisibleReply;
            }
            set
            {
                this.Timeline.IsVisibleReply = value;
            }
        }
        #endregion

        #region IsVisibleImagesStatus 変更通知プロパティ
        public bool IsVisibleImagesStatus
        {
            get
            {
                return this.Timeline.IsVisibleImagesStatus;
            }
            set
            {
                this.Timeline.IsVisibleImagesStatus = value;
            }
        }
        #endregion

        #region IsVisibleGifStatus 変更通知プロパティ
        public bool IsVisibleGifStatus
        {
            get
            {
                return this.Timeline.IsVisibleGifStatus;
            }
            set
            {
                this.Timeline.IsVisibleGifStatus = value;
            }
        }
        #endregion

        #region IsVisibleVideoStatus 変更通知プロパティ
        public bool IsVisibleVideoStatus
        {
            get
            {
                return this.Timeline.IsVisibleVideoStatus;
            }
            set
            {
                this.Timeline.IsVisibleVideoStatus = value;
            }
        }
        #endregion

        #region IsVisibleLinkStatus 変更通知プロパティ
        public bool IsVisibleLinkStatus
        {
            get
            {
                return this.Timeline.IsVisibleLinkStatus;
            }
            set
            {
                this.Timeline.IsVisibleLinkStatus = value;
            }
        }
        #endregion

        public ICommand UpdateCommand { get; }
        public ICommand ResetFilterCommand { get; }

        public IEnumerable<TimelineItemProperties> TimelineItems
        {
            get
            {
                return this.Timeline.TimelineItems;
            }
        }

        public ObservableCollection<TimelineNotice> TimelineNotice { get; }

        public string ScreenName { get; }

        private TimelineModel Timeline;
    }
}
