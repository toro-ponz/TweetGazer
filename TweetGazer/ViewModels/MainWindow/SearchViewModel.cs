using Livet;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models;
using TweetGazer.Models.MainWindow;
using TweetGazer.Models.Timeline;

namespace TweetGazer.ViewModels.MainWindow
{
    public class SearchViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SearchViewModel()
        {
            this.Search = new Search();

            this._IsVisibleBackButton = false;
            this._TimelineItems = new ObservableCollection<TimelineItemProperties>();
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Search, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Search.ScreenNames):
                            this.RaisePropertyChanged(() => this.ScreenNames);
                            break;
                        case nameof(this.Search.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpen);
                            break;
                        case nameof(this.Search.ProfileImageUrl):
                            this.RaisePropertyChanged(() => this.ProfileImageUrl);
                            break;
                        case nameof(this.Search.TokenSuffix):
                            this.RaisePropertyChanged(() => this.TokenSuffix);
                            break;
                        case nameof(this.Search.SearchText):
                            this.RaisePropertyChanged(() => this.SearchText);
                            break;
                    }
                })
            );

            this.CloseCommand = new RelayCommand(this.ToggleOpen);
            this.SearchingCommand = new RelayCommand(this.Searching);
        }

        /// <summary>
        /// 1ページ戻る
        /// </summary>
        public void Back()
        {
            this.Search.SearchText = "";
            this.Timeline.Back();
        }

        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        public void ToggleOpen()
        {
            this.Search.ToggleOpen();
            if (this.Timeline == null)
            {
                this.Timeline = this.Search.Timeline;
                this.TimelineItems = this.Timeline.TimelineItems;
                this.CompositeDisposable.Add(
                    new PropertyChangedEventListener(this.Timeline.TimelineItems, (_, __) =>
                    {
                        switch (__.PropertyName)
                        {
                            case nameof(this.Timeline.TimelineItems):
                                this.RaisePropertyChanged(() => this.TimelineItems);
                                break;
                        }
                    })
                );
                this.CompositeDisposable.Add(
                    new PropertyChangedEventListener(this.Timeline.Data, (_, __) =>
                    {
                        switch (__.PropertyName)
                        {
                            case nameof(this.Timeline.Data.PageSuffix):
                                this.IsVisibleBackButton = this.Timeline.Data.IsVisibleBackButton;
                                this.TimelineItems = this.Timeline.TimelineItems;
                                break;
                        }
                    })
                );
                this.CompositeDisposable.Add(this.Timeline);
            }
        }

        /// <summary>
        /// 検索
        /// </summary>
        public void Searching()
        {
            this.Search.Searching();
        }

        #region TimelineItems 変更通知プロパティ
        public ObservableCollection<TimelineItemProperties> TimelineItems
        {
            get
            {
                return this._TimelineItems;
            }
            set
            {
                this._TimelineItems = value;
                this.RaisePropertyChanged();
            }
        }
        private ObservableCollection<TimelineItemProperties> _TimelineItems;
        #endregion

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.Search.IsOpen;
            }
            set
            {
                this.Search.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsVisibleBackButton 変更通知プロパティ
        public bool IsVisibleBackButton
        {
            get
            {
                return this._IsVisibleBackButton;
            }
            set
            {
                this._IsVisibleBackButton = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsVisibleBackButton;
        #endregion

        #region SearchText 変更通知プロパティ
        public string SearchText
        {
            get
            {
                return this.Search.SearchText;
            }
            set
            {
                this.Search.SearchText = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ScreenNames 変更通知プロパティ
        public ObservableCollection<string> ScreenNames
        {
            get
            {
                return this.Search.ScreenNames;
            }
        }
        #endregion

        #region TokenSuffix 変更通知プロパティ
        public int TokenSuffix
        {
            get
            {
                return this.Search.TokenSuffix;
            }
            set
            {
                this.Search.TokenSuffix = value;
                this.Timeline.TokenSuffix = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ProfileImageUrl 変更通知プロパティ
        public Uri ProfileImageUrl
        {
            get
            {
                return this.Search.ProfileImageUrl;
            }
        }
        #endregion

        public ICommand CloseCommand { get; }
        public ICommand SearchingCommand { get; }

        private TimelineModel Timeline;
        private Search Search;
    }
}
