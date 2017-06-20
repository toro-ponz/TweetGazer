using SourceChord.Lighty;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TweetGazer.Views.ShowDialogs
{
    /// <summary>
    /// ShowCopiableText.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowCopiableText : UserControl, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="text">表示する文字列</param>
        public ShowCopiableText(string text)
        {
            this.InitializeComponent();

            this.TextBox.Text = text;
            this.TextBox.GotFocus += (sender, e) =>
            {
                this.TextBox.SelectAll();
                e.Handled = true;
            };
            this.SetFocus();
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

            }

            this.Close();
        }

        /// <summary>
        /// テキストボックスにフォーカスする
        /// </summary>
        private async void SetFocus()
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(500);
            });

            this.TextBox.Focus();
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
            (sender as Button).Focus();
            e.Handled = true;
        }
    }
}