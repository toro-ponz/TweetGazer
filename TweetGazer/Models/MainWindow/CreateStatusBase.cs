using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using TweetGazer.Behaviors;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class CreateStatusBase : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CreateStatusBase()
        {
            this._IsProgressRingVisible = false;
            this._IsSelectButtonEnabled = true;
            this._IsDeleteButtonVisible = false;

            this.Users = new ObservableCollection<CreateStatusUserProperties>();
            this.ReloadUsers();
            BindingOperations.EnableCollectionSynchronization(this.Users, new object());
            this.FileNames = new ObservableCollection<string>();

            this._CaretPosition = CaretPosition.Undefined;
            this._StatusText = "";
            this._ReplyText = "リプライ先:なし";
            this.ReplyId = null;
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

            if (Properties.Settings.Default.IsCloseAfterCreateStatusCorrect && this.IsOpen)
            {
                this.ToggleOpen();
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
            if (!IsSelectButtonEnabled)
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
                    CommonMethods.Notify("ファイルタイプエラー．", NotificationType.Error);
                    return;
                }
            }

            //異なるタイプのメディアファイルが混在していないか判定
            var mediaType = mediaTypes.First();
            foreach (var mt in mediaTypes)
            {
                if (mediaType != mt || (Type != MediaType.Undefined && Type != mt))
                {
                    CommonMethods.Notify("異なるメディアを同時にアップロードすることはできません．", NotificationType.Error);
                    return;
                }
            }

            if (this.Type == MediaType.Undefined)
                this.Type = mediaType;

            if (this.FileNames.Count + ofd.FileNames.Count() > 4)
            {
                CommonMethods.Notify("画像は4枚までです．", NotificationType.Error);
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
            foreach (var user in Users)
                user.Media.Clear();

            this.FileNames.Clear();
            Type = MediaType.Undefined;

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
        /// ユーザーの再読み込み
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

        /// <summary>
        /// 指定ユーザー(のみ)をツイート状態に
        /// </summary>
        /// <param name="suffix">ユーザー番号</param>
        public void SelectUser(int suffix)
        {
            int i = 0;
            foreach (var user in Users)
            {
                user.IsCreate = false;
                if (i == suffix)
                    user.IsCreate = true;
                i++;
            }
        }

        #region CaretPosition 変更通知プロパティ
        public CaretPosition CaretPosition
        {
            get
            {
                return this._CaretPosition;
            }
            set
            {
                this._CaretPosition = value;
                this.RaisePropertyChanged();
            }
        }
        private CaretPosition _CaretPosition;
        #endregion

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

        #region ReplyText 変更通知プロパティ
        public string ReplyText
        {
            get
            {
                return this._ReplyText;
            }
            set
            {
                this._ReplyText = value;
                this.RaisePropertyChanged();
            }
        }
        private string _ReplyText;
        #endregion

        private MediaType Type;

        public ObservableCollection<CreateStatusUserProperties> Users { get; }
        public ObservableCollection<string> FileNames { get; }

        public long? ReplyId;
    }
}