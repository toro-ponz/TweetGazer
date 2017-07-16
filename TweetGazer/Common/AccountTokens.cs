using CoreTweet;
using CoreTweet.Core;
using CoreTweet.Streaming;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.Common
{
    public static class AccountTokens
    {
        public static IConnectableObservable<StreamingMessage> StartStreaming(int suffix, StreamingMode mode, string param = null)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                switch (mode)
                {
                    case StreamingMode.User:
                        return Tokens[suffix].Streaming.UserAsObservable(tweet_mode => "extended").Publish();
                    case StreamingMode.Filter:
                        if (param == null)
                            return null;
                        return Tokens[suffix].Streaming.FilterAsObservable(replies => "all", track => param).Publish();
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<DictionaryResponse<string, Dictionary<string, RateLimit>>> LoadRateLimitAsync(int suffix)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Application.RateLimitStatusAsync();
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<Status>> LoadHomeTimelineAsync(int suffix, long? maxId = null, long? sinceId = null)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Statuses.HomeTimelineAsync(count => LoadStatusCount, max_id => maxId, since_id => sinceId, tweet_mode => "extended");
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<Status>> LoadUserTimelineAsync(int suffix, long? userId, bool excludeReplies = true, long? maxId = null, long? sinceId = null, bool includeRts = true)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;
            if (userId == null)
                return null;

            try
            {
                return await Tokens[suffix].Statuses.UserTimelineAsync(count => LoadStatusCount, exclude_replies => excludeReplies.ToString(), include_rts => includeRts, max_id => maxId, since_id => sinceId, tweet_mode => "extended", user_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<Status>> LoadListTimelineAsync(int suffix, long? listId, long? maxId = null, long? sinceId = null)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;
            if (listId == null)
                return null;

            try
            {
                return await Tokens[suffix].Lists.StatusesAsync(count => LoadStatusCount, list_id => listId, max_id => maxId, since_id => sinceId, tweet_mode => "extended");
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<Status>> LoadMentionsTimelineAsync(int suffix, long? maxId = null)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Statuses.MentionsTimelineAsync(count => LoadStatusCount, max_id => maxId, tweet_mode => "extended");
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<Status>> LoadFavoritesAsync(int suffix, long? userId = null, long? maxId = null, long? sinceId = null)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Favorites.ListAsync(count => LoadStatusCount, max_id => maxId, since_id => sinceId, tweet_mode => "extended", user_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<SearchResult> LoadSearchTimelineAsync(int suffix, string query, long? maxId = null, long? sinceId = null, string includeEntities = "true", string resultType = "recent")
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Search.TweetsAsync(count => LoadStatusCount, max_id => maxId, q => query + " exclude:retweets", result_type => resultType, since_id => sinceId, tweet_mode => "extended", include_entities => includeEntities);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<List>> LoadListsAsync(int suffix, long? userId = null)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Lists.ListAsync(user_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<User>> LoadSearchedUsersAsync(int suffix, string query)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;
            if (query == null)
                return null;

            try
            {
                return await Tokens[suffix].Users.SearchAsync(count => LoadUsersCount, q => query);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<CoreTweet.DirectMessage>> LoadReceiveDirectMessagesAsync(int suffix)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].DirectMessages.ReceivedAsync(count => LoadMessageCount, include_entities => true);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<CoreTweet.DirectMessage>> LoadSentDirectMessagesAsync(int suffix)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].DirectMessages.SentAsync(count => LoadMessageCount, include_entities => true);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<TrendsResult>> LoadTrendsAsync(int suffix, long woeid = 23424856)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Trends.PlaceAsync(id => woeid);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<Cursored<User>> LoadFriendsAsync(int suffix, long? userId = null)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Friends.ListAsync(count => LoadUsersCount, user_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<UserResponse> ShowUserAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;
            if (userId == null)
                return null;

            try
            {
                return await Tokens[suffix].Users.ShowAsync(user_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<UserResponse> ShowUserAsync(int suffix, string screenName)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;
            if (screenName == null)
                return null;

            try
            {
                return await Tokens[suffix].Users.ShowAsync(screen_name => screenName);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<StatusResponse> ShowStatusAsync(int suffix, long statusId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Statuses.ShowAsync(id => statusId, trim_user => false, include_my_retweet => true, include_entities => true, tweet_mode => "extended");
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<Relationship> ShowRelationshipAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Friendships.ShowAsync(target_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<ListedResponse<Status>> LookupStatusAsync(int suffix, IEnumerable<long> statusIds)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Statuses.LookupAsync(id => statusIds, trim_user => false, include_my_retweet => true, include_entities => true, tweet_mode => "extended");
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<OAuth.OAuthSession> CreateAuthenticationSessionAsync()
        {
            try
            {
                var session = await OAuth.AuthorizeAsync(SecretParameters.ConsumerKey, SecretParameters.ConsumerSecret);
                return session;
            }
            catch (HttpRequestException e)
            {
                CommonMethods.Notify("ネットワークに正常に接続できませんでした．", NotificationType.Error);
                DebugConsole.Write(e);
                return null;
            }
            catch (WebException e)
            {
                CommonMethods.Notify("ネットワークに正常に接続できませんでした．", NotificationType.Error);
                DebugConsole.Write(e);
                return null;
            }
            catch (Exception e)
            {
                CommonMethods.Notify("エラー．", NotificationType.Error);
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<bool> TokenVerifyAsync(IList<Tokens> tokens)
        {
            if (tokens == null)
                return false;

            try
            {
                foreach (var token in tokens)
                {
                    Users.Add(await token.Account.VerifyCredentialsAsync());
                }
            }
            catch (TwitterException e)
            {
                CommonMethods.Notify("トークン認証失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            catch (HttpRequestException e)
            {
                CommonMethods.Notify("ネットワークに正常に接続できませんでした．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            catch (Exception e)
            {
                CommonMethods.Notify("エラー．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            CommonMethods.Notify("トークン認証成功．", NotificationType.Success);
            return true;
        }

        public static async Task<bool> CreateStatusAsync(int suffix,  string text, long? replyId = null, string mediaId = null )
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Statuses.UpdateAsync(status => text, in_reply_to_status_id => replyId, media_ids => mediaId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("ツイート失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            CommonMethods.Notify("ツイート完了．", NotificationType.Success);
            return true;
        }

        public static async Task<MediaUploadResult> ImageUploadAsync(int suffix, string filePath)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] bs = new byte[fs.Length];
                fs.Read(bs, 0, bs.Length);
                fs.Close();
                return await Tokens[suffix].Media.UploadAsync(bs);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("画像のアップロード失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<MediaUploadResult> VideoUploadAsync(int suffix, string filePath)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return await Tokens[suffix].Media.UploadChunkedAsync(fs, UploadMediaType.Video, media_category: "tweet_video");
            }
            catch (Exception e)
            {
                CommonMethods.Notify("動画のアップロード失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<bool> RetweetStatusAsync(int suffix, long statusId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Statuses.RetweetAsync(id => statusId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("リツイート失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            return true;
        }

        public static async Task<bool> UnretweetStatusAsync(int suffix, long statusId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Statuses.UnretweetAsync(id => statusId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("リツイート解除失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            return true;
        }

        public static async Task<bool> CreateFavoriteStatusAsync(int suffix, long statusId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Favorites.CreateAsync(id => statusId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("いいね失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            return true;
        }

        public static async Task<bool> DestroyFavoriteStatusAsync(int suffix, long statusId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Favorites.DestroyAsync(id => statusId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("いいね解除失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            return true;
        }

        public static async Task<UserResponse> CreateFriendshipAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Friendships.CreateAsync(user_id => userId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("フォロー失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<UserResponse> DestroyFriendshipAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Friendships.DestroyAsync(user_id => userId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("フォロー解除失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<UserResponse> CreateBlockAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Blocks.CreateAsync(user_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<UserResponse> DestroyBlockAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return null;

            try
            {
                return await Tokens[suffix].Blocks.DestroyAsync(user_id => userId);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return null;
            }
        }

        public static async Task<bool> CreateMuteAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Mutes.Users.CreateAsync(user_id => userId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("ミュート失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            CommonMethods.Notify("ミュート成功．", NotificationType.Normal);
            return true;
        }

        public static async Task<bool> DestroyMuteAsync(int suffix, long? userId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Mutes.Users.DestroyAsync(user_id => userId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("ミュート解除失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            CommonMethods.Notify("ミュート解除成功．", NotificationType.Normal);
            return true;
        }

        public static async Task<bool> DeleteStatusAsync(int suffix, long statusId)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Statuses.DestroyAsync(id => statusId);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("ツイートの削除失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            CommonMethods.Notify("ツイートの削除完了．", NotificationType.Success);
            return true;
        }

        public static async Task<bool> SetNotificationsAsync(int suffix, long? userId, bool isNotify, bool isIncludeRetweets = true)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Friendships.UpdateAsync(user_id => userId, device => isNotify, retweets => isIncludeRetweets);
            }
            catch (Exception e)
            {
                if (isNotify)
                    CommonMethods.Notify("指定ユーザーを通知リストに追加することができませんでした．", NotificationType.Error);
                else
                    CommonMethods.Notify("指定ユーザーを通知リストから削除することができませんでした．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            if (isNotify)
                CommonMethods.Notify("指定ユーザーを通知リストに追加しました．", NotificationType.Normal);
            else
                CommonMethods.Notify("指定ユーザーを通知リストから削除しました．", NotificationType.Normal);
            return true;
        }

        public static async Task<bool> UpdateProfileAsync(int suffix, string name, string url, string location, string description)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            UserResponse userResponse = null;
            try
            {
                userResponse = await Tokens[suffix].Account.UpdateProfileAsync(name, url, location, description, null, false, true);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return false;
            }

            if (userResponse == null)
                return false;

            Users[suffix] = userResponse;

            return true;
        }

        public static async Task<bool> UpdateProfileImageAsync(int suffix, string filePath)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            UserResponse userResponse = null;
            try
            {
                userResponse = await Tokens[suffix].Account.UpdateProfileImageAsync(image => new FileInfo(filePath));
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return false;
            }

            if (userResponse == null)
                return false;

            Users[suffix] = userResponse;

            return true;
        }

        public static async Task<bool> UpdateProfileBannerAsync(int suffix, string filePath)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Account.UpdateProfileBannerAsync(banner => new FileInfo(filePath));
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return false;
            }

            Users[suffix] = await ShowUserAsync(suffix, Users[suffix].Id);

            return true;
        }

        public static async Task<bool> RemoveProfileBannerAsync(int suffix)
        {
            if (Tokens == null || suffix >= Tokens.Count || suffix < 0)
                return false;

            try
            {
                await Tokens[suffix].Account.RemoveProfileBannerAsync();
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return false;
            }

            Users[suffix] = await ShowUserAsync(suffix, Users[suffix].Id);

            return true;
        }

        public static async Task<bool> AutheticationAsync(OAuth.OAuthSession session, string pin)
        {
            if (session == null || pin == null)
                return false;

            try
            {
                var token = await OAuth.GetTokensAsync(session, pin);
                await AddToken(token);
            }
            catch (Exception e)
            {
                CommonMethods.Notify("ログイン失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return false;
            }
            return true;
        }

        public static async Task<bool> LoadTokensAsync()
        {
            //ファイルの存在確認
            if (!File.Exists(SecretParameters.TokensFilePath))
                return false;

            IsVerifying = true;

            if (Tokens == null)
                Tokens = new List<Tokens>();

            var loadedText = "";
            //ファイルから読み込み
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(SecretParameters.TokensFilePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream))
                {
                    fileStream = null;
                    loadedText = CommonMethods.DecryptString(streamReader.ReadToEnd(), SecretParameters.TokensEncryptionKey);
                }
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                IsVerifying = false;
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }

            var regex = new Regex(@"AccessData\[(?<token>.+?), (?<secret>.+?)\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regex.Match(loadedText);
            while (match.Success)
            {
                Tokens.Add(
                    CoreTweet.Tokens.Create(
                        SecretParameters.ConsumerKey,
                        SecretParameters.ConsumerSecret,
                        match.Groups["token"].Value,
                        match.Groups["secret"].Value
                    )
                );
                match = match.NextMatch();
            }

            if (Tokens.Count < 1)
            {
                IsVerifying = false;
                return false;
            }

            //ユーザー情報の取得
            if (!await TokenVerifyAsync(Tokens))
            {
                Tokens.Clear();
                CommonMethods.Notify("トークンの認証失敗．", NotificationType.Error);
                IsVerifying = false;
                return false;
            }

            CommonMethods.Notify("トークン読み込み完了．", NotificationType.Success);

            IsVerifying = false;
            return true;
        }

        public static bool WriteTokens()
        {
            //書き込むトークン文字列を生成
            var writeText = "";
            foreach (var token in Tokens)
            {
                writeText += "AccessData[" + token.AccessToken + ", " + token.AccessTokenSecret + "]\n";
            }
            writeText = CommonMethods.EncryptString(writeText, SecretParameters.TokensEncryptionKey);

            //ディレクトリが存在しない場合作成する
            if (!Directory.GetParent(SecretParameters.TokensFilePath).Exists)
            {
                Directory.CreateDirectory(Directory.GetParent(SecretParameters.TokensFilePath).FullName);
            }

            //ファイルへの書き込み
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(SecretParameters.TokensFilePath, FileMode.Create, FileAccess.Write);
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    fileStream = null;
                    streamWriter.Write(writeText);
                }
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }

            return true;
        }

        public static async Task AddToken(Tokens token)
        {
            //既に追加されたアカウントか検証
            try
            {
                var addUser = await token.Account.VerifyCredentialsAsync();
                if (Users != null)
                {
                    foreach (var user in Users)
                    {
                        if (addUser.Id == user.Id)
                        {
                            CommonMethods.Notify("既に追加されているアカウントです．", NotificationType.Error);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CommonMethods.Notify("トークン認証失敗．", NotificationType.Error);
                DebugConsole.Write(e);
                return;
            }

            //ユーザー情報の取得
            if (!await TokenVerifyAsync(new List<Tokens>() { token }))
                return;

            if (Tokens == null)
                Tokens = new List<Tokens>();
            //トークンの追加
            Tokens.Add(token);

            //トークンの保存
            if (!WriteTokens())
            {
                CommonMethods.Notify("トークン書き込み失敗．", NotificationType.Error);
                return;
            }
            else
            {
                CommonMethods.Notify("トークン書き込み成功．", NotificationType.Success);
            }
            return;
        }

        public static ObservableCollection<User> Users
        {
            get
            {
                if (_Users == null)
                {
                    _Users = new ObservableCollection<User>();
                    System.Windows.Data.BindingOperations.EnableCollectionSynchronization(_Users, new object());
                }
                return _Users;
            }
            set
            {
                _Users = value;
            }
        }
        private static ObservableCollection<User> _Users;

        public static bool IsVerifying { get; private set; } = true;

        public static readonly int LoadStatusCount = 100;
        public static readonly int LoadMessageCount = 100;
        public static readonly int LoadUsersCount = 100;
        public static int TokensCount
        {
            get
            {
                if (Tokens == null)
                    Tokens = new List<Tokens>();
                return Tokens.Count;
            }
        }

        private static List<Tokens> Tokens;
    }

    public enum StreamingMode
    {
        User,
        Filter
    }
}