using Livet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class NetworkState : NotificationObject, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NetworkState()
        {
            this._MainProfileImage = new Uri(@"https://abs.twimg.com/sticky/default_profile_images/default_profile_bigger.png");
            this._IsOnline = false;
            this._IsInternetOnline = false;
            this._IsTwitterOnline = false;

            this.Timers = new List<Timer>();
            var timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(this.Check);
            timer.Interval = 10000;
            timer.AutoReset = true;
            timer.Enabled = true;
            this.Timers.Add(timer);

            this.Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private async void Initialize()
        {
            this.Check();

            await Task.Run(() =>
            {
                while (AccountTokens.Users.Count == 0)
                {
                    System.Threading.Thread.Sleep(100);
                }
            });

            this.MainProfileImage = new Uri(AccountTokens.Users[0].ProfileImageUrlHttps.Replace("_normal.", "_bigger."));
        }

        /// <summary>
        /// ネットワーク状態等の確認
        /// </summary>
        private async void Check()
        {
            await Task.Run(() =>
            {
                this.IsInternetOnline = CommonMethods.CheckInternetConnectEnabled();
                this.IsTwitterOnline = CommonMethods.CheckTwitterConnectEnabled();
            });

            this.IsOnline = this.IsInternetOnline && this.IsTwitterOnline;
        }

        /// <summary>
        /// ネットワーク状態等の確認
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Check(object sender, EventArgs e)
        {
            this.Check();
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

            //タイマーを削除
            if (this.Timers != null)
            {
                foreach (var timer in this.Timers)
                {
                    timer.Stop();
                }
            }
        }

        #region MainProfileImage 変更通知プロパティ
        public Uri MainProfileImage
        {
            get
            {
                return this._MainProfileImage;
            }
            set
            {
                this._MainProfileImage = value;
                this.RaisePropertyChanged();
            }
        }
        private Uri _MainProfileImage;
        #endregion

        #region IsOnline 変更通知プロパティ
        public bool IsOnline
        {
            get
            {
                return this._IsOnline;
            }
            set
            {
                this._IsOnline = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsOnline;
        #endregion

        #region IsInternetOnline 変更通知プロパティ
        public bool IsInternetOnline
        {
            get
            {
                return this._IsInternetOnline;
            }
            set
            {
                this._IsInternetOnline = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsInternetOnline;
        #endregion

        #region IsTwitterOnline 変更通知プロパティ
        public bool IsTwitterOnline
        {
            get
            {
                return this._IsTwitterOnline;
            }
            set
            {
                this._IsTwitterOnline = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsTwitterOnline;
        #endregion

        private List<Timer> Timers;
    }
}
