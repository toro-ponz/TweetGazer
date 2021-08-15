namespace TweetGazer.ViewModels.MainWindow
{
    public interface IFlyoutViewModel
    {
        /// <summary>
        /// Flyoutの開閉
        /// </summary>
        void ToggleOpen();

        /// <summary>
        /// Flyoutの開閉状態
        /// </summary>
        bool IsOpen
        {
            get;
            set;
        }
    }
}
