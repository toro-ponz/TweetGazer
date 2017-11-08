using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class StatusMedia : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StatusMedia()
        {
            this._Type = MediaType.Undefined;
            this.IsUploaded = false;
            this._Source = new ObservableCollection<Uri>();
            this._IsEnabled = false;
            this.PlayCommand = new RelayCommand<object>(this.PlayCommandEntity);
            this.LoopPlayCommand = new RelayCommand<object>(this.LoopPlayCommandEntity);
        }

        /// <summary>
        /// アップロード予定のメディアを削除する
        /// </summary>
        public void Clear()
        {
            this.Type = MediaType.Undefined;
            this.Source.Clear();
            this.Ids = null; ;
            this.IsUploaded = false;
            this.IsEnabled = false;
        }

        /// <summary>
        /// 画像をアップロードする
        /// </summary>
        /// <param name="tokenSuffix">アップロードするアカウント番号</param>
        /// <param name="filePaths">ファイルパス</param>
        /// <returns></returns>
        public async Task<bool> ImageUpload(int tokenSuffix, IReadOnlyCollection<string> filePaths)
        {
            if (this.IsUploaded)
            {
                this.Clear();
            }

            if (filePaths == null || filePaths.Count() == 0)
            {
                return false;
            }

            if (filePaths.Count() > 4 - this.Source.Count)
            {
                CommonMethods.Notify("画像ファイルは4枚までです", NotificationType.Error);
                return false;
            }

            foreach (var filePath in filePaths)
            {
                var media = await AccountTokens.ImageUploadAsync(tokenSuffix, filePath);
                if (media != null && media.Size != 0)
                {
                    if (this.Ids == null)
                    {
                        this.Ids = media.MediaId.ToString();
                    }
                    else
                    {
                        this.Ids += "," + media.MediaId.ToString();
                    }

                    this.IsEnabled = true;
                    CommonMethods.Notify("画像アップロード完了", NotificationType.Success);
                    this.Source.Add(new Uri(filePath));
                    this.Type = MediaType.Image;
                }
                else
                {
                    CommonMethods.Notify("画像アップロード失敗", NotificationType.Error);
                    return false;
                }
            }

            this.IsUploaded = true;
            return true;
        }

        /// <summary>
        /// アニメーションGIFをアップロードする
        /// </summary>
        /// <param name="tokenSuffix">アップロードするアカウント番号</param>
        /// <param name="filePaths">ファイルパス</param>
        /// <returns></returns>
        public async Task<bool> AnimationGifUpload(int tokenSuffix, IReadOnlyCollection<string> filePaths)
        {
            if (this.IsUploaded)
            {
                this.Clear();
            }

            if (filePaths == null || filePaths.Count() == 0)
            {
                return false;
            }

            if (filePaths.Count() > 1 - this.Source.Count)
            {
                CommonMethods.Notify("アニメーションGIFファイルは1枚までです", NotificationType.Error);
                return false;
            }

            foreach (var filePath in filePaths)
            {
                var media = await AccountTokens.ImageUploadAsync(tokenSuffix, filePath);
                if (media != null && media.Size != 0)
                {
                    this.Ids = media.MediaId.ToString();

                    this.IsEnabled = true;
                    CommonMethods.Notify("アニメーションGIFアップロード完了", NotificationType.Success);
                    this.Source.Add(new Uri(filePath));
                    this.Type = MediaType.Gif;
                }
                else
                {
                    CommonMethods.Notify("アニメーションGIFアップロード失敗", NotificationType.Error);
                    return false;
                }
                break;
            }

            this.IsUploaded = true;
            return true;
        }

        /// <summary>
        /// 動画をアップロードする
        /// </summary>
        /// <param name="tokenSuffix">アップロードするアカウント番号</param>
        /// <param name="filePaths">ファイルパス</param>
        /// <returns></returns>
        public async Task<bool> VideoUpload(int tokenSuffix, IReadOnlyCollection<string> filePaths)
        {
            if (this.IsUploaded)
            {
                this.Clear();
            }

            if (filePaths == null || filePaths.Count == 0)
            {
                return false;
            }

            if (filePaths.Count > 1 - this.Source.Count)
            {
                CommonMethods.Notify("動画は1つまでです", NotificationType.Error);
                return false;
            }

            foreach (var filePath in filePaths)
            {
                var media = await AccountTokens.VideoUploadAsync(tokenSuffix, filePath);
                if (media != null && media.Size != 0)
                {
                    this.Ids = media.MediaId.ToString();
                    this.IsEnabled = true;
                    CommonMethods.Notify("動画アップロード完了", NotificationType.Success);
                    this.Source.Add(new Uri(filePath));
                    this.Type = MediaType.Video;
                }
                else
                {
                    CommonMethods.Notify("動画アップロード失敗", NotificationType.Error);
                    return false;
                }
                break;
            }

            this.IsUploaded = true;
            return true;
        }

        /// <summary>
        /// アニメーションGIF・動画のプレビュー再生
        /// </summary>
        /// <param name="sender">MediaElement</param>
        private void PlayCommandEntity(object sender)
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.FromMilliseconds(1);
                try
                {
                    mediaElement.Play();
                }
                catch (Exception e)
                {
                    DebugConsole.Write(e);
                }
            }
        }

        /// <summary>
        /// アニメーションGIF・動画のループ再生
        /// </summary>
        /// <param name="sender">MediaElement</param>
        private void LoopPlayCommandEntity(object sender)
        {
            if (this.IsGif)
            {
                this.PlayCommandEntity(sender);
            }
        }

        #region Type 変更通知プロパティ
        public MediaType Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.UploadButtonIsEnabled));
            }
        }
        private MediaType _Type;
        #endregion

        #region DeleteButtonVisibility 変更通知プロパティ
        public Visibility DeleteButtonVisibility
        {
            get
            {
                if (this.IsEnabled)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }
        #endregion

        #region Source 変更通知プロパティ
        public ObservableCollection<Uri> Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                this._Source = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.UploadButtonIsEnabled));
                this.RaisePropertyChanged(nameof(this.DeleteButtonVisibility));
            }
        }
        private ObservableCollection<Uri> _Source;
        #endregion

        #region IsEnabled 変更通知プロパティ
        public bool IsEnabled
        {
            get
            {
                return this._IsEnabled;
            }
            set
            {
                this._IsEnabled = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.DeleteButtonVisibility));
            }
        }
        private bool _IsEnabled;
        #endregion

        #region UploadButtonIsEnabled 変更通知プロパティ
        public bool UploadButtonIsEnabled
        {
            get
            {
                switch (this.Type)
                {
                    case MediaType.Gif:
                    case MediaType.Video:
                        return false;
                    default:
                        return true;
                }
            }
        }
        #endregion

        public ICommand PlayCommand { get; }
        public ICommand LoopPlayCommand { get; }

        public string Ids;
        public bool IsUploaded { get; set; }
        public bool IsGif
        {
            get
            {
                if (this.Type == MediaType.Gif)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
