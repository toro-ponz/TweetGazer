using Livet;
using Livet.EventListeners;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class MentionsViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MentionsViewModel()
        {
            Mentions = new Mentions();

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Mentions, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Mentions.IsOpen):
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
            this.Mentions.ToggleOpen();
        }

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.Mentions.IsOpen;
            }
            set
            {
                this.Mentions.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand CloseCommand { get; }

        private Mentions Mentions;
    }
}