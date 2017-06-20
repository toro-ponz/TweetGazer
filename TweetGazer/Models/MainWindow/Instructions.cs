using Livet;
using System;
using System.Collections.Generic;
using System.Windows;

namespace TweetGazer.Models.MainWindow
{
    public class Instructions : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Instructions()
        {
            this.Pages = new List<Uri>()
            {
                new Uri("pack://application:,,,/Views/Pages/Instructions/Instruction1.xaml"),
                new Uri("pack://application:,,,/Views/Pages/Instructions/Instruction2.xaml"),
            };
            this._PageNumber = 0;
            this.PagesCount = this.Pages.Count;

            if (Common.CommonMethods.CheckFirstBoot())
                this._Visibility = Visibility.Visible;
            else
                this._Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 前のページへ戻る
        /// </summary>
        public void Previous()
        {
            if (this.PageNumber >= 1)
                this.PageNumber--;
        }

        /// <summary>
        /// 次のページへ進む
        /// </summary>
        public void Next()
        {
            if (this.PageNumber == this.PagesCount - 1)
                this.Close();
            else
                this.PageNumber++;
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            this.Visibility = Visibility.Collapsed;
            this.PageNumber = 0;
        }

        #region Visibility 変更通知プロパティ
        public Visibility Visibility
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                this._Visibility = value;
                this.RaisePropertyChanged();
            }
        }
        private Visibility _Visibility;
        #endregion

        #region PageNumber 変更通知プロパティ
        public int PageNumber
        {
            get
            {
                return this._PageNumber;
            }
            set
            {
                this._PageNumber = value;
                this.RaisePropertyChanged();
            }
        }
        private int _PageNumber;
        #endregion

        public IList<Uri> Pages { get; }

        public int PagesCount { get; }
    }
}