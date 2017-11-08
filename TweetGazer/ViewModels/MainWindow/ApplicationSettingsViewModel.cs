using Livet;
using Livet.EventListeners;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class ApplicationSettingsViewModel : ViewModel, IFlyoutViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationSettingsViewModel() : base()
        {
            this.ApplicationSettings = new ApplicationSettings();

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.ApplicationSettings, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.ApplicationSettings.IsOpen):
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
            this.ApplicationSettings.ToggleOpen();
        }

        /// <summary>
        /// Flyoutを閉じる
        /// </summary>
        public void Close()
        {
            if (this.ApplicationSettings.IsOpen)
            {
                this.ApplicationSettings.ToggleOpen();
            }
        }

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this.ApplicationSettings.IsOpen;
            }
            set
            {
                this.ApplicationSettings.IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand CloseCommand { get; }

        private ApplicationSettings ApplicationSettings;
    }
}
