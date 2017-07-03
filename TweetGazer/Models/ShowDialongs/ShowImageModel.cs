using Livet;
using System.Collections.Generic;
using TweetGazer.Behaviors;

namespace TweetGazer.Models.ShowDialongs
{
    public class ShowImageModel : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="images">画像のリスト</param>
        /// <param name="suffix">最初に表示させる画像番号</param>
        public ShowImageModel(IList<ImageProperties> images, int suffix)
        {
            this._ImageSuffix = suffix;
            this.Images = images;
            if (this.Images == null)
                this.Images = new List<ImageProperties>();
            this.ImageCount = this.Images.Count;

            if (this.ImageSuffix > this.ImageCount - 1)
                return;
        }

        /// <summary>
        /// 表示画像を変更する
        /// </summary>
        /// <param name="plus">正の方向に変えるかどうか</param>
        private void ChangeImage(bool plus)
        {
            if (!plus)
            {
                if (this.ImageSuffix != 0)
                    this.ImageSuffix--;
            }
            else
            {
                if (this.ImageSuffix != this.ImageCount - 1)
                    this.ImageSuffix++;
            }

        }

        /// <summary>
        /// 1ページ戻る
        /// </summary>
        public void Previous()
        {
            this.ChangeImage(false);
        }

        /// <summary>
        /// 1ページ進む
        /// </summary>
        public void Next()
        {
            this.ChangeImage(true);
        }

        /// <summary>
        /// ズームの切り替え
        /// </summary>
        public void ToggleZoom()
        {
            this.IsZoom = !this._IsZoom;
        }

        #region Image 変更通知プロパティ
        public ImageProperties Image
        {
            get
            {
                return this.Images[this.ImageSuffix];
            }
        }
        private IList<ImageProperties> Images;
        public int ImageSuffix
        {
            get
            {
                return this._ImageSuffix;
            }
            set
            {
                this._ImageSuffix = value;
                this.RaisePropertyChanged(nameof(this.Image));
                this.RaisePropertyChanged(nameof(this.IsLeftButtonVisible));
                this.RaisePropertyChanged(nameof(this.IsRightButtonVisible));
            }
        }
        private int _ImageSuffix;
        #endregion

        #region IsLeftButtonVisible 変更通知プロパティ
        public bool IsLeftButtonVisible
        {
            get
            {
                if (this.ImageSuffix > 0)
                    return true;
                return false;
            }
        }
        #endregion

        #region IsRightButtonVisible 変更通知プロパティ
        public bool IsRightButtonVisible
        {
            get
            {
                if (this.ImageSuffix < this.ImageCount - 1)
                    return true;
                return false;
            }
        }
        #endregion

        #region IsZoom 変更通知プロパティ
        public bool IsZoom
        {
            get
            {
                return this._IsZoom;
            }
            set
            {
                this._IsZoom = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsZoom;
        #endregion

        public int ImageCount;
    }
}