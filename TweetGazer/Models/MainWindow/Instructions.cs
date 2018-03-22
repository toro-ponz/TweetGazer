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
            this._IsOpen = Common.CommonMethods.CheckFirstBoot();
        }

        /// <summary>
        /// 前のページへ戻る
        /// </summary>
        public void Previous()
        {
            if (this.PageNumber >= 1)
            {
                this.PageNumber--;
            }
        }

        /// <summary>
        /// 次のページへ進む
        /// </summary>
        public void Next()
        {
            if (this.PageNumber == this.PagesCount - 1)
            {
                this.Close();
            }
            else
            {
                this.PageNumber++;
            }
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            this.IsOpen = false;
            this.PageNumber = 0;
        }

        #region IsOpen 変更通知プロパティ
        public bool IsOpen
        {
            get
            {
                return this._IsOpen;
            }
            set
            {
                this._IsOpen = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsOpen;
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
