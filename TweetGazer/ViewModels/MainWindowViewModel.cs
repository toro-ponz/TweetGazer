﻿using CoreTweet;
using Livet;
using Livet.EventListeners;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TweetGazer.Common;
using TweetGazer.Models;
using TweetGazer.Models.MainWindow;
using TweetGazer.ViewModels.MainWindow;

namespace TweetGazer.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            this.MainWindowModel = new MainWindowModel(this);
            this.ToastNotifications = this.MainWindowModel.ToastNotifications;
            this.Login();

            this.CreateStatus = new CreateStatusViewModel();
            this.MovableCreateStatus = new MovableCreateStatusViewModel();
            this.AddTimeline = new AddTimelineViewModel(this);
            this.AddAccount = new AddAccountViewModel(this);
            this.Mentions = new MentionsViewModel();
            this.Notifications = new NotificationsViewModel();
            this.DirectMessages = new DirectMessagesViewModel();
            this.AccountSettings = new AccountSettingsViewModel();
            this.Search = new SearchViewModel();
            this.ApplicationSettings = new ApplicationSettingsViewModel();
            this.NetworkState = new NetworkStateViewModel();
            this.Instructions = new InstructionsViewModel();
            this.Timelines = new TimelinesGridViewModel();
            
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.CreateStatus, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.CreateStatus.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.AddTimeline, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.AddTimeline.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.AccountSettings, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.AccountSettings.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.ApplicationSettings, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.ApplicationSettings.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Mentions, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Mentions.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Notifications, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Notifications.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.DirectMessages, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.DirectMessages.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Search, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Search.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpeningFlyouts);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(MentionsStack.Mentions, (_, __) => this.RaisePropertyChanged(() => this.HasMentionNotifications))
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(NotificationsStack.Notifications, (_, __) => this.RaisePropertyChanged(() => this.HasNotifications))
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(Properties.Settings.Default, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(Properties.Settings.Default.AccentColor):
                        case nameof(Properties.Settings.Default.BaseColor):
                            this.ChangeColors();
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(this.CreateStatus);
            this.CompositeDisposable.Add(this.MovableCreateStatus);
            this.CompositeDisposable.Add(this.AddTimeline);
            this.CompositeDisposable.Add(this.Mentions);
            this.CompositeDisposable.Add(this.Notifications);
            this.CompositeDisposable.Add(this.DirectMessages);
            this.CompositeDisposable.Add(this.AccountSettings);
            this.CompositeDisposable.Add(this.ApplicationSettings);
            this.CompositeDisposable.Add(this.Timelines);
            this.CompositeDisposable.Add(this.AddAccount);
            this.CompositeDisposable.Add(this.Instructions);
            this.CompositeDisposable.Add(this.Search);
            this.CompositeDisposable.Add(this.NetworkState);

            this.CompositeDisposable.Add(this.MainWindowModel);
        }

        /// <summary>
        /// ホームボタンを押したとき
        /// </summary>
        public void Home()
        {
            if (this.Timelines.Timelines.Count == 0)
            {
                return;
            }

            foreach (var timeline in this.Timelines.Timelines)
            {
                timeline.TimelineViewModel.Home();
            }
        }

        /// <summary>
        /// Flyoutを閉じる
        /// </summary>
        public void CloseFlyout()
        {
            if (this.CreateStatus.IsOpen)
            {
                this.CreateStatus.ToggleOpen();
            }

            if (this.AddTimeline.IsOpen)
            {
                this.AddTimeline.ToggleOpen();
            }

            if (this.Mentions.IsOpen)
            {
                this.Mentions.ToggleOpen();
            }

            if (this.Notifications.IsOpen)
            {
                this.Notifications.ToggleOpen();
            }

            if (this.DirectMessages.IsOpen)
            {
                this.DirectMessages.ToggleOpen();
            }

            if (this.AccountSettings.IsOpen)
            {
                this.AccountSettings.ToggleOpen();
            }

            if (this.ApplicationSettings.IsOpen)
            {
                this.ApplicationSettings.ToggleOpen();
            }

            if (this.Search.IsOpen)
            {
                this.Search.ToggleOpen();
            }
        }

        /// <summary>
        /// Windowを閉じる
        /// </summary>
        public void Close()
        {
            if (AccountTokens.TokensCount > 0)
            {
                this.Timelines.SaveColumnData();
            }

            while (this.Timelines.Timelines.Count != 0)
            {
                this.Timelines.Timelines.First().TimelineViewModel.Close();
            }

            while (this.Timelines.Grid.First().Children.Count != 0)
            {
                this.Timelines.Grid.First().Children.RemoveAt(0);
            }

            this.Dispose();
        }

        /// <summary>
        /// 更新ボタンを押したとき
        /// </summary>
        public async void UpdateTimelines()
        {
            if (AccountTokens.TokensCount == 0 || AccountTokens.Users.Count == 0)
            {
                this.MainWindowModel.Notify("トークン再認証開始", NotificationType.Normal);
                await AccountTokens.LoadTokensAsync();
                return;
            }

            if (this.Timelines.Timelines.Count == 0)
            {
                return;
            }

            foreach (var timeline in this.Timelines.Timelines)
            {
                timeline.TimelineViewModel.Clear();
                await timeline.TimelineViewModel.Update();
            }
        }

        /// <summary>
        /// デバッグコンソールを開く
        /// </summary>
        public void DebugConsoleOpen()
        {
            this.MainWindowModel.DebugConsoleOpen();
        }

        /// <summary>
        /// 通知を行う
        /// </summary>
        /// <param name="message">通知内容</param>
        /// <param name="type">通知タイプ</param>
        public void Notify(string message, NotificationType type)
        {
            this.MainWindowModel.Notify(message, type);
        }

        /// <summary>
        /// リプライボタンを押したとき
        /// </summary>
        /// <param name="tokenSuffix">元ツイートを含むTLの親アカウント番号</param>
        /// <param name="text">ツイート文(@id)</param>
        /// <param name="replyText">リプライ先のツイート文</param>
        /// <param name="replyId">リプライ先のツイートID</param>
        public void Reply(int tokenSuffix, string text, string replyText = "リプライ先:なし", long? replyId = null)
        {
            if (!this.CreateStatus.IsOpen)
            {
                this.CreateStatus.ToggleOpen();
            }

            this.CreateStatus.StatusText = text;
            this.CreateStatus.ReplyText = replyText;
            this.CreateStatus.ReplyId = replyId;
            this.CreateStatus.CaretPosition = Behaviors.CaretPosition.Undefined;
            this.CreateStatus.CaretPosition = Behaviors.CaretPosition.Last;
            this.CreateStatus.SelectUser(tokenSuffix);
        }

        /// <summary>
        /// 引用(リンク)ボタンを押したとき
        /// </summary>
        /// <param name="tokenSuffix">元ツイートを含むTLの親アカウント番号</param>
        /// <param name="link">引用ツイートのURL</param>
        public void QuotationLinkRetweet(int tokenSuffix, string link)
        {
            if (!this.CreateStatus.IsOpen)
            {
                this.CreateStatus.ToggleOpen();
            }

            this.CreateStatus.StatusText = "\n" + link;
            this.CreateStatus.CaretPosition = Behaviors.CaretPosition.Undefined;
            this.CreateStatus.CaretPosition = Behaviors.CaretPosition.Top;
            this.CreateStatus.SelectUser(tokenSuffix);
        }

        /// <summary>
        /// 引用(文字列)ボタンを押したとき
        /// </summary>
        /// <param name="tokenSuffix">元ツイートを含むTLの親アカウント番号</param>
        /// <param name="text">元ツイートの内容など</param>
        public void QuotationTextRetweet(int tokenSuffix, string text)
        {
            if (!this.CreateStatus.IsOpen)
            {
                this.CreateStatus.ToggleOpen();
            }

            this.CreateStatus.StatusText = "\n" + text;
            this.CreateStatus.CaretPosition = Behaviors.CaretPosition.Undefined;
            this.CreateStatus.CaretPosition = Behaviors.CaretPosition.Top;
            this.CreateStatus.SelectUser(tokenSuffix);
        }

        /// <summary>
        /// 起動時のログイン
        /// </summary>
        private async void Login()
        {
            await Task.Run(() =>
            {
                while (this.AddAccount == null)
                {
                    System.Threading.Thread.Sleep(100);
                }
            });

            //トークンファイルが正常に読み込めなかった場合、ログイン画面を表示する
            try
            {
                //トークンの読み込み
                if (!await AccountTokens.LoadTokensAsync())
                {

                }
                //列データの読み込み
                else if (this.Timelines.LoadColumnData())
                {
                    this.MainWindowModel.Notify("カラム読み込み完了.", NotificationType.Normal);
                }
            }
            catch (TwitterException e)
            {
                this.MainWindowModel.Notify("トークン認証失敗.", NotificationType.Error);
                DebugConsole.Write(e);
                return;
            }
            catch (Exception e) when (e is HttpRequestException || e is WebException)
            {
                this.MainWindowModel.Notify("ネットワークに正常に接続できませんでした\n左下の更新ボタンを押して再認証してください", NotificationType.Error);
                DebugConsole.Write(e);
                return;
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return;
            }
        }

        /// <summary>
        /// ウィンドウのカラーを変更する
        /// </summary>
        private void ChangeColors()
        {
            this.MainWindowModel.ChangeColors();
        }

        #region HasMentionNotifications 変更通知プロパティ
        public bool HasMentionNotifications
        {
            get
            {
                return MentionsStack.Mentions.Any();
            }
        }
        #endregion

        #region HasNotifications 変更通知プロパティ
        public bool HasNotifications
        {
            get
            {
                return NotificationsStack.Notifications.Any();
            }
        }
        #endregion

        #region IsOpeningFlyouts 変更通知プロパティ
        public bool IsOpeningFlyouts
        {
            get
            {
                return this.CreateStatus.IsOpen
                    || this.AddTimeline.IsOpen
                    || this.Mentions.IsOpen
                    || this.Notifications.IsOpen
                    || this.DirectMessages.IsOpen
                    || this.AccountSettings.IsOpen
                    || this.ApplicationSettings.IsOpen
                    || this.Search.IsOpen;
            }
        }
        #endregion

        public CreateStatusViewModel CreateStatus { get; }
        public MovableCreateStatusViewModel MovableCreateStatus { get; }
        public AddTimelineViewModel AddTimeline { get; }
        public MentionsViewModel Mentions { get; }
        public NotificationsViewModel Notifications { get; }
        public DirectMessagesViewModel DirectMessages { get; }
        public AccountSettingsViewModel AccountSettings { get; }
        public SearchViewModel Search { get; }
        public ApplicationSettingsViewModel ApplicationSettings { get; }
        public TimelinesGridViewModel Timelines { get; }
        public AddAccountViewModel AddAccount { get; }
        public NetworkStateViewModel NetworkState { get; }
        public InstructionsViewModel Instructions { get; }
        public ObservableCollection<ToastNotification> ToastNotifications { get; }

        private MainWindowModel MainWindowModel;
    }
}
