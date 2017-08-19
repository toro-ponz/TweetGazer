using CoreTweet;
using CoreTweet.Streaming;
using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using TweetGazer.Common;
using TweetGazer.Models.Timeline;
using TweetGazer.ViewModels;

namespace TweetGazer.Models
{
    public class TimelineModel : NotificationObject, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">タイムラインデータ</param>
        public TimelineModel(TimelineData data)
        {
            this.Data = data;

            this.Disposables = new List<IDisposable>();
            this.Timers = new List<Timer>();
            this._ProgressRingVisibility = Visibility.Collapsed;
            this._IsVisibleSettings = false;
            
            this.TimelineItems = new ObservableCollection<TimelineItemProperties>();
            BindingOperations.EnableCollectionSynchronization(this.TimelineItems, new object());
            this.TimelineNotice = new ObservableCollection<TimelineNotice>();
            BindingOperations.EnableCollectionSynchronization(this.TimelineNotice, new object());

            var collectionView = CollectionViewSource.GetDefaultView(this.TimelineItems);
            if (collectionView != null)
                collectionView.CollectionChanged += this.CollectionChanged;

            if (this.Data == null)
                this.Data = new TimelineData();

            this.Data.SinceId = null;
            this.Data.PageSuffix = this.Data.PageSuffix;

            this.Initialize(this.Data.CurrentPage);
            this.Filtering();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="pageData">タイムラインデータ</param>
        public async void Initialize(TimelinePageData pageData)
        {
            await this.InitializeAsync(pageData);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="pageData">タイムラインデータ</param>
        public async Task InitializeAsync(TimelinePageData pageData)
        {
            this.IsInitializing = true;

            //ストリームを切断
            if (this.Disposables != null)
            {
                foreach (var disposable in this.Disposables)
                {
                    disposable.Dispose();
                }
            }

            //タイマーを削除
            if (this.Timers != null)
            {
                foreach (var timer in this.Timers)
                {
                    timer.Stop();
                }
            }

            this.SetTitle();
            this.StartStreaming();
            await this.Update();

            this.IsInitializing = false;
        }

        /// <summary>
        /// TLを最初のページの最上部にする
        /// </summary>
        public void Home()
        {
            this.Up();
            if (this.Data.PageSuffix == 0 || this.Data.Pages.Count < 2)
                return;

            while (this.Data.Pages.Count > 1)
                this.Data.Pages.RemoveAt(1);

            this.Data.PageSuffix = 0;
            this.TimelineItems.Clear();
            this.Initialize(this.Data.CurrentPage);
            this.VerticalOffset = 0;
        }

        /// <summary>
        /// 更新する
        /// </summary>
        /// <param name="maxId">取得するツイートの最大値(ID)</param>
        /// <returns></returns>
        public async Task<bool> Update(long? maxId = null)
        {
            if (this.IsLoading)
                return false;

            if (maxId == null)
            {
                if (this.Data.CurrentPage.TimelineType == TimelineType.User)
                {
                    while (this.TimelineItems.Count > 2)
                        this.TimelineItems.RemoveAt(this.TimelineItems.Count - 1);
                }
                else if (this.Data.CurrentPage.TimelineType == TimelineType.Search)
                {
                    while (this.TimelineItems.Count > 1)
                        this.TimelineItems.RemoveAt(this.TimelineItems.Count - 1);
                }
                else
                {
                    this.TimelineItems.Clear();
                }
            }

            this.Message = "";
            this.VerticalOffset = 0;

            this.IsLoading = true;
            if (maxId == null)
            {
                this.IsInitializing = true;
                this.ProgressRingVisibility = Visibility.Visible;
            }

            IEnumerable<Status> loadedTimeline = null;
            try
            {
                switch (this.Data.CurrentPage.TimelineType)
                {
                    case TimelineType.Home:
                        loadedTimeline = await AccountTokens.LoadHomeTimelineAsync(this.Data.TokenSuffix, maxId);
                        break;
                    case TimelineType.User:
                        var user = await AccountTokens.ShowUserAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId);
                        if (user != null)
                        {
                            //ユーザー概要がない場合は追加
                            if (this.TimelineItems.Count == 0)
                            {
                                this.TimelineItems.Add(new TimelineItemProperties(this, user));
                                this.TimelineItems.Add(new TimelineItemProperties(this, this.Data.CurrentPage.UserTimelineTab));
                            }

                            var excludeReplies = false;
                            //リプライを省く時
                            if (this.Data.CurrentPage.UserTimelineTab == UserTimelineTab.Tweets)
                                excludeReplies = true;

                            //いいねタブの時
                            if (this.Data.CurrentPage.UserTimelineTab == UserTimelineTab.Favorites)
                                loadedTimeline = await AccountTokens.LoadFavoritesAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId, maxId);
                            //メディアタブの時
                            else if (this.Data.CurrentPage.UserTimelineTab == UserTimelineTab.Media)
                            {
                                loadedTimeline = await AccountTokens.LoadUserTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId, excludeReplies, maxId, null, false);
                                if (loadedTimeline != null && loadedTimeline.Count() != 0)
                                {
                                    var nextMaxId = loadedTimeline.Last().Id - 1;
                                    loadedTimeline = loadedTimeline.Where(x => x.Entities != null && x.Entities.Media != null).ToList();

                                    //メディアの数は最低15件ずつ読み込む
                                    while (loadedTimeline.Count() < 15)
                                    {
                                        var nextStatuses = await AccountTokens.LoadUserTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId, excludeReplies, nextMaxId, null, false);
                                        if (nextStatuses == null || nextStatuses.Count() == 0)
                                            break;

                                        nextMaxId = nextStatuses.Last().Id - 1;
                                        loadedTimeline = loadedTimeline.Concat(nextStatuses.Where(x => x.Entities != null && x.Entities.Media != null).ToList());
                                    }
                                    //もっと読むの時
                                    if (maxId != null)
                                        this.TimelineItems.Last().LoadingProperties.Parameter = nextMaxId;
                                }
                            }
                            else
                                loadedTimeline = await AccountTokens.LoadUserTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId, excludeReplies, maxId);
                        }
                        break;
                    case TimelineType.List:
                        loadedTimeline = await AccountTokens.LoadListTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.ListNumber, maxId);
                        break;
                    case TimelineType.Mentions:
                        loadedTimeline = await AccountTokens.LoadMentionsTimelineAsync(this.Data.TokenSuffix, maxId);
                        break;
                    case TimelineType.MentionsStack:
                        this.TokenSuffix = 0;
                        this.TimelineItems.Clear();
                        foreach (var me in MentionsStack.Mentions)
                            this.TimelineItems.Add(new TimelineItemProperties(this, me.Status));
                        break;
                    case TimelineType.Favorite:
                        loadedTimeline = await AccountTokens.LoadFavoritesAsync(this.Data.TokenSuffix, maxId);
                        break;
                    case TimelineType.DirectMessage:
                        break;
                    case TimelineType.Trend:
                        {
                            var loadedTrends = await AccountTokens.LoadTrendsAsync(this.Data.TokenSuffix);
                            if (loadedTrends != null)
                                InsertTrends(loadedTrends);
                            else
                            {
                                this.IsLoading = false;
                                this.IsInitializing = false;
                                this.ProgressRingVisibility = Visibility.Collapsed;
                                return false;
                            }
                            break;
                        }
                    case TimelineType.Search:
                        // タブがない場合は追加
                        if (this.TimelineItems.Count == 0)
                            this.TimelineItems.Add(new TimelineItemProperties(this, this.Data.CurrentPage.SearchTimelineTab));

                        // ストリーミングの場合は空にする
                        if (this.Data.CurrentPage.SearchTimelineTab == SearchTimelineTab.Top)
                            loadedTimeline = await AccountTokens.LoadSearchTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.SearchText, maxId, resultType: "mixed");
                        else if (this.Data.CurrentPage.SearchTimelineTab == SearchTimelineTab.Latest)
                            loadedTimeline = await AccountTokens.LoadSearchTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.SearchText, maxId, resultType: "recent");
                        break;
                    case TimelineType.Notifications:
                        break;
                    case TimelineType.NotificationsStack:
                        this.TokenSuffix = 0;
                        this.TimelineItems.Clear();
                        foreach (var no in NotificationsStack.Notifications)
                            this.TimelineItems.Add(new TimelineItemProperties(this, no.SentUser, no.ReceiveUser, no.NotificationPropertiesType, no.Text, no.Id));
                        break;
                }
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);

                this.IsLoading = false;
                this.IsInitializing = false;
                this.ProgressRingVisibility = Visibility.Collapsed;

                if (maxId == null)
                    this.Message = "読み込み中にエラーが発生しました。タイムライン左下のリロードボタンで再度読み込み試行をしてください。";
                else if (this.TimelineItems[this.TimelineItems.Count - 1].LoadingProperties != null)
                    this.TimelineItems.RemoveAt(this.TimelineItems.Count - 1);

                return false;
            }

            // 読み込めていれば追加する
            if (loadedTimeline != null && loadedTimeline.Count() > 0)
            {
                if (maxId == null)
                {
                    this.InsertStatus(loadedTimeline);
                    // 末尾にローディングプログレスリングを追加
                    if (this.Data.CurrentPage.TimelineType != TimelineType.DirectMessage &&
                        this.Data.CurrentPage.TimelineType != TimelineType.MentionsStack &&
                        this.Data.CurrentPage.TimelineType != TimelineType.Trend &&
                        !(this.Data.CurrentPage.TimelineType == TimelineType.Search && this.Data.CurrentPage.SearchTimelineTab == SearchTimelineTab.Streaming) &&
                        this.Data.CurrentPage.TimelineType != TimelineType.Notifications &&
                        this.Data.CurrentPage.TimelineType != TimelineType.NotificationsStack)
                    {
                        this.TimelineItems.Add(new TimelineItemProperties(this, LoadingType.ReadMore, loadedTimeline.Last().Id - 1));
                    }
                }
                // ローディングプログレスリングによる古いツイートの読み込みのとき
                else
                {
                    // 最新ツイートを更新しないように追加
                    this.InsertStatus(loadedTimeline, true);
                    // ローディングプログレスリングを削除して再追加
                    if (this.TimelineItems[this.TimelineItems.Count - 1].LoadingProperties != null)
                    {
                        var loadingType = this.TimelineItems[this.TimelineItems.Count - 1].LoadingProperties.Type;
                        this.TimelineItems.RemoveAt(this.TimelineItems.Count - 1);
                        this.TimelineItems.Add(new TimelineItemProperties(this, loadingType, loadedTimeline.Last().Id - 1));
                    }
                }
            }
            else
            {
                this.IsLoading = false;
                this.IsInitializing = false;
                this.ProgressRingVisibility = Visibility.Collapsed;

                if (this.Data.CurrentPage.TimelineType == TimelineType.DirectMessage ||
                    this.Data.CurrentPage.TimelineType == TimelineType.MentionsStack ||
                    this.Data.CurrentPage.TimelineType == TimelineType.Trend ||
                    (this.Data.CurrentPage.TimelineType == TimelineType.Search && this.Data.CurrentPage.SearchTimelineTab == SearchTimelineTab.Streaming) ||
                    this.Data.CurrentPage.TimelineType == TimelineType.Notifications ||
                    this.Data.CurrentPage.TimelineType == TimelineType.NotificationsStack)
                    return true;
                else if (loadedTimeline == null && maxId == null)
                    this.Message = "読み込み中にエラーが発生しました。タイムライン左下のリロードボタンで再度読み込み試行をしてください。";
                else if (this.TimelineItems[this.TimelineItems.Count - 1].LoadingProperties != null)
                    this.TimelineItems.RemoveAt(this.TimelineItems.Count - 1);

                return false;
            }

            this.IsLoading = false;
            this.IsInitializing = false;
            this.ProgressRingVisibility = Visibility.Collapsed;
            return true;
        }

        /// <summary>
        /// 個別アカウントページの表示
        /// </summary>
        /// <param name="user">対象ユーザーのパラメーター</param>
        public void ShowUserTimeline(UserOverviewProperties user)
        {
            //今のページと同じなら追加しない
            if (this.Data.CurrentPage.TimelineType == TimelineType.User)
            {
                if (user != null && this.Data.CurrentPage.TargetUserId == user.Id)
                    return;
            }

            if (user == null)
                return;

            this.Data.Pages.Add(new TimelinePageData()
            {
                TimelineType = TimelineType.User,
                TargetUserId = user.Id,
                TargetUserName = user.Name,
                TargetUserScreenName = user.ScreenName,
                UserTimelineTab = UserTimelineTab.Tweets
            });
            this.Data.PageSuffix++;
            this.TimelineItems.Clear();
            this.Initialize(Data.CurrentPage);
        }

        /// <summary>
        /// 検索タイムラインへ遷移
        /// </summary>
        /// <param name="text">検索文字列</param>
        public void ShowSearchTimeline(string text)
        {
            this.Data.Pages.Add(new TimelinePageData()
            {
                TimelineType = TimelineType.Search,
                SearchText = text
            });
            this.Data.PageSuffix++;
            this.TimelineItems.Clear();
            this.Initialize(Data.CurrentPage);
        }

        /// <summary>
        /// ユーザーページのタブを変更したとき
        /// </summary>
        /// <param name="tab">変更後のタブ</param>
        public void ChangeUserTimelineTab(UserTimelineTab tab)
        {
            System.Windows.Input.Keyboard.ClearFocus();
            this.Data.CurrentPage.UserTimelineTab = tab;

            while (this.TimelineItems.Count > 2)
                this.TimelineItems.RemoveAt(this.TimelineItems.Count - 1);

            this.Data.SinceId = null;
            this.Initialize(this.Data.CurrentPage);
        }

        /// <summary>
        /// 検索ページのタブを変更したとき
        /// </summary>
        /// <param name="tab">変更後のタブ</param>
        public void ChangeSearchTimelineTab(SearchTimelineTab tab)
        {
            System.Windows.Input.Keyboard.ClearFocus();
            this.Data.CurrentPage.SearchTimelineTab = tab;

            while (this.TimelineItems.Count > 1)
                this.TimelineItems.RemoveAt(this.TimelineItems.Count - 1);

            this.Data.SinceId = null;
            this.Initialize(this.Data.CurrentPage);
        }

        /// <summary>
        /// タイムラインデータをシリアライズする
        /// </summary>
        /// <returns>タイムラインデータ</returns>
        public string Serialize()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TimelineData));
            using (var stringWriter = new StringWriter(CultureInfo.CurrentCulture))
            {
                serializer.Serialize(stringWriter, Data);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// 1つ前のページへ戻る
        /// </summary>
        public async void Back()
        {
            if (this.Data.PageSuffix == 0)
                return;

            this.TimelineItems.Clear();
            this.Data.Pages.RemoveAt(this.Data.PageSuffix);
            this.Data.PageSuffix--;
            var verticalOffset = this.VerticalOffset;
            await this.InitializeAsync(this.Data.CurrentPage);
            this.VerticalOffset = verticalOffset;
        }

        /// <summary>
        /// 最上部へスクロール
        /// </summary>
        public void Up()
        {
            this.VerticalOffset = 0;

            var collectionView = CollectionViewSource.GetDefaultView(this.TimelineItems) as CollectionView;
            if (collectionView == null)
                return;

            while(collectionView.Count > 100)
            {
                this.TimelineItems.RemoveAt(this.TimelineItems.Count - 2);
            }

            if (this.TimelineItems.Count != 0)
            {
                if (this.TimelineItems.Last().LoadingProperties != null)
                {
                    for (int i = this.TimelineItems.Count - 1; i > this.TimelineItems.Count - 10 && i >= 0; i--)
                    {
                        if (this.TimelineItems[i].StatusProperties != null)
                        {
                            this.TimelineItems.Last().LoadingProperties.Parameter = this.TimelineItems[i].StatusProperties.Id - 1;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 設定画面の開閉
        /// </summary>
        public void ToggleOpenSettings()
        {
            this.IsVisibleSettings = !this.IsVisibleSettings;
        }

        /// <summary>
        /// タイムラインをクリアする
        /// </summary>
        public void Clear()
        {
            this.TimelineItems.Clear();
        }

        /// <summary>
        /// 通知を表示する
        /// </summary>
        /// <param name="message">通知内容</param>
        /// <param name="type">通知タイプ</param>
        public async void Notify(string message, NotificationType type)
        {
            try
            {
                await Task.Run(async () =>
                {
                    this.TimelineNotice.Add(new TimelineNotice(message, type));
                    await Task.Delay(5000);
                    this.RemoveNotice();
                });
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        /// <summary>
        /// ストリーミングで流れてきたツイートを流す
        /// </summary>
        /// <param name="statusMessage">ツイート</param>
        /// <param name="userId">受け取ったユーザーID</param>
        public void StreamStatusMessage(StatusMessage statusMessage, long? userId)
        {
            if (userId == null || this.Data.UserId != userId)
                return;

            switch (this.Data.CurrentPage.TimelineType)
            {
                case TimelineType.Home:
                    // 自分のツイートがRTされたイベントを弾く
                    if (statusMessage.Status.RetweetedStatus != null &&
                        statusMessage.Status.RetweetedStatus.User.Id == this.Data.UserId)
                        return;

                    // 流れてきたツイートを挿入
                    this.InsertStatus(new List<Status>() { statusMessage.Status });
                    break;
                case TimelineType.Mentions:
                    // リプライでない場合リターン
                    if (statusMessage.Status.Entities == null || statusMessage.Status.Entities.UserMentions == null)
                        return;

                    foreach (var entity in statusMessage.Status.Entities.UserMentions)
                    {
                        // 自分へのリプライの場合処理する
                        if (entity.Id == this.Data.UserId)
                        {
                            // 流れてきたツイートを挿入
                            this.InsertStatus(new List<Status>() { statusMessage.Status });
                            break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// ストリーミングで流れてきた削除されたツイートを削除する
        /// </summary>
        /// <param name="statusMessage">削除されたツイート</param>
        /// <param name="userId">受け取ったユーザーID</param>
        public void StreamDeleteMessage(DeleteMessage deleteMessage, long? userId)
        {
            if (userId == null || this.Data.UserId != userId)
                return;

            switch (this.Data.CurrentPage.TimelineType)
            {
                case TimelineType.Home:
                case TimelineType.Mentions:
                    this.DeleteStatus(deleteMessage.Id);
                    break;
            }
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        /// <param name="disposing">マネージリソースを破棄するか否か</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            //ストリームを切断
            if (this.Disposables != null)
            {
                foreach (var disposable in this.Disposables)
                {
                    disposable.Dispose();
                }
            }

            //タイマーを削除
            if (this.Timers != null)
            {
                foreach (var timer in this.Timers)
                {
                    timer.Stop();
                }
            }

            //ウィンドウから自分を削除
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null && this.ColumnIndex >= 0)
                (mainWindow.DataContext as MainWindowViewModel).Timelines.RemoveTimeline(this.ColumnIndex);
        }

        /// <summary>
        /// タイトルを(再)設定する
        /// </summary>
        private void SetTitle()
        {
            switch (this.Data.CurrentPage.TimelineType)
            {
                case TimelineType.Home:
                    this.Title = "ホーム";
                    break;
                case TimelineType.List:
                    this.Title = this.Data.CurrentPage.ListName;
                    break;
                case TimelineType.DirectMessage:
                    this.Title = "ダイレクトメッセージ";
                    break;
                case TimelineType.Favorite:
                    this.Title = "いいね";
                    break;
                case TimelineType.Notifications:
                    this.Title = "通知";
                    break;
                case TimelineType.Mentions:
                    this.Title = "メンション";
                    break;
                case TimelineType.User:
                    this.Title = this.Data.CurrentPage.TargetUserName;
                    break;
                case TimelineType.Search:
                    this.Title = this.Data.CurrentPage.SearchText;
                    break;
                case TimelineType.Trend:
                    this.Title = "トレンド";
                    break;
            }
        }

        /// <summary>
        /// ストリーミング・タイマーの開始
        /// </summary>
        private void StartStreaming()
        {
            if (this.Data.CurrentPage.TimelineType != TimelineType.Notifications &&
                this.Data.CurrentPage.TimelineType != TimelineType.NotificationsStack &&
                this.Data.CurrentPage.TimelineType != TimelineType.Trend)
            {
                // ツイート時間の更新タイマー
                var reculcTimeTimer = new Timer();
                reculcTimeTimer.Elapsed += new ElapsedEventHandler(this.RecalculateTime);
                reculcTimeTimer.Interval = 1000;
                reculcTimeTimer.AutoReset = true;
                reculcTimeTimer.Enabled = true;
                this.Timers.Add(reculcTimeTimer);
            }

            switch (this.Data.CurrentPage.TimelineType)
            {
                case TimelineType.List:
                    {
                        // 5秒間隔でリストを更新するタイマー
                        var timer = new Timer();
                        timer.Elapsed += new ElapsedEventHandler(this.LoadListTimelineAsync);
                        timer.Interval = 5000;
                        timer.AutoReset = true;
                        timer.Enabled = true;
                        this.Timers.Add(timer);
                        break;
                    }
                case TimelineType.User:
                    {
                        // 30秒間隔でユーザータイムラインを更新するタイマー
                        var timer = new Timer();
                        timer.Elapsed += new ElapsedEventHandler(this.LoadUserTimelineAsync);
                        timer.Interval = 30000;
                        timer.AutoReset = true;
                        timer.Enabled = true;
                        this.Timers.Add(timer);
                        break;
                    }
                case TimelineType.Notifications:
                    {
                        var stream = AccountTokens.StartStreaming(this.Data.TokenSuffix, StreamingMode.User);
                        if (stream != null)
                        {
                            // 再接続
                            stream
                                .Catch(stream.DelaySubscription(TimeSpan.FromSeconds(10)).Retry())
                                .Repeat()
                                .Subscribe(
                                    (StreamingMessage m) => DebugConsole.WriteLine(m),
                                    (Exception ex) => DebugConsole.WriteLine(ex),
                                    () => DebugConsole.WriteLine("Streaming Ended.")
                                );
                            stream.OfType<EventMessage>().Subscribe(x => ProcessEventMessage(x));
                            stream.OfType<DisconnectMessage>().Subscribe(x => ProcessDisconnectMessage(x));
                            this.Disposables.Add(stream.Connect());
                        }
                        break;
                    }
                case TimelineType.Trend:
                    {
                        var timer = new Timer();
                        timer.Elapsed += new ElapsedEventHandler(this.LoadTrendsAsync);
                        timer.Interval = 15000;
                        timer.AutoReset = true;
                        timer.Enabled = true;
                        this.Timers.Add(timer);
                        break;
                    }
                case TimelineType.Search:
                    {
                        if (this.Data.CurrentPage.SearchTimelineTab == SearchTimelineTab.Streaming)
                        {
                            var stream = AccountTokens.StartStreaming(this.Data.TokenSuffix, StreamingMode.Filter, this.Data.CurrentPage.SearchText);
                            if (stream != null)
                            {
                                // 再接続
                                stream
                                    .Catch(stream.DelaySubscription(TimeSpan.FromSeconds(10)).Retry())
                                    .Repeat()
                                    .Subscribe(
                                        (StreamingMessage m) => { },
                                        (Exception ex) => DebugConsole.WriteLine(ex),
                                        () => DebugConsole.WriteLine("Streaming Ended.")
                                    );
                                // 検索ワードに引っかかる新規ツイートが流れてきたとき
                                stream.OfType<StatusMessage>().Subscribe(x =>
                                {
                                    // RTなら処理しない
                                    if (x.Status.RetweetedStatus != null)
                                        return;

                                    // 流れてきたツイートを挿入
                                    this.InsertStatus(new List<Status>() { x.Status });
                                });
                                stream.OfType<DeleteMessage>().Subscribe(x => DeleteStatus(x.Id));
                                stream.OfType<DisconnectMessage>().Subscribe(x => ProcessDisconnectMessage(x));
                                this.Disposables.Add(stream.Connect());
                            }
                        }
                        break;
                    }
                case TimelineType.DirectMessage:
                    {
                        //var stream = AccountTokens.StartStreaming(this.Data.TokenSuffix, StreamingMode.User);
                        //if (stream != null)
                        //{
                        //    //DMを受信した際
                        //    stream.OfType<DirectMessageMessage>().Subscribe(x =>
                        //    {
                        //        if (this.Data.IsNotificationSoundPlay)
                        //            CommonMethods.PlaySoundEffect(SoundEffect.Notification1);

                        //        //流れてきたツイートを挿入
                        //        this.InsertDirectMessage(new List<DirectMessage>() { x.DirectMessage });
                        //    });
                        //    stream.OfType<DisconnectMessage>().Subscribe(x => ProcessDisconnectMessage(x));
                        //}
                        break;
                    }
            }
        }

        /// <summary>
        /// イベントが流れてきたとき
        /// </summary>
        /// <param name="eventMessage">イベント</param>
        private void ProcessEventMessage(EventMessage eventMessage)
        {
            return;
        }

        /// <summary>
        /// 切断情報が流れてきたとき
        /// </summary>
        /// <param name="disconnectMessage">切断情報</param>
        private void ProcessDisconnectMessage(DisconnectMessage disconnectMessage)
        {

        }

        /// <summary>
        /// ツイートの追加・挿入
        /// </summary>
        /// <param name="statuses">ツイート</param>
        /// <param name="loadMore">もっと読むによる挿入かどうか</param>
        /// <returns></returns>
        private bool InsertStatus(IEnumerable<Status> statuses, bool loadMore = false)
        {
            if (!statuses.Any())
                return false;

            // もっと読むの場合最後のID - 1をMaxIdとして控える
            if (loadMore)
                this.TimelineItems.Last().LoadingProperties.Parameter = statuses.Last().Id - 1;
            // そうでない場合最初のID + 1をSinceIdとして控える
            else
                this.Data.SinceId = statuses.First().Id + 1;

            int i = 0;
            int insertPosition = 0;
            if (this.Data.CurrentPage.TimelineType == TimelineType.User)
                insertPosition = 2;
            else if (this.Data.CurrentPage.TimelineType == TimelineType.Search)
                insertPosition = 1;

            foreach (var status in statuses)
            {
                var properties = new TimelineItemProperties(this, status);

                if (loadMore)
                    this.TimelineItems.Insert(this.TimelineItems.Count - 1, properties);
                else
                    this.TimelineItems.Insert(i + insertPosition, properties);

                i++;
            }

            return true;
        }

        /// <summary>
        /// 削除されたツイートを取り除く
        /// </summary>
        /// <param name="id">当該ツイートID</param>
        private void DeleteStatus(long id)
        {
            for (int i = 0; i < this.TimelineItems.Count; i++)
            {
                if (this.TimelineItems[i].StatusProperties != null)
                {
                    if (this.TimelineItems[i].StatusProperties.Id == id)
                    {
                        this.TimelineItems.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// トレンドの設定・更新
        /// </summary>
        /// <param name="trends">トレンド</param>
        /// <returns></returns>
        private bool InsertTrends(IEnumerable<TrendsResult> trends)
        {
            if (!trends.Any())
                return false;

            int i = 1;
            foreach (var trend in trends)
            {
                foreach (var trendEntity in trend.Trends)
                {
                    var properties = new TimelineItemProperties(this, trendEntity, i);

                    if (this.TimelineItems.Count > i - 1)
                        this.TimelineItems[i - 1] = properties;
                    else
                        this.TimelineItems.Add(properties);

                    i++;

                    if (i > 30)
                        break;
                }
                break;
            }

            return true;
        }

        /// <summary>
        /// リストタイムラインを読み込む
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadListTimelineAsync(object sender, EventArgs e)
        {
            if (this.IsLoading)
                return;

            try
            {
                var loadedTimeline = await AccountTokens.LoadListTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.ListNumber, null, this.Data.SinceId);
                if (loadedTimeline != null && loadedTimeline.Count != 0)
                    this.InsertStatus(loadedTimeline);
            }
            catch (Exception ex)
            {
                DebugConsole.Write(ex);
            }
        }

        /// <summary>
        /// ユーザータイムラインを読み込む
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadUserTimelineAsync(object sender, EventArgs e)
        {
            if (this.IsLoading)
                return;

            try
            {
                IEnumerable<Status> loadedTimeline = null;
                var excludeReplies = false;
                //リプライを省く時
                if (this.Data.CurrentPage.UserTimelineTab == UserTimelineTab.Tweets)
                    excludeReplies = true;

                //いいねタブの時
                if (this.Data.CurrentPage.UserTimelineTab == UserTimelineTab.Favorites)
                    loadedTimeline = await AccountTokens.LoadFavoritesAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId, null, this.Data.SinceId);
                //メディアタブの時
                else if (this.Data.CurrentPage.UserTimelineTab == UserTimelineTab.Media)
                {
                    loadedTimeline = await AccountTokens.LoadUserTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId, excludeReplies, null, this.Data.SinceId, false);
                    if (loadedTimeline != null)
                        loadedTimeline = loadedTimeline.Where(x => x.Entities != null && x.Entities.Media != null);
                }
                else
                    loadedTimeline = await AccountTokens.LoadUserTimelineAsync(this.Data.TokenSuffix, this.Data.CurrentPage.TargetUserId, excludeReplies, null, this.Data.SinceId);

                if (loadedTimeline != null && loadedTimeline.Count() != 0)
                    this.InsertStatus(loadedTimeline);
            }
            catch (Exception ex)
            {
                DebugConsole.Write(ex);
            }
        }

        /// <summary>
        /// トレンドを読み込む
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadTrendsAsync(object sender, EventArgs e)
        {
            if (this.IsLoading)
                return;

            try
            {
                var loadedTrend = await AccountTokens.LoadTrendsAsync(this.Data.TokenSuffix);
                if (loadedTrend != null && loadedTrend.Count != 0)
                {
                    this.InsertTrends(loadedTrend);
                }
            }
            catch (Exception ex)
            {
                DebugConsole.Write(ex);
            }
        }

        /// <summary>
        /// ツイート時間表示の1秒毎更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecalculateTime(object sender, EventArgs e)
        {
            var currentTime = DateTimeOffset.Now;
            try
            {
                for (int i = 0; i < this.TimelineItems.Count; i++)
                {
                    if (this.TimelineItems[i].StatusProperties != null)
                    {
                        if (!this.TimelineItems[i].StatusProperties.RecalculateTime(currentTime))
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugConsole.Write(ex);
                return;
            }
        }

        /// <summary>
        /// ツイート受信通知を行う
        /// </summary>
        /// <param name="status"></param>
        private void NotifyStatus(StatusProperties status)
        {
            //サウンド通知
            if (this.Data.IsNotificationSoundPlay)
                CommonMethods.PlaySoundEffect(SoundEffect.Notification1);

            //トレイ通知
            if (this.Data.IsNotification)
            {
                var text = "ツイート受信\n@" + status.User.ScreenName + ":";
                    text += status.FullText;

                this.Notify(text, NotificationType.Normal);
            }
        }

        /// <summary>
        /// 通知を削除する
        /// </summary>
        /// <param name="noticeNumber">削除する通知番号</param>
        private void RemoveNotice(int noticeNumber = 0)
        {
            if (noticeNumber < this.TimelineNotice.Count)
                this.TimelineNotice.RemoveAt(noticeNumber);
        }

        /// <summary>
        /// タイムラインのフィルタリング
        /// </summary>
        private void Filtering()
        {
            var collectionView = CollectionViewSource.GetDefaultView(this.TimelineItems);
            if (collectionView == null)
                return;

            collectionView.Filter = x =>
            {
                var item = (TimelineItemProperties)x;
                var status = item.StatusProperties != null;
                var media = status && item.StatusProperties.Media.Count > 0;
                var excludeRetweet = !this.IsVisibleRetweet && (status && item.StatusProperties.IsRetweetedByUser);
                var excludeReply = !this.IsVisibleReply && (status && item.StatusProperties.ReplyToStatusProperties.IsExist);
                var excludeImage = !this.IsVisibleIncludeImagesStatus && (media && item.StatusProperties.Media.First().Type == Behaviors.StatusMediaType.Image);
                var excludeGif = !this.IsVisibleIncludeGifStatus && (media && item.StatusProperties.Media.First().Type == Behaviors.StatusMediaType.AnimationGif);
                var excludeVideo = !this.IsVisibleIncludeVideoStatus && (media && item.StatusProperties.Media.First().Type == Behaviors.StatusMediaType.Video);
                var excludeLink = !this.IsVisibleIncludeLinkStatus && (status && item.StatusProperties.HyperlinkText.Urls.Count > 0);
                var excludeOther = !this.IsVisibleOtherStatus &&
                    (status &&
                    !media &&
                    !item.StatusProperties.IsRetweetedByUser &&
                    !item.StatusProperties.ReplyToStatusProperties.IsExist &&
                    item.StatusProperties.HyperlinkText.Urls.Count <= 0);
                return !excludeRetweet && !excludeReply && !excludeImage && !excludeGif && !excludeVideo && !excludeLink && !excludeOther;
            };
        }

        /// <summary>
        /// タイムラインの要素が変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.Action == null)
                return;

            switch (e.Action)
            {
                // 要素が追加されたとき
                case NotifyCollectionChangedAction.Add:
                    // 初期化中は通知しない
                    if (!this.IsInitializing)
                    {
                        // 追加されたオブジェクトがツイートなら通知
                        if (e?.NewItems?.SyncRoot != null &&
                            e.NewItems.SyncRoot is object[] addItems &&
                            addItems.Count() != 0 &&
                            addItems[0] is TimelineItemProperties item &&
                            item.StatusProperties != null)
                        {
                            this.NotifyStatus(item.StatusProperties);
                        }
                    }
                    break;
            }
        }

        #region ProgressRingVisibility 変更通知プロパティ
        public Visibility ProgressRingVisibility
        {
            get
            {
                return this._ProgressRingVisibility;
            }
            set
            {
                this._ProgressRingVisibility = value;
                this.RaisePropertyChanged();
            }
        }
        private Visibility _ProgressRingVisibility;
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
                return this.Data.CurrentPage.Title;
            }
            set
            {
                this.Data.CurrentPage.Title = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region VerticalOffset 変更通知プロパティ
        public double VerticalOffset
        {
            get
            {
                return this.Data.CurrentPage.VerticalOffset;
            }
            set
            {
                this.Data.CurrentPage.VerticalOffset = value;
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
                return this.Data.CurrentPage.IsVisibleRetweet;
            }
            set
            {
                this.Data.CurrentPage.IsVisibleRetweet = value;
                this.RaisePropertyChanged();
                this.Filtering();
            }
        }
        #endregion

        #region IsVisibleReply 変更通知プロパティ
        public bool IsVisibleReply
        {
            get
            {
                return this.Data.CurrentPage.IsVisibleReply;
            }
            set
            {
                this.Data.CurrentPage.IsVisibleReply = value;
                this.RaisePropertyChanged();
                this.Filtering();
            }
        }
        private bool _IsVisibleReply;
        #endregion

        #region IsVisibleIncludeImagesStatus 変更通知プロパティ
        public bool IsVisibleIncludeImagesStatus
        {
            get
            {
                return this.Data.CurrentPage.IsVisibleIncludeImagesStatus;
            }
            set
            {
                this.Data.CurrentPage.IsVisibleIncludeImagesStatus = value;
                this.RaisePropertyChanged();
                this.Filtering();
            }
        }
        #endregion

        #region IsVisibleIncludeGifStatus 変更通知プロパティ
        public bool IsVisibleIncludeGifStatus
        {
            get
            {
                return this.Data.CurrentPage.IsVisibleIncludeGifStatus;
            }
            set
            {
                this.Data.CurrentPage.IsVisibleIncludeGifStatus = value;
                this.RaisePropertyChanged();
                this.Filtering();
            }
        }
        #endregion

        #region IsVisibleIncludeVideoStatus 変更通知プロパティ
        public bool IsVisibleIncludeVideoStatus
        {
            get
            {
                return this.Data.CurrentPage.IsVisibleIncludeVideoStatus;
            }
            set
            {
                this.Data.CurrentPage.IsVisibleIncludeVideoStatus = value;
                this.RaisePropertyChanged();
                this.Filtering();
            }
        }
        #endregion

        #region IsVisibleIncludeLinkStatus 変更通知プロパティ
        public bool IsVisibleIncludeLinkStatus
        {
            get
            {
                return this.Data.CurrentPage.IsVisibleIncludeLinkStatus;
            }
            set
            {
                this.Data.CurrentPage.IsVisibleIncludeLinkStatus = value;
                this.RaisePropertyChanged();
                this.Filtering();
            }
        }
        #endregion

        #region IsVisibleOtherStatus 変更通知プロパティ
        public bool IsVisibleOtherStatus
        {
            get
            {
                return this.Data.CurrentPage.IsVisibleOtherStatus;
            }
            set
            {
                this.Data.CurrentPage.IsVisibleOtherStatus = value;
                this.RaisePropertyChanged();
                this.Filtering();
            }
        }
        #endregion

        #region TokenSuffix 変更通知プロパティ
        public int TokenSuffix
        {
            get
            {
                return this.Data.TokenSuffix;
            }
            set
            {
                this.Data.TokenSuffix = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ObservableCollection<TimelineItemProperties> TimelineItems { get; }
        public ObservableCollection<TimelineNotice> TimelineNotice { get; }

        public TimelineData Data { get; }

        public string ScreenName
        {
            get
            {
                return this.Data.ScreenName;
            }
            set
            {
                this.Data.ScreenName = value;
            }
        }
        public int ColumnIndex
        {
            get
            {
                return this.Data.ColumnIndex;
            }
            set
            {
                this.Data.ColumnIndex = value;
            }
        }

        private List<IDisposable> Disposables;
        private List<Timer> Timers;

        private bool IsLoading;
        private bool IsInitializing;
    }
}