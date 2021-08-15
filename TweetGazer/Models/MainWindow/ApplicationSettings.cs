namespace TweetGazer.Models.MainWindow
{
    public class ApplicationSettings : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationSettings() : base()
        {

        }

        /// <summary>
        /// Flyoutを閉じるとき
        /// </summary>
        public override void Close()
        {
            Properties.Settings.Default.Save();
        }
    }
}
