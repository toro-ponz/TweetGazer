﻿using Livet;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models;
using TweetGazer.Models.MainWindow;
using TweetGazer.Models.Timeline;

namespace TweetGazer.ViewModels.MainWindow
{
    public class NotificationsViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotificationsViewModel()
        {
            NotificationsStack.Initialize();

            this.Notifications = new Notifications();

            this._IsVisibleBackButton = false;
            this._TimelineItems = new List<TimelineItemProperties>();
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Notifications, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Notifications.ScreenNames):
                            this.RaisePropertyChanged(() => this.ScreenNames);
                            break;
                        case nameof(this.Notifications.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpen);
                            break;
                        case nameof(this.Notifications.ProfileImageUrl):
                            this.RaisePropertyChanged(() => this.ProfileImageUrl);
                            break;
                        case nameof(this.Notifications.TokenSuffix):
                            this.RaisePropertyChanged(() => this.TokenSuffix);
                            break;
                    }
                })
            );

            this.CloseCommand = new RelayCommand(this.ToggleOpen);
        }

        /// <summary>
        /// 1ページ戻る
        /// </summary>
        public void Back()
        {
            this.Timeline.Back();
        }

        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        public void ToggleOpen()
        {
            this.Notifications.ToggleOpen();
            if (this.Timeline == null)
            {
                this.Timeline = this.Notifications.Timeline;
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
                            case nameof(this.Timeline.Data.IsVisibleBackButton):
                            case nameof(this.Timeline.Data.PageSuffix):
                                this.IsVisibleBackButton = this.Timeline.Data.IsVisibleBackButton;
                                break;
                        }
                    })
                );
                this.CompositeDisposable.Add(this.Timeline);
            }
        }

        /// <summary>
        /// 通知スタックの削除
        /// </summary>
        public void Delete()
        {
            this.Notifications.Delete();
        }

        #region TimelineItems 変更通知プロパティ
        public IEnumerable<TimelineItemProperties> TimelineItems
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
        private IEnumerable<TimelineItemProperties> _TimelineItems;
        #endregion

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.Notifications.IsOpen;
            }
            set
            {
                this.Notifications.IsOpen = value;
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

        #region ScreenNames 変更通知プロパティ
        public ObservableCollection<string> ScreenNames
        {
            get
            {
                return this.Notifications.ScreenNames;
            }
        }
        #endregion

        #region TokenSuffix 変更通知プロパティ
        public int TokenSuffix
        {
            get
            {
                return this.Notifications.TokenSuffix;
            }
            set
            {
                this.Notifications.TokenSuffix = value;
                this.Timeline.TokenSuffix = value;
                Task.Run(async () =>
                {
                    await this.Timeline.Update();
                });
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ProfileImageUrl 変更通知プロパティ
        public Uri ProfileImageUrl
        {
            get
            {
                return this.Notifications.ProfileImageUrl;
            }
        }
        #endregion

        public ICommand CloseCommand { get; }
        public ICommand SearchingCommand { get; }

        private TimelineModel Timeline;
        private Notifications Notifications;
    }
}