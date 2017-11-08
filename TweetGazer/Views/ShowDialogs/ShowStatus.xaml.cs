using SourceChord.Lighty;
using System;
using System.Windows;
using System.Windows.Controls;
using TweetGazer.Models;
using TweetGazer.ViewModels.ShowDialogs;

namespace TweetGazer.Views.ShowDialogs
{
    /// <summary>
    /// ShowStatus.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowStatus : UserControl, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">遷移元のTimelineModel</param>
        /// <param name="id">ツイートID</param>
        public ShowStatus(TimelineModel timelineModel, long id)
        {
            this.InitializeComponent();

            this.ShowStatusViewModel = new ShowStatusViewModel(timelineModel, id);
            this.DataContext = this.ShowStatusViewModel;
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
            this.ShowStatusViewModel.Dispose();
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
        
        private ShowStatusViewModel ShowStatusViewModel;
    }
} 
