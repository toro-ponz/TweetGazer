using SourceChord.Lighty;
using System;
using System.Windows;
using System.Windows.Controls;
using TweetGazer.Behaviors;

namespace TweetGazer.Views.ShowDialogs
{
    /// <summary>
    /// ShowMovie.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowMovie : UserControl, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="url">動画のURL</param>
        /// <param name="isGif">アニメーションGIFかどうか</param>
        public ShowMovie(Uri url, bool isGif = false)
        {
            this.InitializeComponent();

            this.DataContext = url;
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        /// <param name="disposing">マネージリソースを破棄するか否か</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DataContext = null;
            }

            this.Close();
        }

        /// <summary>
        /// 閉じるボタンをクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
            e.Handled = true;
        }

        /// <summary>
        /// 背面をクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// 再生ボタンをクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayAndPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoMediaElementBehavior.GetState(this.MediaElement) == States.Playing)
            {
                this.PlayIcon.Visibility = Visibility.Visible;
                this.PauseIcon.Visibility = Visibility.Collapsed;
            }
            else if (VideoMediaElementBehavior.GetState(this.MediaElement) == States.Pauseing)
            {
                this.PlayIcon.Visibility = Visibility.Collapsed;
                this.PauseIcon.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 最小化ボタンをクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// ミュートボタンをクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoMediaElementBehavior.GetIsMuted(this.MediaElement))
            {
                this.VolumeIcon.Visibility = Visibility.Visible;
                this.MuteIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.VolumeIcon.Visibility = Visibility.Collapsed;
                this.MuteIcon.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// リピートボタンをクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (VideoMediaElementBehavior.GetIsLoop(this.MediaElement))
                this.NotRepeat.Visibility = Visibility.Visible;
            else
                this.NotRepeat.Visibility = Visibility.Collapsed;
        }
    }
}