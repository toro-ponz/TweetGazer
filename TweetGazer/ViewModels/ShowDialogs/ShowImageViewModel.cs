using Livet;
using Livet.EventListeners;
using System.Collections.Generic;
using System.Windows.Input;
using TweetGazer.Behaviors;
using TweetGazer.Common;
using TweetGazer.Models.ShowDialogs;

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
                        case nameof(this.ShowImage.IsZoom):
                            this.RaisePropertyChanged(() => this.IsZoom);
                            break;
                    }
                })
            );

            this.PreviousCommand = new RelayCommand(this.Previous);
            this.NextCommand = new RelayCommand(this.Next);
            this.ToggleZoomCommand = new RelayCommand(this.ToggleZoom);
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

        /// <summary>
        /// ズームの切り替え
        /// </summary>
        public void ToggleZoom()
        {
            this.ShowImage.ToggleZoom();
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

        #region IsZoom 変更通知プロパティ
        public bool IsZoom
        {
            get
            {
                return this.ShowImage.IsZoom;
            }
        }
        #endregion

        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand ToggleZoomCommand { get; }

        private ShowImageModel ShowImage;
    }
}