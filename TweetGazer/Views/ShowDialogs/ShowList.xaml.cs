using SourceChord.Lighty;
using System;
using System.Windows;
using System.Windows.Controls;
using TweetGazer.Models.Timeline;
using TweetGazer.ViewModels.ShowDialogs;

namespace TweetGazer.Views.ShowDialogs
{
    /// <summary>
    /// ShowList.xaml の相互作用ロジック
    /// </summary>
    public partial class ShowList : UserControl, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokenSuffix">アカウント番号</param>
        /// <param name="user">表示対象のユーザー</param>
        public ShowList(int tokenSuffix, UserProperties user)
        {
            InitializeComponent();

            this.ShowListViewModel = new ShowListViewModel(tokenSuffix, user);
            this.DataContext = this.ShowListViewModel;
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
            this.ShowListViewModel.Dispose();
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

        private ShowListViewModel ShowListViewModel;
    }
}
