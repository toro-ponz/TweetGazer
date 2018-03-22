using CoreTweet;
using Livet;
using System;
using System.Threading.Tasks;
using TweetGazer.Common;
using TweetGazer.ViewModels;

namespace TweetGazer.Models.MainWindow
{
    public class AddAccount : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AddAccount(MainWindowViewModel mainWindowViewModel)
        {
            this.MainWindowViewModel = mainWindowViewModel;

            this._Pin = "";
            this._Message = "";
            this._Session = null;
            this.AuthenticationUrl = null;
            this._IsOpen = CommonMethods.CheckFirstBoot();
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
            this.IsOpen = true;
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
                this.MainWindowViewModel.StartStreaming();
                this.Close();
            }
            else
            {
                this.Pin = "";
                this.Message = "認証に失敗しました\nURLクリックからやり直してください";
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
            {
                await this.CreateSession();
            }

            if (this._Session == null || this.AuthenticationUrl == null)
            {
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(this.AuthenticationUrl.ToString());
                this.AuthenticationUrl = null;
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        private void Close()
        {
            if (AccountTokens.Users.Count != 0)
            {
                this.IsOpen = false;
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
            {
                this.AuthenticationUrl = this._Session.AuthorizeUri;
            }
            else
            {
                this.Message = "セッションの生成に失敗しました。\n認証ボタンを押し、再度セッションを生成してください。";
            }
        }

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this._IsOpen;
            }
            set
            {
                this._IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsOpen;
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
                {
                    return false;
                }

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
                this.RaisePropertyChanged(nameof(this.IsPinTextBoxEnabled));
            }
        }
        private OAuth.OAuthSession _Session;
        #endregion

        private Uri AuthenticationUrl;

        private MainWindowViewModel MainWindowViewModel;
    }
}
