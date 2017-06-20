using Livet;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class MovableCreateStatus : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MovableCreateStatus() : base()
        {
            this._IsProgressRingVisible = false;
            this._IsSelectButtonEnabled = true;
            this._IsDeleteButtonVisible = false;
            this._IsMinimized = false;
            this._IsTrayVisible = true;

            this.Users = new ObservableCollection<CreateStatusUserProperties>();
            this.ReloadUsers();
            BindingOperations.EnableCollectionSynchronization(this.Users, new object());
            this.FileNames = new ObservableCollection<string>();

            this._StatusText = "";
            this.ReplyId = null;
        }

        /// <summary>
        /// ツイートトレイの表示・非表示
        /// </summary>
        public void ToggleOpen()
        {
            this.IsTrayVisible = !this._IsTrayVisible;
        }

        /// <summary>
        /// ツイートトレイの開閉
        /// </summary>
        public void ToggleMinimize()
        {
            this.IsMinimized = !this._IsMinimized;
        }

        /// <summary>
        /// ツイートの送信
        /// </summary>
        public async void Create()
        {
            this.IsProgressRingVisible = true;

            foreach (var user in Users)
            {
                if (user.IsCreate)
                {
                    if (this.IsDeleteButtonVisible && !user.Media.IsUploaded)
                    {
                        if (!(await user.Upload(this.Type, this.FileNames.ToList())))
                        {
                            this.IsProgressRingVisible = false;
                            return;
                        }
                    }

                    if (await AccountTokens.CreateStatusAsync(user.TokenSuffix, this.StatusText, this.ReplyId, user.Media.Ids))
                    {
                        user.Media.Clear();
                    }
                    else
                    {
                        this.IsProgressRingVisible = false;
                        return;
                    }
                }
            }

            this.StatusText = "";
            this.FileNames.Clear();
            this.Type = MediaType.Undefined;
            this.IsSelectButtonEnabled = true;
            this.IsDeleteButtonVisible = false;
            this.IsProgressRingVisible = false;
        }

        /// <summary>
        /// メディアの選択
        /// </summary>
        public void SelectMedia()
        {
            if (!this.IsSelectButtonEnabled)
                return;

            //ファイルダイアログを表示
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "メディアファイル(*.jpg;*.jpeg;.*.png;*.gif;*.mp4)|*.jpg;*.jpeg;*.png;*.gif;*.mp4",
                Multiselect = true,
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == false)
                return;

            var mediaTypes = new List<MediaType>();

            //選択したファイルのタイプ判別
            foreach (var filePath in ofd.FileNames)
            {
                var imageRegex = new Regex(@"^.+\.(jpe?g|png)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var imageRegexMatch = imageRegex.Match(filePath);
                var gifRegex = new Regex(@"^.+\.gif$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var gifRegexMatch = gifRegex.Match(filePath);
                var videoRegex = new Regex(@"^.+\.mp4$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var videoRegexMatch = videoRegex.Match(filePath);

                //画像ファイルの時
                if (imageRegexMatch.Success)
                    mediaTypes.Add(MediaType.Image);
                //GIFファイルの時
                else if (gifRegexMatch.Success)
                {
                    using (var bitmap = new System.Drawing.Bitmap(filePath))
                    {
                        //アニメーションGIFの場合
                        if (System.Drawing.ImageAnimator.CanAnimate(bitmap))
                            mediaTypes.Add(MediaType.Gif);
                        else
                            mediaTypes.Add(MediaType.Image);
                    }
                }
                //動画ファイルの場合
                else if (videoRegexMatch.Success)
                    mediaTypes.Add(MediaType.Video);
                else
                {
                    CommonMethods.Notify("ファイルタイプエラー．", NoticeType.Error);
                    return;
                }
            }

            //異なるタイプのメディアファイルが混在していないか判定
            var mediaType = mediaTypes.First();
            foreach (var mt in mediaTypes)
            {
                if (mediaType != mt || (this.Type != MediaType.Undefined && this.Type != mt))
                {
                    CommonMethods.Notify("異なるメディアを同時にアップロードすることはできません．", NoticeType.Error);
                    return;
                }
            }

            if (this.Type == MediaType.Undefined)
                this.Type = mediaType;

            if (this.FileNames.Count + ofd.FileNames.Count() > 4)
            {
                CommonMethods.Notify("画像は4枚までです．", NoticeType.Error);
                return;
            }

            foreach (var fileName in ofd.FileNames)
            {
                this.FileNames.Add(fileName);
            }

            this.IsProgressRingVisible = true;

            if (mediaType == MediaType.Gif || mediaType == MediaType.Video || this.FileNames.Count() >= 4)
            {
                this.IsSelectButtonEnabled = false;
            }

            this.IsDeleteButtonVisible = true;
            this.IsProgressRingVisible = false;
        }

        /// <summary>
        /// メディアの削除
        /// </summary>
        public void DeleteMedia()
        {
            foreach (var user in this.Users)
                user.Media.Clear();

            this.FileNames.Clear();
            this.Type = MediaType.Undefined;

            this.IsSelectButtonEnabled = true;
            this.IsDeleteButtonVisible = false;
        }

        /// <summary>
        /// Ctrl+Enterが押されたとき
        /// </summary>
        public void PressCtrlEnter()
        {
            if (Properties.Settings.Default.IsCreateStatusWhenPressCtrlEnter)
                this.Create();
        }

        /// <summary>
        /// ユーザーを再読み込みする
        /// </summary>
        public void ReloadUsers()
        {
            this.Users.Clear();
            var i = 0;
            foreach (var user in AccountTokens.Users)
            {
                this.Users.Add(new CreateStatusUserProperties(i, user));
                i++;
            }
        }

        #region IsProgressRingVisible 変更通知プロパティ
        public bool IsProgressRingVisible
        {
            get
            {
                return this._IsProgressRingVisible;
            }
            private set
            {
                this._IsProgressRingVisible = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsProgressRingVisible;
        #endregion

        #region IsDeleteButtonVisible 変更通知プロパティ
        public bool IsDeleteButtonVisible
        {
            get
            {
                return this._IsDeleteButtonVisible;
            }
            private set
            {
                this._IsDeleteButtonVisible = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsDeleteButtonVisible;
        #endregion

        #region IsSelectButtonEnabled 変更通知プロパティ
        public bool IsSelectButtonEnabled
        {
            get
            {
                return this._IsSelectButtonEnabled;
            }
            private set
            {
                this._IsSelectButtonEnabled = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsSelectButtonEnabled;
        #endregion

        #region IsTrayVisible 変更通知プロパティ
        public bool IsTrayVisible
        {
            get
            {
                return this._IsTrayVisible;
            }
            private set
            {
                this._IsTrayVisible = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsTrayVisible;
        #endregion

        #region IsMinimized 変更通知プロパティ
        public bool IsMinimized
        {
            get
            {
                return this._IsMinimized;
            }
            private set
            {
                this._IsMinimized = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsMinimized;
        #endregion

        #region StatusText 変更通知プロパティ
        public string StatusText
        {
            get
            {
                return this._StatusText;
            }
            set
            {
                this._StatusText = value;
                this.RaisePropertyChanged();
            }
        }
        private string _StatusText;
        #endregion

        private MediaType Type;

        public ObservableCollection<CreateStatusUserProperties> Users { get; }
        public ObservableCollection<string> FileNames { get; }

        public long? ReplyId;
    }
}