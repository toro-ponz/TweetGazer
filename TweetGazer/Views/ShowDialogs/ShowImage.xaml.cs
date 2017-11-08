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

            this.Type = ZoomType.Normal;
            this.Dragging = false;
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
            {
                e.Handled = true;
            }
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
            {
                this.ResetZoom();
                this.ShowImageViewModel.Previous();
            }
            else
            {
                this.ResetZoom();
                this.ShowImageViewModel.Next();
            }

            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetZoom()
        {
            this.Image.Visibility = Visibility.Visible;
            this.ZoomedImageScrollViewer.Visibility = Visibility.Collapsed;
            this.ZoomedImage.Width = this.Image.ActualWidth;
            this.ZoomedImage.Height = this.Image.ActualHeight;
            this.Type = ZoomType.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetZoomButton_Click(object sender, RoutedEventArgs e)
        {
            this.ResetZoom();

            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.Type)
            {
                case ZoomType.Normal:
                    this.ZoomedImage.Width = this.Image.ActualWidth * 2;
                    this.ZoomedImage.Height = this.Image.ActualHeight * 2;
                    this.Image.Visibility = Visibility.Collapsed;
                    this.ZoomedImageScrollViewer.Visibility = Visibility.Visible;
                    this.Type = ZoomType.TwoTimes;
                    break;
                case ZoomType.TwoTimes:
                    this.ZoomedImage.Width = this.ZoomedImage.ActualWidth / 2 * 3;
                    this.ZoomedImage.Height = this.ZoomedImage.ActualHeight / 2 * 3;
                    this.Type = ZoomType.ThreeTimes;
                    break;
                case ZoomType.ThreeTimes:
                    this.ResetZoom();
                    break;
            }

            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomedImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Dragging)
            {
                if (this.Point == e.GetPosition(this))
                {
                    return;
                }

                var p = this.Point - e.GetPosition(this);
                this.ZoomedImageScrollViewer.ScrollToHorizontalOffset(this.ZoomedImageScrollViewer.HorizontalOffset + p.X);
                this.ZoomedImageScrollViewer.ScrollToVerticalOffset(this.ZoomedImageScrollViewer.VerticalOffset + p.Y);
                this.Point = e.GetPosition(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomedImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Dragging = true;
            this.Point = e.GetPosition(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomedImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Dragging = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomedImage_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Dragging = false;
        }

        private enum ZoomType
        {
            Normal,
            TwoTimes,
            ThreeTimes
        }
        private ZoomType Type;

        private Point Point;
        private bool Dragging;

        private ShowImageViewModel ShowImageViewModel { get; }
    }
}
