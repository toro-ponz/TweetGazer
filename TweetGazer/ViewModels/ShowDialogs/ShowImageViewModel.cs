﻿using Livet;
using Livet.EventListeners;
using System.Collections.Generic;
using System.Windows.Input;
using TweetGazer.Behaviors;
using TweetGazer.Common;
using TweetGazer.Models.ShowDialongs;

namespace TweetGazer.ViewModels.ShowDialogs
{
    public class ShowImageViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="images">画像のリスト</param>
        /// <param name="suffix">最初に表示させる画像番号</param>
        public ShowImageViewModel(IList<ImageProperties> images, int suffix)
        {
            this.ShowImage = new ShowImageModel(images, suffix);

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.ShowImage, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.ShowImage.Image):
                            this.RaisePropertyChanged(() => this.Image);
                            break;
                        case nameof(this.ShowImage.IsLeftButtonVisible):
                            this.RaisePropertyChanged(() => this.IsLeftButtonVisible);
                            break;
                        case nameof(this.ShowImage.IsRightButtonVisible):
                            this.RaisePropertyChanged(() => this.IsRightButtonVisible);
                            break;
                    }
                })
            );

            this.PreviousCommand = new RelayCommand(this.Previous);
            this.NextCommand = new RelayCommand(this.Next);
        }

        /// <summary>
        /// 1ページ戻る
        /// </summary>
        public void Previous()
        {
            this.ShowImage.Previous();
        }

        /// <summary>
        /// 1ページ進む
        /// </summary>
        public void Next()
        {
            this.ShowImage.Next();
        }

        #region Image 変更通知プロパティ
        public ImageProperties Image
        {
            get
            {
                return this.ShowImage.Image;
            }
        }
        #endregion

        #region IsLeftButtonVisible 変更通知プロパティ
        public bool IsLeftButtonVisible
        {
            get
            {
                return this.ShowImage.IsLeftButtonVisible;
            }
        }
        #endregion

        #region IsRightButtonVisible 変更通知プロパティ
        public bool IsRightButtonVisible
        {
            get
            {
                return this.ShowImage.IsRightButtonVisible;
            }
        }
        #endregion

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        private ShowImageModel ShowImage;
    }
}