using Livet;
using Livet.EventListeners;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class NoticeViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NoticeViewModel()
        {
            this.Notice = new Notice();

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Notice, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Notice.IsOpen):
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
            this.Notice.ToggleOpen();
        }

        #region IsOpen 変更通知プロパティ
        public virtual bool IsOpen
        {
            get
            {
                return this.Notice.IsOpen;
            }
            set
            {
                this.Notice.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand CloseCommand { get; }

        private Notice Notice;
    }
}