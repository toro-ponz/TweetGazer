using CoreTweet;
using Livet;
using MahApps.Metro.Controls.Dialogs;
using SourceChord.Lighty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TweetGazer.Behaviors;
using TweetGazer.Common;
using TweetGazer.ViewModels;

namespace TweetGazer.Models.Timeline
{
    public class StatusProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="originStatus">ツイート内容</param>
        /// <param name="type">ツイートの表示タイプ</param>
        public StatusProperties(TimelineModel timelineModel, Status originStatus, StatusType type = StatusType.Timeline)
        {
            this.TimelineModel = timelineModel;
            this.Type = type;

            this.Media = new List<MediaProperties>();
            this.QuotationStatus = new List<QuotationStatusProperties>();

            this.MediaColumnWidth = new List<GridLength>()
            {
                new GridLength(0),
                new GridLength(0),
                new GridLength(0),
                new GridLength(0)
            };

            this.SelectItemCommand = new RelayCommand(this.SelectItem);
            this.SelectIconCommand = new RelayCommand(this.SelectIcon);
            this.ReplyCommand = new RelayCommand(this.Reply);
            this.RetweetCommand = new RelayCommand(this.Retweet);
            this.QuotationLinkRetweetCommand = new RelayCommand(this.QuotationLinkRetweet);
            this.QuotationTextRetweetCommand = new RelayCommand(this.QuotationTextRetweet);
            this.FavoriteCommand = new RelayCommand(this.Favorite);
            this.BlockCommand = new RelayCommand(this.Block);
            this.MuteCommand = new RelayCommand(this.Mute);
            this.NotifyCommand = new RelayCommand<bool>(this.Notify);
            this.ShareCommand = new RelayCommand(this.Share);
            this.CopyCommand = new RelayCommand(this.Copy);
            this.OpenWebBrowserCommand = new RelayCommand(this.OpenWebBrowser);
            this.DeleteCommand = new RelayCommand(this.Delete);
            this.SelectQuotationStatusCommand = new RelayCommand(this.SelectQuotationStatus);
            this.SelectQuotationUserCommand = new RelayCommand(this.SelectQuotationUser);
            this.SelectMediaCommand = new RelayCommand<int>(this.SelectMedia);

            this.HyperlinkText = new HyperlinkTextProperties()
            {
                HashtagCommand = new RelayCommand<string>(this.SelectHashTag),
                MentionCommand = new RelayCommand<string>(this.SelectMention),
                UrlCommand = new RelayCommand<string>(this.SelectUrl)
            };

            this.QuotationIds = new List<long>();

            if (originStatus == null)
                return;

            var currentTime = DateTimeOffset.Now;
            var status = originStatus;
            while (status.RetweetedStatus != null)
            {
                status = originStatus.RetweetedStatus;
            }

            #region ツイート本文
            if (status.ExtendedTweet != null && status.ExtendedTweet.FullText != null)
                this.HyperlinkText.Text = status.ExtendedTweet.FullText;
            else if (status.Text != null)
                this.HyperlinkText.Text = status.Text;
            else if (status.FullText != null)
                this.HyperlinkText.Text = status.FullText;
            else
                this.HyperlinkText.Text = "";

            this.FullText = this.HyperlinkText.Text;

            if (status.ExtendedTweet?.Entities != null)
            {
                this.HyperlinkText.Hashtags = status.ExtendedTweet.Entities.HashTags.ToList();
                this.HyperlinkText.Mentions = status.ExtendedTweet.Entities.UserMentions.ToList();
                this.HyperlinkText.Urls = status.ExtendedTweet.Entities.Urls.ToList();
            }
            else if (status.Entities != null)
            {
                this.HyperlinkText.Hashtags = status.Entities.HashTags.ToList();
                this.HyperlinkText.Mentions = status.Entities.UserMentions.ToList();
                this.HyperlinkText.Urls = status.Entities.Urls.ToList();
            }
            #endregion 

            this.Id = status.Id;
            this.User = new UserOverviewProperties(status.User);
            this.IsRetweetedByUser = false;

            //ユーザーがリツイートしたツイートの場合
            if (originStatus.RetweetedStatus != null)
            {
                this.IsRetweetedByUser = true;
                this.RetweetedAt = originStatus.CreatedAt;
                this._RetweetedTime = CommonMethods.CalculateTime(currentTime, this.RetweetedAt);
                this.RetweetedUser = new UserOverviewProperties(originStatus.User);
            }

            this._IsRetweeted = status.IsRetweeted;
            this._IsFavorited = status.IsFavorited;
            this.CreatedAt = status.CreatedAt;
            this._ReplyCount = null;
            this._RetweetCount = status.RetweetCount;
            this._FavoriteCount = status.FavoriteCount;

            //現在時刻との差を計算・表示
            this._Time = CommonMethods.CalculateTime(currentTime, this.CreatedAt);

            this.Via = "via " + this.EscapeHtmlTags(status.Source);

            //メディアが含まれるツイートの場合
            IEnumerable<MediaEntity> media = null;
            if (status.ExtendedTweet?.ExtendedEntities?.Media != null)
                media = status.ExtendedTweet.ExtendedEntities.Media;
            else if (status.ExtendedEntities?.Media != null)
                media = status.ExtendedEntities.Media;
            else if (status.ExtendedTweet?.Entities?.Media != null)
                media = status.ExtendedTweet?.Entities.Media;
            else if (status.Entities?.Media != null)
                media = status.Entities.Media;
            if (media != null)
            {
                var j = 0;
                foreach (var m in media)
                {
                    this.Media.Add(new MediaProperties(m, j));
                    this.MediaColumnWidth[j] = new GridLength(1, GridUnitType.Star);
                    j++;
                }
                this.HyperlinkText.Media = media.ToList();
            }

            //引用が含まれるツイートの場合
            if (status.QuotedStatus != null)
            {
                this.QuotationIds.Add(status.QuotedStatus.Id);
                this.QuotationStatus.Add(new QuotationStatusProperties(status.QuotedStatus));
            }
            
            //リプライの場合
            if (Properties.Settings.Default.IsDisplayReplyStatus && status.InReplyToStatusId != null)
                this.ReplyToStatusProperties = new ReplyToStatusProperties(this.TimelineModel, (long)status.InReplyToStatusId);
            else if (status.InReplyToStatusId != null)
                this.ReplyToStatusProperties = new ReplyToStatusProperties(true);
            else
                this.ReplyToStatusProperties = new ReplyToStatusProperties(false);
        }

