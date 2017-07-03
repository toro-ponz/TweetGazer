using CoreTweet;
using Livet;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class AddAccount : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AddAccount()
        {
            this._Pin = "";
            this._Message = "";
            this._Session = null;
            this.AuthenticationUrl = null;
            if (CommonMethods.CheckFirstBoot())
                this._Visibility = Visibility.Visible;
            else
                this._Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 開く
        /// </summary>
        public void Open()
        {
            this.Pin = "";
            this.Message = "";
            this.Session = null;
            this.AuthenticationUrl = null;
            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// アカウント追加をやめる
        /// </summary>
        public void Cancel()
        {
            this.Close();
        }

        /// <summary>
        /// 認証
        /// </summary>
        public async void Authentication()
        {
            if (await AccountTokens.AutheticationAsync(this._Session, this.Pin))
            {
                this.Close();
            }
            else
            {
                this.Pin = "";
                this.Message = "認証に失敗しました．\nURLクリックからやり直してください．";
                this.Session = null;
                this.AuthenticationUrl = null;
            }
        }

        /// <summary>
        /// アプリ連携URLを開く
        /// </summary>
        public async void OpenAuthenticationUrl()
        {
            if (this._Session == null || this.AuthenticationUrl == null)
                await this.CreateSession();

            if (this._Session == null || this.AuthenticationUrl == null)
                return;

            try
            {
                System.Diagnostics.Process.Start(this.AuthenticationUrl.ToString());
                this.AuthenticationUrl = null;
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        private void Close()
        {
            if (AccountTokens.Users.Count != 0)
            {
                this.Visibility = Visibility.Collapsed;
                this.Pin = "";
                this.Message = "";
                this.Session = null;
                this.AuthenticationUrl = null;
            }
        }

        /// <summary>
        /// セッション生成
        /// </summary>
        private async Task CreateSession()
        {
            this.Session = await AccountTokens.CreateAuthenticationSessionAsync();
            if (this._Session != null)
                this.AuthenticationUrl = this._Session.AuthorizeUri;
            else
                this.Message = "セッションの生成に失敗しました。\n認証ボタンを押し、再度セッションを生成してください。";
        }

        #region Visibility 変更通知プロパティ
        public Visibility Visibility
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                this._Visibility = value;
                this.RaisePropertyChanged();
            }
        }
        private Visibility _Visibility;
        #endregion

        #region Pin 変更通知プロパティ
        public string Pin
        {
            get
            {
                return this._Pin;
            }
            set
            {
                this._Pin = value;
                this.RaisePropertyChanged();
            }
        }
        private string _Pin;
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

        #region IsPinTextBoxEnabled 変更通知プロパティ
        public bool IsPinTextBoxEnabled
        {
            get
            {
                if (this._Session == null)
                    return false;
                return true;
            }
        }
        #endregion

        #region Session 変更通知プロパティ
        private OAuth.OAuthSession Session
        {
            set
            {
                this._Session = value;
                this.RaisePropertyChanged(nameof(IsPinTextBoxEnabled));
            }
        }
        private OAuth.OAuthSession _Session;
        #endregion

        private Uri AuthenticationUrl;
    }
}