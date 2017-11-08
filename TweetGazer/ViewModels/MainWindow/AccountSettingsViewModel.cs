using Livet;
using Livet.EventListeners;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class AccountSettingsViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AccountSettingsViewModel() : base()
        {
            this.AccountSettings = new AccountSettings();

            this.ScreenNames = this.AccountSettings.ScreenNames;
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.AccountSettings, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.AccountSettings.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpen);
                            break;
                        case nameof(this.AccountSettings.TokenSuffix):
                            this.RaisePropertyChanged(() => this.TokenSuffix);
                            this.RaisePropertyChanged(() => this.Name);
                            this.RaisePropertyChanged(() => this.ScreenName);
                            this.RaisePropertyChanged(() => this.Description);
                            this.RaisePropertyChanged(() => this.Location);
                            this.RaisePropertyChanged(() => this.Url);
                            this.RaisePropertyChanged(() => this.ProfileImage);
                            this.RaisePropertyChanged(() => this.ProfileBanner);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(AccountTokens.Users, (_, __) => this.AccountSettings.ReloadUsers())
            );

            this.CloseCommand = new RelayCommand(this.ToggleOpen);
        }

        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        public void ToggleOpen()
        {
            this.AccountSettings.ToggleOpen();
        }

        /// <summary>
        /// アカウントの設定を保存する
        /// </summary>
        public void Save()
        {
            this.AccountSettings.Save();
        }

        /// <summary>
        /// プロフィール画像を選択する
        /// </summary>
        public void SelectProfileImage()
        {
            this.AccountSettings.SelectProfileImage();
        }

        /// <summary>
        /// プロフィールバナー画像を選択する
        /// </summary>
        public void SelectProfileBanner()
        {
            this.AccountSettings.SelectProfileBanner();
        }

        /// <summary>
        /// プロフィールバナー画像を削除する
        /// </summary>
        public void RemoveProfileBanner()
        {
            this.AccountSettings.RemoveProfileBanner();
        }

        #region TokenSuffix 変更通知プロパティ
        public int TokenSuffix
        {
            get
            {
                return this.AccountSettings.TokenSuffix;
            }
            set
            {
                this.AccountSettings.TokenSuffix = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Name 変更通知プロパティ
        public string Name
        {
            get
            {
                if (this.AccountSettings.User == null)
                {
                    return "";
                }

                return this.AccountSettings.User.Name;
            }
            set
            {
                this.AccountSettings.User.Name = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ScreenName 変更通知プロパティ
        public string ScreenName
        {
            get
            {
                if (this.AccountSettings.User == null)
                {
                    return "";
                }

                return this.AccountSettings.User.ScreenName;
            }
            set
            {
                this.AccountSettings.User.ScreenName = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Description 変更通知プロパティ
        public string Description
        {
            get
            {
                if (this.AccountSettings.User == null)
                {
                    return "";
                }

                return this.AccountSettings.User.Description;
            }
            set
            {
                this.AccountSettings.User.Description = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Location 変更通知プロパティ
        public string Location
        {
            get
            {
                if (this.AccountSettings.User == null)
                {
                    return "";
                }

                return this.AccountSettings.User.Location;
            }
            set
            {
                this.AccountSettings.User.Location = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Url 変更通知プロパティ
        public string Url
        {
            get
            {
                if (this.AccountSettings.User == null)
                {
                    return "";
                }

                return this.AccountSettings.User.Url;
            }
            set
            {
                this.AccountSettings.User.Url = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ProfileImage 変更通知プロパティ
        public Uri ProfileImage
        {
            get
            {
                if (this.AccountSettings.User == null)
                {
                    return new Uri("", UriKind.Relative);
                }

                return this.AccountSettings.User.ProfileImage;
            }
        }
        #endregion

        #region ProfileBanner 変更通知プロパティ
        public Uri ProfileBanner
        {
            get
            {
                if (this.AccountSettings.User == null)
                {
                    return new Uri("", UriKind.Relative);
                }

                return this.AccountSettings.User.ProfileBanner;
            }
        }
        #endregion

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.AccountSettings.IsOpen;
            }
            set
            {
                this.AccountSettings.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand CloseCommand { get; }

        public ObservableCollection<string> ScreenNames { get; }

        private AccountSettings AccountSettings;
    }
}
