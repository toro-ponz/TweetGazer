using Livet;
using Livet.EventListeners;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TweetGazer.Behaviors;
using TweetGazer.Common;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class CreateStatusViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateStatusViewModel() : base()
        {
            this.CreateStatus = new CreateStatus();

            this.Users = this.CreateStatus.Users;
            this.FileNames = this.CreateStatus.FileNames;
            this._IsProgressRingVisible = this.CreateStatus.IsProgressRingVisible;
            this._IsSelectButtonEnabled = this.CreateStatus.IsSelectButtonEnabled;
            this._IsDeleteButtonVisible = this.CreateStatus.IsDeleteButtonVisible;

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.CreateStatus, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.CreateStatus.CaretPosition):
                            this.CaretPosition = this.CreateStatus.CaretPosition;
                            break;
                        case nameof(this.CreateStatus.IsProgressRingVisible):
                            this.IsProgressRingVisible = this.CreateStatus.IsProgressRingVisible;
                            break;
                        case nameof(this.CreateStatus.IsDeleteButtonVisible):
                            this.IsDeleteButtonVisible = this.CreateStatus.IsDeleteButtonVisible;
                            break;
                        case nameof(this.CreateStatus.IsSelectButtonEnabled):
                            this.IsSelectButtonEnabled = this.CreateStatus.IsSelectButtonEnabled;
                            break;
                        case nameof(this.CreateStatus.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpen);
                            break;
                        case nameof(this.CreateStatus.StatusText):
                            this.RaisePropertyChanged(() => this.StatusText);
                            break;
                        case nameof(this.CreateStatus.ReplyText):
                            this.RaisePropertyChanged(() => this.ReplyText);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(AccountTokens.Users, (_, __) => this.CreateStatus.ReloadUsers())
            );

            this.PressCtrlEnterCommand = new RelayCommand(this.CreateStatus.PressCtrlEnter);
            this.CloseCommand = new RelayCommand(this.ToggleOpen);
        }

        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        public void ToggleOpen()
        {
            this.CreateStatus.ToggleOpen();
        }

        /// <summary>
        /// ツイートの送信
        /// </summary>
        public void Create()
        {
            this.CreateStatus.Create();
        }

        /// <summary>
        /// ツイートするメディア(画像：動画)の選択
        /// </summary>
        public void SelectMedia()
        {
            this.CreateStatus.SelectMedia();
        }

        /// <summary>
        /// メディアの削除
        /// </summary>
        public void DeleteMedia()
        {
            this.CreateStatus.DeleteMedia();
        }

        /// <summary>
        /// 指定したアカウントをツイート可能状態にする
        /// </summary>
        /// <param name="suffix">アカウント番号</param>
        public void SelectUser(int suffix)
        {
            this.CreateStatus.SelectUser(suffix);
        }

        #region CaretPosition 変更通知プロパティ
        public CaretPosition CaretPosition
        {
            get
            {
                return this._CaretPosition;
            }
            set
            {
                this._CaretPosition = value;
                this.RaisePropertyChanged();
            }
        }
        private CaretPosition _CaretPosition;
        #endregion

        #region IsProgressRingVisible 変更通知プロパティ
        public bool IsProgressRingVisible
        {
            get
            {
                return this._IsProgressRingVisible;
            }
            set
            {
                this._IsProgressRingVisible = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsProgressRingVisible;
        #endregion

        #region IsDeleteButtonVisible 変更通知プロパティ
        public bool IsDeleteButtonVisible
        {
            get
            {
                return this._IsDeleteButtonVisible;
            }
            set
            {
                this._IsDeleteButtonVisible = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsDeleteButtonVisible;
        #endregion

        #region IsSelectButtonEnabled 変更通知プロパティ
        public bool IsSelectButtonEnabled
        {
            get
            {
                return this._IsSelectButtonEnabled;
            }
            set
            {
                this._IsSelectButtonEnabled = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsSelectButtonEnabled;
        #endregion

        #region StatusText 変更通知プロパティ
        public string StatusText
        {
            get
            {
                return this.CreateStatus.StatusText;
            }
            set
            {
                this.CreateStatus.StatusText = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ReplyText 変更通知プロパティ
        public string ReplyText
        {
            get
            {
                return this.CreateStatus.ReplyText;
            }
            set
            {
                this.CreateStatus.ReplyText = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ReplyId 変更通知プロパティ
        public long? ReplyId
        {
            get
            {
                return this.CreateStatus.ReplyId;
            }
            set
            {
                this.CreateStatus.ReplyId = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.CreateStatus.IsOpen;
            }
            set
            {
                this.CreateStatus.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand PressCtrlEnterCommand { get; }
        public ICommand CloseCommand { get; }

        public ObservableCollection<CreateStatusUserProperties> Users { get; }
        public ObservableCollection<string> FileNames { get; }

        private CreateStatus CreateStatus;
    }
}