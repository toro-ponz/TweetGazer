using Livet;
using Livet.EventListeners;
using System.Windows;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class AddAccountViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AddAccountViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.AddAccount = new AddAccount(mainWindowViewModel);
            this._Visibility = this.AddAccount.Visibility;
            this._IsPinTextBoxEnabled = false;
            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.AddAccount, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.AddAccount.Visibility):
                            this.Visibility = this.AddAccount.Visibility;
                            break;
                        case nameof(this.AddAccount.Pin):
                            this.RaisePropertyChanged(() => this.Pin);
                            break;
                        case nameof(this.AddAccount.Message):
                            this.Message = this.AddAccount.Message;
                            break;
                        case nameof(this.AddAccount.IsPinTextBoxEnabled):
                            this.IsPinTextBoxEnabled = this.AddAccount.IsPinTextBoxEnabled;
                            break;
                    }
                })
            );
        }

        /// <summary>
        /// 開く
        /// </summary>
        public void Open()
        {
            this.AddAccount.Open();
        }

        /// <summary>
        /// アカウント追加をやめる
        /// </summary>
        public void Cancel()
        {
            this.AddAccount.Cancel();
        }
        
        /// <summary>
        /// 認証
        /// </summary>
        public void Authentication()
        {
            this.AddAccount.Authentication();
        }

        /// <summary>
        /// アプリ連携URLを開く
        /// </summary>
        public void OpenAuthenticationUrl()
        {
            this.AddAccount.OpenAuthenticationUrl();
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
                return this.AddAccount.Pin;
            }
            set
            {
                this.AddAccount.Pin = value;
                this.RaisePropertyChanged();
            }
        }
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
                return this._IsPinTextBoxEnabled;
            }
            set
            {
                this._IsPinTextBoxEnabled = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsPinTextBoxEnabled;
        #endregion

        private AddAccount AddAccount;
    }
}