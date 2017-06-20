using SourceChord.Lighty;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TweetGazer.Behaviors;
using TweetGazer.ViewModels.ShowDialogs;

namespace TweetGazer.Views.ShowDialogs
{
    /// <summary>
    /// ShowImage.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowImage : UserControl, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="images">画像のリスト</param>
        /// <param name="suffix">最初に表示させる画像番号</param>
        public ShowImage(IList<ImageProperties> images, int suffix)
        {
            this.InitializeComponent();

            this.ShowImageViewModel = new ShowImageViewModel(images, suffix);
            this.DataContext = this.ShowImageViewModel;
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
            this.ShowImageViewModel.Dispose();
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
            if (!Properties.Settings.Default.IsCloseWhenClickImage)
                e.Handled = true;
        }

        /// <summary>
        /// イベントをブロックする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// マウスホイールを回転させたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.ShowImageViewModel.Previous();
            else
                this.ShowImageViewModel.Next();
        }

        private ShowImageViewModel ShowImageViewModel { get; }
    }
}