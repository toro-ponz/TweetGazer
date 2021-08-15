using Livet;
using Livet.EventListeners;
using System;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class NetworkStateViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NetworkStateViewModel()
        {
            this.NetworkState = new NetworkState();

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.NetworkState, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.NetworkState.MainProfileImage):
                            this.RaisePropertyChanged(() => this.MainProfileImage);
                            break;
                        case nameof(this.NetworkState.IsOnline):
                            this.RaisePropertyChanged(() => this.IsOnline);
                            break;
                        case nameof(this.NetworkState.IsInternetOnline):
                            this.RaisePropertyChanged(() => this.IsInternetOnline);
                            break;
                        case nameof(this.NetworkState.IsTwitterOnline):
                            this.RaisePropertyChanged(() => this.IsTwitterOnline);
                            break;
                    }
                })
            );

            this.CompositeDisposable.Add(this.NetworkState);
        }

        #region MainProfileImage 変更通知プロパティ
        public Uri MainProfileImage
        {
            get
            {
                return this.NetworkState.MainProfileImage;
            }
        }
        #endregion

        #region IsOnline 変更通知プロパティ
        public bool IsOnline
        {
            get
            {
                return this.NetworkState.IsOnline;
            }
        }
        #endregion

        #region IsInternetOnline 変更通知プロパティ
        public bool IsInternetOnline
        {
            get
            {
                return this.NetworkState.IsInternetOnline;
            }
        }
        #endregion

        #region IsTwitterOnline 変更通知プロパティ
        public bool IsTwitterOnline
        {
            get
            {
                return this.NetworkState.IsTwitterOnline;
            }
        }
        #endregion

        private NetworkState NetworkState;
    }
}
