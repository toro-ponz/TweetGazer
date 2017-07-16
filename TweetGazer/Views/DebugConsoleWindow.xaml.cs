using System.Windows;

namespace TweetGazer.Views
{
    /// <summary>
    /// DebugConsoleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DebugConsoleWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DebugConsoleWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 閉じるボタン押下時にCloseをせずにVisibilityを変える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                e.Cancel = true;
                this.Visibility = Visibility.Collapsed;
            }
        }
    }
}