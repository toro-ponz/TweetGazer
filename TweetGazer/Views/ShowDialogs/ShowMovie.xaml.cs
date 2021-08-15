using SourceChord.Lighty;
using System;
using System.Windows;
using System.Windows.Controls;
using Gu.Wpf.Media;
using System.Threading.Tasks;
using TweetGazer.Common;
using System.IO;

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

            this.DownloadVideo(url);
        }

        /// <summary>
        /// 動画のダウンロード
        /// </summary>
        /// <param name="url">動画のURL</param>
        private async void DownloadVideo(Uri url)
        {
            if (url == null)
            {
                return;
            }

            string fileName = DateTime.Now.Ticks.ToString() + new Random().Next().ToString() + ".mp4";
            var filePath = Directory.GetCurrentDirectory() + SecretParameters.TemporaryDirectoryPath + fileName;

            await Task.Run(() =>
            {
                var wc = new System.Net.WebClient();

                //ディレクトリが存在しない場合作成する
                if (!Directory.GetParent(filePath).Exists)
                {
                    Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
                }

                try
                {
                    wc.DownloadFile(url, SecretParameters.TemporaryDirectoryPath + fileName);
                }
                catch (Exception ex)
                {
                    DebugConsole.Write(ex);
                }
                finally
                {
                    wc.Dispose();
                }
            });

            this.MediaElement.SetCurrentValue(MediaElementWrapper.SourceProperty, new Uri(filePath));
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
                this.MediaElement.IsPlaying = false;
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
    }
}
