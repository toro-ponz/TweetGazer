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
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                this.ShowImageViewModel.Previous();
            else
                this.ShowImageViewModel.Next();
        }

        /// <summary>
        /// 画像の拡大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ShowImageViewModel.IsZoom)
            {
                if (Double.IsNaN(this.ZoomImage.Width))
                    this.ZoomImage.Width = this.Image.ActualWidth * 3;
                if (Double.IsNaN(this.ZoomImage.Height))
                    this.ZoomImage.Height = this.Image.ActualHeight * 3;

                // 拡大部分のサイズは画像サイズの大きい方の2/5
                if (Double.IsNaN(this.ZoomScrollViewer.Width) || Double.IsNaN(this.ZoomScrollViewer.Height))
                {
                    if (this.Image.ActualHeight < this.Image.ActualWidth)
                    {
                        this.ZoomScrollViewer.Width = this.Image.ActualWidth / 2.5;
                        this.ZoomScrollViewer.Height = this.Image.ActualWidth / 2.5;
                    }
                    else
                    {
                        this.ZoomScrollViewer.Width = this.Image.ActualHeight / 2.5;
                        this.ZoomScrollViewer.Height = this.Image.ActualHeight / 2.5;
                    }
                }

                var widthRatio = this.ZoomImage.ActualWidth / this.Image.ActualWidth;
                var heightRatio = this.ZoomImage.ActualHeight / this.Image.ActualHeight;

                var widthOffset = 0.0d;
                var heightOffset = 0.0d;

                // 拡大部分が画面からはみ出るならマウスポインタの逆側に移動
                if (e.GetPosition(this.ZoomCanvas1).X + this.ZoomScrollViewer.Width > this.RenderSize.Width - 10)
                    widthOffset = this.ZoomScrollViewer.Width;
                if (e.GetPosition(this.ZoomCanvas1).Y + this.ZoomScrollViewer.Height > this.RenderSize.Height - 10)
                    heightOffset = this.ZoomScrollViewer.Height;

                Canvas.SetLeft(this.ZoomImage, (e.GetPosition(this.Image).X * -1) * widthRatio);
                Canvas.SetTop(this.ZoomImage, (e.GetPosition(this.Image).Y * -1) * heightRatio);
                Canvas.SetLeft(this.ZoomScrollViewer, e.GetPosition(this.ZoomCanvas1).X - widthOffset);
                Canvas.SetTop(this.ZoomScrollViewer, e.GetPosition(this.ZoomCanvas1).Y - heightOffset);

                this.ZoomImage.Visibility = Visibility.Visible;
            }
            else
            {
                this.ZoomImage.Visibility = Visibility.Hidden;
            }
        }

        private ShowImageViewModel ShowImageViewModel { get; }
    }
}