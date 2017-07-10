using Livet;
using System;
using System.Threading.Tasks;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class AccountSettingsParameters : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokenSuffix"></param>
        /// <param name="user"></param>
        public AccountSettingsParameters(int tokenSuffix, CoreTweet.User user)
        {
            this.TokenSuffix = tokenSuffix;

            this.OtherProfileIsChanged = false;
            this.ProfileImageIsChanged = false;
            this.ProfileBannerIsChanged = false;

            if (user == null)
                return;

            this._Name = user.Name;
            this._ScreenName = user.ScreenName;
            this._Description = user.Description;
            if (user.Location != null)
                this._Location = user.Location.ToString();
            this._Url = user.Url;

            if (!String.IsNullOrEmpty(user.ProfileImageUrlHttps))
                this._ProfileImage = new Uri(user.ProfileImageUrlHttps.Replace("_normal.", "_bigger."));

            if (!String.IsNullOrEmpty(user.ProfileBannerUrl))
                this._ProfileBanner = new Uri(user.ProfileBannerUrl);
        }

        /// <summary>
        /// プロフィールの再読み込み
        /// </summary>
        /// <param name="user"></param>
        private void ReloadProfile(CoreTweet.User user)
        {
            this.Name = user.Name;
            this.ScreenName = user.ScreenName;
            this.Description = user.Description;
            if (user.Location != null)
                this.Location = user.Location.ToString();
            this.Url = user.Url;

            this.OtherProfileIsChanged = false;
        }

        /// <summary>
        /// プロフィール画像の再読み込み
        /// </summary>
        /// <param name="user"></param>
        private void ReloadProfileImage(CoreTweet.User user)
        {
            if (!String.IsNullOrEmpty(user.ProfileImageUrlHttps))
                this.ProfileImage = new Uri(user.ProfileImageUrlHttps.Replace("_normal.", "_bigger."));

            this.ProfileImageIsChanged = false;
        }

        /// <summary>
        /// プロフィールバナーの再読み込み
        /// </summary>
        /// <param name="user"></param>
        private void ReloadProfileBanner(CoreTweet.User user)
        {
            if (!String.IsNullOrEmpty(user.ProfileBannerUrl))
                this.ProfileBanner = new Uri(user.ProfileBannerUrl);

            this.ProfileBannerIsChanged = false;
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            if (this.ProfileImageIsChanged)
            {
                if (await AccountTokens.UpdateProfileImageAsync(this.TokenSuffix, this.ProfileImage.OriginalString))
                {
                    CommonMethods.Notify("プロフィール画像の更新成功．", NotificationType.Success);
                    this.ReloadProfileImage(AccountTokens.Users[TokenSuffix]);
                }
                else
                {
                    CommonMethods.Notify("プロフィール画像の更新失敗．", NotificationType.Error);
                }
            }
            if (this.ProfileBannerIsChanged)
            {
                if (await AccountTokens.UpdateProfileBannerAsync(this.TokenSuffix, this.ProfileBanner.OriginalString))
                {
                    CommonMethods.Notify("プロフィールバナーの更新成功．", NotificationType.Success);
                    this.ReloadProfileBanner(AccountTokens.Users[TokenSuffix]);
                }
                else
                {
                    CommonMethods.Notify("プロフィールバナーの更新失敗．", NotificationType.Error);
                }
            }
            if (this.OtherProfileIsChanged)
            {
                if (await AccountTokens.UpdateProfileAsync(this.TokenSuffix, this.Name, this.Url, this.Location, this.Description))
                {
                    CommonMethods.Notify("プロフィールの更新成功．", NotificationType.Success);
                    this.ReloadProfile(AccountTokens.Users[this.TokenSuffix]);
                }
                else
                {
                    CommonMethods.Notify("プロフィールの更新失敗．", NotificationType.Error);
                }
            }
        }

        /// <summary>
        /// プロフィールバナーの削除
        /// </summary>
        /// <returns></returns>
        public async Task RemoveProfileBanner()
        {
            if (await AccountTokens.RemoveProfileBannerAsync(this.TokenSuffix))
            {
                CommonMethods.Notify("プロフィールバナーの削除成功．", NotificationType.Success);
            }
            else
            {
                CommonMethods.Notify("プロフィールバナーの削除失敗．", NotificationType.Error);
            }
        }

        #region ProfileImage 変更通知プロパティ
        public Uri ProfileImage
        {
            get
            {
                return this._ProfileImage;
            }
            set
            {
                this._ProfileImage = value;
                this.ProfileImageIsChanged = true;
                this.RaisePropertyChanged();
            }
        }
        private Uri _ProfileImage;
        #endregion

        #region ProfileBanner 変更通知プロパティ
        public Uri ProfileBanner
        {
            get
            {
                return this._ProfileBanner;
            }
            set
            {
                this._ProfileBanner = value;
                this.ProfileBannerIsChanged = true;
                this.RaisePropertyChanged();
            }
        }
        private Uri _ProfileBanner;
        #endregion

        #region Name 変更通知プロパティ
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
                this.OtherProfileIsChanged = true;
                this.RaisePropertyChanged();
            }
        }
        private string _Name;
        #endregion

        #region ScreenName 変更通知プロパティ
        public string ScreenName
        {
            get
            {
                return this._ScreenName;
            }
            set
            {
                this._ScreenName = value;
                this.OtherProfileIsChanged = true;
                this.RaisePropertyChanged();
            }
        }
        private string _ScreenName;
        #endregion

        #region Description 変更通知プロパティ
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
                this.OtherProfileIsChanged = true;
                this.RaisePropertyChanged();
            }
        }
        private string _Description;
        #endregion

        #region Location 変更通知プロパティ
        public string Location
        {
            get
            {
                return this._Location;
            }
            set
            {
                this._Location = value;
                this.OtherProfileIsChanged = true;
                this.RaisePropertyChanged();
            }
        }
        private string _Location;
        #endregion

        #region Url 変更通知プロパティ
        public string Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
                this.OtherProfileIsChanged = true;
                this.RaisePropertyChanged();
            }
        }
        private string _Url;
        #endregion

        private bool ProfileImageIsChanged;
        private bool ProfileBannerIsChanged;
        private bool OtherProfileIsChanged;

        private int TokenSuffix;
    }
}