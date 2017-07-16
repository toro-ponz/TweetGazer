using CoreTweet;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using TweetGazer.Behaviors;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class UserProperties : UserOverviewProperties
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="user">ユーザーデータ</param>
        public UserProperties(TimelineModel timelineModel, User user) : base(user)
        {
            this.TimelineModel = timelineModel;

            this.Description = new HyperlinkTextProperties()
            {
                HashtagCommand = new RelayCommand<string>(this.SelectHashTag),
                MentionCommand = new RelayCommand<string>(this.SelectMention),
                UrlCommand = new RelayCommand<string>(this.SelectUrl)
            };
            this.Description.Text = user.Description;
            if (user.Entities != null && user.Entities.Description != null)
            {
                if (user.Entities.Description.HashTags != null)
                    this.Description.Hashtags = user.Entities.Description.HashTags.ToList();
                if (user.Entities.Description.UserMentions != null)
                    this.Description.Mentions = user.Entities.Description.UserMentions.ToList();
                if (user.Entities.Description.Urls != null)
                    this.Description.Urls = user.Entities.Description.Urls.ToList();
            }

            this.StatusesCount = user.StatusesCount;
            this.FavouritesCount = user.FavouritesCount;
            this.FollowersCount = user.FollowersCount;
            this.FriendsCount = user.FriendsCount;
            if (user.ProfileBannerUrl != null)
                this.ProfileBanner = new ImageProperties(user.ProfileBannerUrl, true);
            if (user.Entities != null && user.Entities.Url != null)
            {
                if (user.Entities.Url.Urls.First().ExpandedUrl != null)
                    this.Url = new Uri(user.Entities.Url.Urls.First().ExpandedUrl);
                else
                    this.Url = new Uri(user.Entities.Url.Urls.First().Url);

                if (user.Entities.Url.Urls.First().DisplayUrl != null)
                    this.UrlText = user.Entities.Url.Urls.First().DisplayUrl;
                else
                    this.UrlText = user.Entities.Url.Urls.First().Url;
            }
            this.CreatedAt = user.CreatedAt;
            this.IsProtected = user.IsProtected;
            if (user.IsMuting != null)
                this._IsMuting = (bool)user.IsMuting;

            this.FollowCommand = new RelayCommand(this.Follow);
            this.UrlCommand = new RelayCommand<Uri>(this.SelectUrl);

            this.LoadRelationship(user);
        }

        /// <summary>
        /// フォローボタンをクリックしたとき
        /// </summary>
        private async void Follow()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow == null)
            {
                CommonMethods.Notify("内部エラー．", MainWindow.NotificationType.Error);
                return;
            }

            // ブロック中のときはブロック解除を行う
            if (this.IsBlocking)
            {
                if (await mainWindow.ShowMessageAsync("確認", this.Name + "(@" + this.ScreenName + ")のブロックを解除しますか？", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    var user = await AccountTokens.DestroyBlockAsync(this.TimelineModel.TokenSuffix, this.Id);
                    if (user != null)
                    {
                        CommonMethods.Notify(this.Name + "(@" + this.ScreenName + ")のブロックを解除しました．", MainWindow.NotificationType.Success);
                        this.LoadRelationship(user);
                    }
                    else
                        CommonMethods.Notify(this.Name + "(@" + this.ScreenName + ")のブロック解除が正常に完了しませんでした．", MainWindow.NotificationType.Error);
                }
            }
            // フォロー中のときはフォロー解除を行う
            else if (this.IsFollowing)
            {
                if (await mainWindow.ShowMessageAsync("確認", this.Name + "(@" + this.ScreenName + ")のフォローを解除しますか？", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    var user = await AccountTokens.DestroyFriendshipAsync(this.TimelineModel.TokenSuffix, this.Id);
                    if (user != null)
                    {
                        CommonMethods.Notify(this.Name + "(@" + this.ScreenName + ")のフォローを解除しました．", MainWindow.NotificationType.Success);
                        this.LoadRelationship(user);
                    }
                    else
                        CommonMethods.Notify(this.Name + "(@" + this.ScreenName + ")のフォロー解除が正常に完了しませんでした．", MainWindow.NotificationType.Error);
                }
            }
            // フォローリクエスト承認待ちのときは謝罪文を表示する
            else if (this.IsSendingFollowRequest)
            {
                await mainWindow.ShowMessageAsync("申し訳ありません。", "フォローリクエストの解除はTwitter社からAPIが提供されていないため、公式のクライアント等から解除を行ってください。", MessageDialogStyle.Affirmative);
            }
            // その他の場合はフォロー(又はフォローリクエスト)を行う
            else
            {
                var text = "をフォロー";
                if (this.IsProtected)
                    text = "にフォローリクエストを送信";
                if (await mainWindow.ShowMessageAsync("確認", this.Name + "(@" + this.ScreenName + ")" + text + "しますか？", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    var user = await AccountTokens.CreateFriendshipAsync(this.TimelineModel.TokenSuffix, this.Id);
                    if (user != null)
                    {
                        CommonMethods.Notify(this.Name + "(@" + this.ScreenName + ")" + text + "しました．", MainWindow.NotificationType.Success);
                        this.LoadRelationship(user);
                    }
                    else
                        CommonMethods.Notify(this.Name + "(@" + this.ScreenName + ")のフォローが正常に完了しませんでした．", MainWindow.NotificationType.Error);
                }
            }
        }

        /// <summary>
        /// ユーザーのフォロー状況を取得
        /// </summary>
        /// <param name="user">相手ユーザーデータ</param>
        private async void LoadRelationship(User user)
        {
            if (user == null)
                return;

            var relationship = await AccountTokens.ShowRelationshipAsync(this.TimelineModel.TokenSuffix, user.Id);
            if (relationship != null)
            {
                if (relationship.Source.Id == relationship.Target.Id)
                    this.IsOwn = true;
                if (relationship.Source.IsBlocking != null)
                    this.IsBlocking = (bool)relationship.Source.IsBlocking;
                this.IsFollowing = relationship.Source.IsFollowing;
                if (relationship.Source.IsFollowingRequested != null)
                    this.IsSendingFollowRequest = (bool)relationship.Source.IsFollowingRequested;
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
        /// URLをクリックしたとき
        /// </summary>
        /// <param name="url">URL</param>
        private void SelectUrl(Uri url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url.ToString()));
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        #region FollowButtonVisibility 変更通知プロパティ
        public Visibility FollowButtonVisibility
        {
            get
            {
                if (this.IsOwn)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
        #endregion

        #region IsOwn 変更通知プロパティ
        public bool IsOwn
        {
            get
            {
                return this._IsOwn;
            }
            set
            {
                this._IsOwn = value;
                this.RaisePropertyChanged(nameof(this.FollowButtonVisibility));
                this.RaisePropertyChanged(nameof(this.HoveringFollowButtonText));
            }
        }
        private bool _IsOwn;
        #endregion

        #region IsBlocking 変更通知プロパティ
        public bool IsBlocking
        {
            get
            {
                return this._IsBlocking;
            }
            private set
            {
                this._IsBlocking = value;
                this.RaisePropertyChanged(nameof(this.FollowButtonText));
                this.RaisePropertyChanged(nameof(this.HoveringFollowButtonText));
            }
        }
        private bool _IsBlocking;
        #endregion

        #region IsFollowing 変更通知プロパティ
        public bool IsFollowing
        {
            get
            {
                return this._IsFollowing;
            }
            private set
            {
                this._IsFollowing = value;
                this.RaisePropertyChanged(nameof(this.FollowButtonText));
                this.RaisePropertyChanged(nameof(this.HoveringFollowButtonText));
            }
        }
        private bool _IsFollowing;
        #endregion

        #region IsFollowButtonHover 変更通知プロパティ
        public bool IsFollowButtonHover
        {
            get
            {
                return this._IsFollowButtonHover;
            }
            private set
            {
                this._IsFollowButtonHover = value;
            }
        }
        private bool _IsFollowButtonHover;
        #endregion

        #region IsProtected 変更通知プロパティ
        public new bool IsProtected
        {
            get
            {
                return this._IsProtected;
            }
            private set
            {
                this._IsProtected = value;
                this.RaisePropertyChanged(nameof(this.FollowButtonText));
                this.RaisePropertyChanged(nameof(this.HoveringFollowButtonText));
            }
        }
        private bool _IsProtected;
        #endregion

        #region IsMuting 変更通知プロパティ
        public bool IsMuting
        {
            get
            {
                return this._IsMuting;
            }
            private set
            {
                this._IsMuting = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsMuting;
        #endregion

        #region IsSendingFollowRequest 変更通知プロパティ
        public bool IsSendingFollowRequest
        {
            get
            {
                return this._IsSendingFollowRequest;
            }
            private set
            {
                this._IsSendingFollowRequest = value;
                this.RaisePropertyChanged(nameof(this.FollowButtonText));
                this.RaisePropertyChanged(nameof(this.HoveringFollowButtonText));
            }
        }
        private bool _IsSendingFollowRequest;
        #endregion

        #region FollowButtonText 変更通知プロパティ
        public string FollowButtonText
        {
            get
            {
                if (this._IsBlocking)
                    return "ブロック中";
                else if (this._IsFollowing)
                    return "フォロー中";
                else if (this._IsSendingFollowRequest)
                    return "承認待ち";
                else if (this._IsProtected)
                    return "フォローする(鍵)";
                else
                    return "フォローする";
            }
        }
        #endregion

        #region HoveringFollowButtonText 変更通知プロパティ
        public string HoveringFollowButtonText
        {
            get
            {
                if (this._IsBlocking)
                    return "ブロック解除";
                else if (this._IsFollowing)
                    return "フォロー解除";
                else if (this._IsSendingFollowRequest)
                    return "リクエスト解除";
                else if (this._IsProtected)
                    return "フォローする(鍵)";
                else
                    return "フォローする";
            }
        }
        #endregion

        public ICommand FollowCommand { get; }
        public ICommand UrlCommand { get; }

        public DateTimeOffset CreatedAt { get; }

        public ImageProperties ProfileBanner { get; }

        public Uri Url { get; }

        public HyperlinkTextProperties Description { get; }

        public int StatusesCount { get; }
        public int FavouritesCount { get; }
        public int FollowersCount { get; }
        public int FriendsCount { get; }

        public string UrlText { get; }

        private TimelineModel TimelineModel;
    }
}