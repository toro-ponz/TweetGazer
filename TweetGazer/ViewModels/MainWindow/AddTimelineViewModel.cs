using Livet;
using Livet.EventListeners;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class AddTimelineViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mainWindowViewModel"></param>
        public AddTimelineViewModel(MainWindowViewModel mainWindowViewModel) : base()
        {
            this.AddTimeline = new AddTimeline(mainWindowViewModel);
            this.ExtraGrid = this.AddTimeline.ExtraGrid;

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.AddTimeline, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.AddTimeline.ScreenNames):
                            this.RaisePropertyChanged(() => this.ScreenNames);
                            break;
                        case nameof(this.AddTimeline.TokenSuffix):
                            this.RaisePropertyChanged(() => this.TokenSuffix);
                            this.RaisePropertyChanged(() => this.ProfileImageUrl);
                            break;
                        case nameof(this.AddTimeline.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpen);
                            break;
                    }
                })
            );

            this.CloseCommand = new RelayCommand(this.ToggleOpen);
        }

        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        public void ToggleOpen()
        {
            this.AddTimeline.ToggleOpen();
        }

        /// <summary>
        /// ホームタイムラインの追加
        /// </summary>
        public void AddHomeTimeline()
        {
            this.AddTimeline.AddHomeTimeline();
        }

        /// <summary>
        /// 自分のユーザータイムラインの追加
        /// </summary>
        public void AddOwnTimeline()
        {
            this.AddTimeline.AddOwnTimeline();
        }

        /// <summary>
        /// ユーザータイムラインの追加
        /// </summary>
        public void AddUserTimeline()
        {
            this.AddTimeline.AddUserTimeline();
        }

        /// <summary>
        /// リストタイムラインの追加
        /// </summary>
        public void AddListTimeline()
        {
            this.AddTimeline.AddListTimeline();
        }

        /// <summary>
        /// メンションタイムラインの追加
        /// </summary>
        public void AddMentionTimeline()
        {
            this.AddTimeline.AddMentionTimeline();
        }

        /// <summary>
        /// お気に入りタイムラインの追加
        /// </summary>
        public void AddFavoriteTimeline()
        {
            this.AddTimeline.AddFavoriteTimeline();
        }

        /// <summary>
        /// 通知タイムラインの追加
        /// </summary>
        public void AddNoticeTimeline()
        {
            this.AddTimeline.AddNoticeTimeline();
        }

        /// <summary>
        /// トレンドタイムラインの追加
        /// </summary>
        public void AddTrendTimeline()
        {
            this.AddTimeline.AddTrendTimeline();
        }

        /// <summary>
        /// 検索タイムラインの追加
        /// </summary>
        public void AddSearchTimeline()
        {
            this.AddTimeline.AddSearchTimeline();
        }

        /// <summary>
        /// DMタイムラインの追加
        /// </summary>
        public void AddDirectMessageTimeline()
        {
            this.AddTimeline.AddDirectMessageTimeline();
        }

        #region ScreenNames 変更通知プロパティ
        public ObservableCollection<string> ScreenNames
        {
            get
            {
                return this.AddTimeline.ScreenNames;
            }
        }
        #endregion

        #region TokenSuffix 変更通知プロパティ
        public int TokenSuffix
        {
            get
            {
                return this.AddTimeline.TokenSuffix;
            }
            set
            {
                this.AddTimeline.TokenSuffix = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ProfileImageUrl 変更通知プロパティ
        public Uri ProfileImageUrl
        {
            get
            {
                return this.AddTimeline.ProfileImageUrl;
            }
        }
        #endregion

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.AddTimeline.IsOpen;
            }
            set
            {
                this.AddTimeline.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand CloseCommand { get; }

        public ObservableCollection<Grid> ExtraGrid { get; }

        private AddTimeline AddTimeline;
    }
}
