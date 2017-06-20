using Livet;
using Livet.EventListeners;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class DirectMessagesViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DirectMessagesViewModel()
        {
            DirectMessages = new DirectMessages();

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.DirectMessages, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.DirectMessages.IsOpen):
                            this.RaisePropertyChanged(() => this.IsOpen);
                            break;
                    }
                })
            );

            this.CloseCommand = new RelayCommand(this.ToggleOpen);
        }

        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        public void ToggleOpen()
        {
            this.DirectMessages.ToggleOpen();
        }

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.DirectMessages.IsOpen;
            }
            set
            {
                this.DirectMessages.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand CloseCommand { get; }

        private DirectMessages DirectMessages;
    }
}