﻿using Livet;
using Livet.EventListeners;
using System;
using System.Windows;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.ViewModels.MainWindow
{
    public class InstructionsViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InstructionsViewModel()
        {
            this.Instructions = new Instructions();

            this._Visibility = this.Instructions.Visibility;
            this._PageNumber = this.Instructions.PageNumber;
            this.PagesCount = this.Instructions.PagesCount;

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.Instructions, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.Instructions.Visibility):
                            this.Visibility = this.Instructions.Visibility;
                            break;
                        case nameof(this.Instructions.PageNumber):
                            this.PageNumber = this.Instructions.PageNumber;
                            break;
                    }
                })
            );
        }

        /// <summary>
        /// 前のページへ戻る
        /// </summary>
        public void Previous()
        {
            this.Instructions.Previous();
        }

        /// <summary>
        /// 次のページへ進む
        /// </summary>
        public void Next()
        {
            this.Instructions.Next();
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public void Close()
        {
            this.Instructions.Close();
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

        #region Page 変更通知プロパティ
        public Uri Page
        {
            get
            {
                return this.Instructions.Pages[this._PageNumber];
            }
        }
        #endregion

        #region PageNumber 変更通知プロパティ
        public int PageNumber
        {
            get
            {
                return this._PageNumber + 1;
            }
            set
            {
                this._PageNumber = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.Page));
            }
        }
        private int _PageNumber;
        #endregion

        public int PagesCount { get; }

        private Instructions Instructions;
    }
}
