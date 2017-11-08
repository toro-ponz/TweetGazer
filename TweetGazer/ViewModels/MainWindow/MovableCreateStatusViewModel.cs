using Livet;
using Livet.EventListeners;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class MovableCreateStatusViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MovableCreateStatusViewModel()
        {
            this.MovableCreateStatus = new MovableCreateStatus();
            this.Users = this.MovableCreateStatus.Users;
            this.FileNames = this.MovableCreateStatus.FileNames;
            this._IsProgressRingVisible = this.MovableCreateStatus.IsProgressRingVisible;
            this._IsSelectButtonEnabled = this.MovableCreateStatus.IsSelectButtonEnabled;
            this._IsDeleteButtonVisible = this.MovableCreateStatus.IsDeleteButtonVisible;

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.MovableCreateStatus, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.MovableCreateStatus.IsProgressRingVisible):
                            this.IsProgressRingVisible = this.MovableCreateStatus.IsProgressRingVisible;
                            break;
                        case nameof(this.MovableCreateStatus.IsDeleteButtonVisible):
                            this.IsDeleteButtonVisible = this.MovableCreateStatus.IsDeleteButtonVisible;
                            break;
                        case nameof(this.MovableCreateStatus.IsSelectButtonEnabled):
                            this.IsSelectButtonEnabled = this.MovableCreateStatus.IsSelectButtonEnabled;
                            break;
                        case nameof(this.MovableCreateStatus.StatusText):
                            this.RaisePropertyChanged(() => this.StatusText);
                            break;
                    }
                })
            );
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(Common.AccountTokens.Users, (_, __) => this.MovableCreateStatus.ReloadUsers())
            );

            this.PressCtrlEnterCommand = new Common.RelayCommand(this.MovableCreateStatus.PressCtrlEnter);
        }

        /// <summary>
        /// トレイの開閉
        /// </summary>
        public void ToggleOpen()
        {
            this.MovableCreateStatus.ToggleOpen();
        }

        /// <summary>
        /// トレイの最小化最大化
        /// </summary>
        public void ToggleMinimize()
        {
            this.MovableCreateStatus.ToggleMinimize();
        }

        /// <summary>
        /// ツイートの送信
        /// </summary>
        public void Create()
        {
            this.MovableCreateStatus.Create();
        }

        /// <summary>
        /// ツイートするメディア(画像・動画)の選択
        /// </summary>
        public void SelectMedia()
        {
            this.MovableCreateStatus.SelectMedia();
        }

        /// <summary>
        /// ツイートするメディアの削除
        /// </summary>
        public void DeleteMedia()
        {
            this.MovableCreateStatus.DeleteMedia();
        }

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
                return this.MovableCreateStatus.StatusText;
            }
            set
            {
                this.MovableCreateStatus.StatusText = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand PressCtrlEnterCommand { get; }

        public ObservableCollection<CreateStatusUserProperties> Users { get;}
        public ObservableCollection<string> FileNames { get; }

        private MovableCreateStatus MovableCreateStatus;
    }
}