        /// <summary>
        /// ツイート・リツイート時間の更新
        /// </summary>
        /// <param name="currentTime">現在の時間</param>
        /// <returns>時間表示の更新が行われたか否か</returns>
        public bool RecalculateTime(DateTimeOffset currentTime)
        {
            var oldTime = this.Time;
            var oldRetweetedTime = this.RetweetedTime;

            this.Time = CommonMethods.CalculateTime(currentTime, this.CreatedAt);
            this.RetweetedTime = CommonMethods.CalculateTime(currentTime, this.RetweetedAt);

            if (oldTime == this.Time && oldRetweetedTime == this.RetweetedTime)
                return false;

            return true;
        }
        
        /// <summary>
        /// ツイートをクリックしたとき
        /// </summary>
        public void SelectItem()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                using (var status = new Views.ShowDialogs.ShowStatus(this.TimelineModel, this.Id))
                {
                    LightBox.ShowDialog(mainWindow, status);
                }
            }
        }

        /// <summary>
        /// ツイート者アイコンをクリックしたとき
        /// </summary>
        public void SelectIcon()
        {
            this.TimelineModel.ShowUserTimeline(this.User);
        }

        /// <summary>
        /// リプライボタンを押したとき
        /// </summary>
        public void Reply()
        {
            var text = "@" + this.User.ScreenName + "\n";
            var replyId = Id;
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                (mainWindow.DataContext as MainWindowViewModel).Reply(TimelineModel.TokenSuffix, text, "@" + this.User.ScreenName + ":\n" + this.FullText, replyId);
            }
        }

        /// <summary>
        /// リツイートボタンを押したとき
        /// </summary>
        public async void Retweet()
        {
            if (!this.CanRetweet)
                return;

            //RT済みでなければRT
            if (this.IsRetweeted == false)
            {
                if (await AccountTokens.RetweetStatusAsync(this.TimelineModel.TokenSuffix, this.Id))
                {
                    this.IsRetweeted = true;
                    this.RetweetCount++;
                }
            }
            //RT済みならばRT解除
            else if (this.IsRetweeted == true)
            {
                if (await AccountTokens.UnretweetStatusAsync(this.TimelineModel.TokenSuffix, this.Id))
                {
                    this.IsRetweeted = false;
                    this.RetweetCount--;
                }
            }
        }

        /// <summary>
        /// 引用リツイート(リンク)ボタンを押したとき
        /// </summary>
        public void QuotationLinkRetweet()
        {
            if (!this.CanRetweet)
                return;

            string link = "https://twitter.com/" + this.User.ScreenName + "/status/" + this.Id;
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                (mainWindow.DataContext as MainWindowViewModel).QuotationLinkRetweet(this.TimelineModel.TokenSuffix, link);
            }
        }

        /// <summary>
        /// 引用リツイート(文字列)ボタンを押したとき
        /// </summary>
        public void QuotationTextRetweet()
        {
            if (!this.CanRetweet)
                return;

            string text = "RT:@" + this.User.ScreenName + " " + this.HyperlinkText.Text;
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                (mainWindow.DataContext as MainWindowViewModel).QuotationTextRetweet(this.TimelineModel.TokenSuffix, text);
            }
        }

        /// <summary>
        /// いいねボタンを押したとき
        /// </summary>
        public async void Favorite()
        {
            //お気に入り済みでなければお気に入り
            if (this.IsFavorited == false)
            {
                if (await AccountTokens.CreateFavoriteStatusAsync(this.TimelineModel.TokenSuffix, this.Id))
                {
                    this.IsFavorited = true;
                    this.FavoriteCount++;
                }
            }
            //お気に入り済みならばお気に入り解除
            else if (this.IsFavorited == true)
            {
                if (await AccountTokens.DestroyFavoriteStatusAsync(this.TimelineModel.TokenSuffix, this.Id))
                {
                    this.IsFavorited = false;
                    this.FavoriteCount--;
                }
            }
        }

        /// <summary>
        /// ブロックボタンを押したとき
        /// </summary>
        public async void Block()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                if (await mainWindow.ShowMessageAsync("確認", "ユーザー名：" + this.User.Name + "をブロックしますか？", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    await AccountTokens.CreateBlockAsync(this.TimelineModel.TokenSuffix, this.User.Id);
                }
            }
        }

        /// <summary>
        /// ミュートボタンを押したとき
        /// </summary>
        public async void Mute()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                if (await mainWindow.ShowMessageAsync("確認", "ユーザー名：" + this.User.Name + "をミュートしますか？", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    await AccountTokens.CreateMuteAsync(this.TimelineModel.TokenSuffix, this.User.Id);
                }
            }
        }

        /// <summary>
        /// 通知ボタンを押したとき
        /// </summary>
        /// <param name="isIncludeRetweets">リツイートも通知するか否か</param>
        public async void Notify(bool isIncludeRetweets)
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                if (await mainWindow.ShowMessageAsync("確認", "ユーザー名：" + this.User.Name + "のツイートを通知リストに追加しますか？\n※フォロー中のユーザーのみ追加が可能です。", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    await AccountTokens.SetNotificationsAsync(this.TimelineModel.TokenSuffix, this.User.Id, true, isIncludeRetweets);
                }
            }
        }

        /// <summary>
        /// 共有ボタンを押したとき
        /// </summary>
        public void Share()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                string link = "https://twitter.com/" + this.User.ScreenName + "/status/" + Id;
                using (var showCopiableText = new Views.ShowDialogs.ShowCopiableText(link))
                    LightBox.ShowDialog(mainWindow, showCopiableText);
            }
        }

        /// <summary>
        /// コピーボタンを押したとき
        /// </summary>
        public void Copy()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                using (var showCopiableText = new Views.ShowDialogs.ShowCopiableText(this.FullText))
                    LightBox.ShowDialog(mainWindow, showCopiableText);
            }
        }

        /// <summary>
        /// Webブラウザで開くボタンを押したとき
        /// </summary>
        private void OpenWebBrowser()
        {
            try
            {
                Process.Start("https://twitter.com/" + this.User.ScreenName + "/status/" + this.Id);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        /// <summary>
        /// ツイートの削除ボタンを押したとき
        /// </summary>
        public async void Delete()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                if (await mainWindow.ShowMessageAsync("確認", "このツイートを削除しますか？", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    await AccountTokens.DeleteStatusAsync(this.TimelineModel.TokenSuffix, this.Id);
                }
            }
        }

        /// <summary>
        /// 引用ツイートをクリックしたとき
        /// </summary>
        public void SelectQuotationStatus()
        {
            var suffix = 0;
            if (suffix > this.QuotationIds.Count - 1)
                return;

            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                using (var status = new Views.ShowDialogs.ShowStatus(this.TimelineModel, this.QuotationIds[suffix]))
                {
                    LightBox.ShowDialog(mainWindow, status);
                }
            }
        }

        /// <summary>
        /// 引用元ツイート者アイコンをクリックしたとき
        /// </summary>
        public void SelectQuotationUser()
        {
            var suffix = 0;
            if (suffix > this.QuotationStatus.Count - 1)
                return;

            this.TimelineModel.ShowUserTimeline(this.QuotationStatus[suffix].User);
        }

        /// <summary>
        /// メディアをクリックしたとき
        /// </summary>
        /// <param name="suffix">メディア番号</param>
        public void SelectMedia(int suffix)
        {
            if (this.Media.Count <= suffix)
                return;

            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                //動画を表示
                if (this.Media[suffix].Type == StatusMediaType.Video)
                {
                    using (var showMovie = new Views.ShowDialogs.ShowMovie(this.Media[suffix].Url, false))
                        LightBox.ShowDialog(mainWindow, showMovie);
                }
                //画像を表示
                else
                {
                    using (var showStatus = new Views.ShowDialogs.ShowImage(this.Media.Select(x => x.Image).ToList(), suffix))
                        LightBox.ShowDialog(mainWindow, showStatus);
                }
            }
        }

        /// <summary>
        /// ハッシュタグをクリックしたとき
        /// </summary>
        /// <param name="hashtag">ハッシュタグ</param>
        private void SelectHashTag(string hashtag)
        {
            this.TimelineModel.ShowSearchTimeline(hashtag);
        }

        /// <summary>
        /// @useridをクリックしたとき
        /// </summary>
        /// <param name="screenName">スクリーンネーム</param>
        private async void SelectMention(string screenName)
        {
            var user = await AccountTokens.ShowUserAsync(this.TimelineModel.TokenSuffix, screenName);
            this.TimelineModel.ShowUserTimeline(new UserOverviewProperties(user));
        }

        /// <summary>
        /// URLをクリックしたとき
        /// </summary>
        /// <param name="url">URL</param>
        private void SelectUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url));
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        /// <summary>
        /// HTMLタグを除去する
        /// </summary>
        /// <param name="text">文字列</param>
        /// <returns>タグが除去された文字列</returns>
        private string EscapeHtmlTags(string text)
        {
            Regex re = new Regex(@"<.*?>", RegexOptions.Singleline);
            return re.Replace(text, "");
        }

        #region RetweetIcon 変更通知プロパティ
        public DataTemplate RetweetIcon
        {
            get
            {
                if (!this.CanRetweet)
                    return Application.Current.FindResource("GrayRetweetIcon") as DataTemplate;
                else if (this.IsRetweeted == true)
                    return Application.Current.FindResource("LightGreenRetweetIcon") as DataTemplate;
                else if (this.Type == StatusType.Timeline)
                {
                    return Application.Current.FindResource("FrontColorRetweetIcon") as DataTemplate;
                }
                return Application.Current.FindResource("DarkColorRetweetIcon") as DataTemplate;
            }
        }
        #endregion

        #region FavoriteHeartIcon 変更通知プロパティ
        public DataTemplate FavoriteHeartIcon
        {
            get
            {
                if (this.IsFavorited == true)
                    return Application.Current.FindResource("RedHeartIcon") as DataTemplate;
                else if (this.Type == StatusType.Timeline)
                    return Application.Current.FindResource("FrontColorHeartIcon") as DataTemplate;
                return Application.Current.FindResource("DarkColorHeartIcon") as DataTemplate;
            }
        }
        #endregion

        #region FavoriteStarIcon 変更通知プロパティ
        public DataTemplate FavoriteStarIcon
        {
            get
            {
                if (this.IsFavorited == true)
                    return Application.Current.FindResource("OrangeStarIcon") as DataTemplate;
                else if (this.Type == StatusType.Timeline)
                    return Application.Current.FindResource("FrontColorStarIcon") as DataTemplate;
                return Application.Current.FindResource("DarkColorStarIcon") as DataTemplate;
            }
        }
        #endregion

        #region Time 変更通知プロパティ
        public string Time
        {
            get
            {
                return this._Time;
            }
            set
            {
                this._Time = value;
                this.RaisePropertyChanged();
            }
        }
        private string _Time;
        #endregion

        #region RetweetedTime 変更通知プロパティ
        public string RetweetedTime
        {
            get
            {
                return this._RetweetedTime;
            }
            set
            {
                this._RetweetedTime = value;
                this.RaisePropertyChanged();
            }
        }
        private string _RetweetedTime;
        #endregion

        #region RetweetMenuText 変更通知プロパティ
        public string RetweetMenuText
        {
            get
            {
                if (this.IsRetweeted == null)
                    return "リツイート";
                else if (!(bool)this.IsRetweeted)
                    return "リツイート";
                else
                    return "リツイート解除";
            }
        }
        #endregion

        #region FavoriteMenuText 変更通知プロパティ
        public string FavoriteMenuText
        {
            get
            {
                if (this.IsFavorited == null)
                    return "いいね";
                else if (!(bool)this.IsFavorited)
                    return "いいね";
                else
                    return "いいね解除";
            }
        }
        #endregion

        #region IsRetweeted 変更通知プロパティ
        public bool? IsRetweeted
        {
            get
            {
                return this._IsRetweeted;
            }
            set
            {
                this._IsRetweeted = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.RetweetIcon));
                this.RaisePropertyChanged(nameof(this.RetweetMenuText));
            }
        }
        private bool? _IsRetweeted;
        #endregion

        #region IsFavorited 変更通知プロパティ
        public bool? IsFavorited
        {
            get
            {
                return this._IsFavorited;
            }
            set
            {
                this._IsFavorited = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.FavoriteHeartIcon));
                this.RaisePropertyChanged(nameof(this.FavoriteStarIcon));
                this.RaisePropertyChanged(nameof(this.FavoriteMenuText));
            }
        }
        public bool? _IsFavorited;
        #endregion

        #region CanDelete 変更通知プロパティ
        public bool CanDelete
        {
            get
            {
                if (this.User.Id == AccountTokens.Users[TimelineModel.TokenSuffix].Id)
                    return true;
                return false;
            }
        }
        #endregion

        #region CanRetweet 変更通知プロパティ
        public bool CanRetweet
        {
            get
            {
                return !this.User.IsProtected;
            }
        }
        #endregion

        #region ReplyCount 変更通知プロパティ
        public int? ReplyCount
        {
            get
            {
                return this._ReplyCount;
            }
            set
            {
                this._ReplyCount = value;
                this.RaisePropertyChanged();
            }
        }
        private int? _ReplyCount;
        #endregion

        #region RetweetCount 変更通知プロパティ
        public int? RetweetCount
        {
            get
            {
                return this._RetweetCount;
            }
            set
            {
                this._RetweetCount = value;
                this.RaisePropertyChanged();
            }
        }
        private int? _RetweetCount;
        #endregion

        #region FavoriteCount 変更通知プロパティ
        public int? FavoriteCount
        {
            get
            {
                return this._FavoriteCount;
            }
            set
            {
                this._FavoriteCount = value;
                this.RaisePropertyChanged();
            }
        }
        private int? _FavoriteCount;
        #endregion
        
        public StatusType Type { get; }

        public ICommand SelectItemCommand { get; }
        public ICommand SelectIconCommand { get; }
        public ICommand ReplyCommand { get; }
        public ICommand RetweetCommand { get; }
        public ICommand QuotationLinkRetweetCommand { get; }
        public ICommand QuotationTextRetweetCommand { get; }
        public ICommand FavoriteCommand { get; }
        public ICommand BlockCommand { get; }
        public ICommand MuteCommand { get; }
        public ICommand NotifyCommand { get; }
        public ICommand ShareCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand OpenWebBrowserCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectQuotationStatusCommand { get; }
        public ICommand SelectQuotationUserCommand { get; }
        public ICommand SelectMediaCommand { get; }

        public IList<MediaProperties> Media { get; }
        public IList<QuotationStatusProperties> QuotationStatus { get; }

        public IList<GridLength> MediaColumnWidth { get; }

        public IList<long> QuotationIds;

        public HyperlinkTextProperties HyperlinkText { get; }

        public ReplyToStatusProperties ReplyToStatusProperties { get; }

        public UserOverviewProperties User { get; }
        public UserOverviewProperties RetweetedUser { get; }

        public string FullText;
        public string Via { get; }

        public bool IsRetweetedByUser { get; }

        public long Id;

        private DateTimeOffset CreatedAt;
        private DateTimeOffset RetweetedAt;

        private TimelineModel TimelineModel;
    }

    /// <summary>
    /// ツイート表示タイプ
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// タイムライン内でのツイート
        /// </summary>
        Timeline,
        /// <summary>
        /// 個別ツイート表示でのメインツイート
        /// </summary>
        IndividualMain,
        /// <summary>
        /// 個別ツイート表示でのメイン以外のツイート
        /// </summary>
        IndividualOther
    }
}