using Livet;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class FlyoutBase : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FlyoutBase()
        {
            this._IsOpen = false;
            this.ScreenNames = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(this.ScreenNames, new object());
            this._TokenSuffix = -1;
        }

        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        public virtual void ToggleOpen()
        {
            if (!this.IsOpen && this.OpenConditions())
            {
                this.IsOpen = true;
                this.Open();
                this.TokenSuffix = 0;
            }
            else if (this.CloseConditions())
            {
                this.IsOpen = false;
                this.Close();
            }
        }

        /// <summary>
        /// Flyoutを開けるかどうかの判定
        /// </summary>
        /// <returns></returns>
        public virtual bool OpenConditions()
        {
            if (AccountTokens.TokensCount == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Flyoutを開くとき
        /// </summary>
        public virtual void Open()
        {

        }

        /// <summary>
        /// Flyoutを閉じれるかどうかの判定
        /// </summary>
        /// <returns></returns>
        public virtual bool CloseConditions()
        {
            return true;
        }

        /// <summary>
        /// Flyoutを閉じるとき
        /// </summary>
        public virtual void Close()
        {

        }

        #region TokenSuffix 変更通知プロパティ
        public virtual int TokenSuffix
        {
            get
            {
                return this._TokenSuffix;
            }
            set
            {
                this._TokenSuffix = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(ProfileImageUrl));
            }
        }
        protected int _TokenSuffix;
        #endregion

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

        public ObservableCollection<string> ScreenNames { get;  }
        public Uri ProfileImageUrl
        {
            get
            {
                if (AccountTokens.Users.Count == 0 || this.TokenSuffix < 0)
                    return new Uri(@"https://abs.twimg.com/sticky/default_profile_images/default_profile_bigger.png");
                else
                    return new Uri(AccountTokens.Users[this.TokenSuffix].ProfileImageUrlHttps.Replace("_normal.", "_bigger."));
            }
        }
    }
}