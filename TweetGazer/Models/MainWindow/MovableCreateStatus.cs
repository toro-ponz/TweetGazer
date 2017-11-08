namespace TweetGazer.Models.MainWindow
{
    public class MovableCreateStatus : CreateStatusBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MovableCreateStatus() : base()
        {

        }

        /// <summary>
        /// ツイートトレイの表示・非表示
        /// </summary>
        public override void ToggleOpen()
        {
            Properties.Settings.Default.IsVisibleCreateStatusTray = !Properties.Settings.Default.IsVisibleCreateStatusTray;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// ツイートトレイの開閉
        /// </summary>
        public void ToggleMinimize()
        {
            Properties.Settings.Default.IsMinimizeCreateStatusTray = !Properties.Settings.Default.IsMinimizeCreateStatusTray;
            Properties.Settings.Default.Save();
        }
    }
}
