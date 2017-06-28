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
            this._Title = this.Timeline.Data.CurrentPage.Title;
            this.ScreenName = this.Timeline.ScreenName;
            this._IsVisibleSettings = this.Timeline.IsVisibleSettings;

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Timeline, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Timeline.TimelineItems):
                            this.RaisePropertyChanged(() => this.TimelineItems);
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
                            this.IsVisibleSettings = this.Timeline.IsVisibleSettings;
                            break;
                        case nameof(this.Timeline.IsVisibleRetweet):
                            this.RaisePropertyChanged(() => this.IsVisibleRetweet);
                            break;
                        case nameof(this.Timeline.IsVisibleReply):
                            this.RaisePropertyChanged(() => this.IsVisibleReply);
                            break;
                        case nameof(this.Timeline.IsVisibleIncludeImagesStatus):
                            this.RaisePropertyChanged(() => this.IsVisibleIncludeImagesStatus);
                            break;
                        case nameof(this.Timeline.IsVisibleIncludeVideoStatus):
                            this.RaisePropertyChanged(() => this.IsVisibleIncludeVideoStatus);
                            break;
                        case nameof(this.Timeline.IsVisibleIncludeLinkStatus):
                            this.RaisePropertyChanged(() => this.IsVisibleIncludeLinkStatus);
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
                    return Visibility.Visible;
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
                return this._IsVisibleSettings;
            }
            set
            {
                this._IsVisibleSettings = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleSettings;
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
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TimelineItems));
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
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TimelineItems));
            }
        }
        #endregion

        #region IsVisibleIncludeImagesStatus 変更通知プロパティ
        public bool IsVisibleIncludeImagesStatus
        {
            get
            {
                return this.Timeline.IsVisibleIncludeImagesStatus;
            }
            set
            {
                this.Timeline.IsVisibleIncludeImagesStatus = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TimelineItems));
            }
        }
        #endregion

        #region IsVisibleIncludeGifStatus 変更通知プロパティ
        public bool IsVisibleIncludeGifStatus
        {
            get
            {
                return this.Timeline.IsVisibleIncludeGifStatus;
            }
            set
            {
                this.Timeline.IsVisibleIncludeGifStatus = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TimelineItems));
            }
        }
        #endregion

        #region IsVisibleIncludeVideoStatus 変更通知プロパティ
        public bool IsVisibleIncludeVideoStatus
        {
            get
            {
                return this.Timeline.IsVisibleIncludeVideoStatus;
            }
            set
            {
                this.Timeline.IsVisibleIncludeVideoStatus = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TimelineItems));
            }
        }
        #endregion

        #region IsVisibleIncludeLinkStatus 変更通知プロパティ
        public bool IsVisibleIncludeLinkStatus
        {
            get
            {
                return this.Timeline.IsVisibleIncludeLinkStatus;
            }
            set
            {
                this.Timeline.IsVisibleIncludeLinkStatus = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TimelineItems));
            }
        }
        #endregion

        #region IsVisibleOtherStatus 変更通知プロパティ
        public bool IsVisibleOtherStatus
        {
            get
            {
                return this.Timeline.IsVisibleOtherStatus;
            }
            set
            {
                this.Timeline.IsVisibleOtherStatus = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.TimelineItems));
            }
        }
        #endregion

        public ICommand UpdateCommand { get; }

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