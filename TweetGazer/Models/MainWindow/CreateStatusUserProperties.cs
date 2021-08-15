using Livet;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Models.MainWindow
{
    public class CreateStatusUserProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokenSuffix">アカウント番号</param>
        /// <param name="user">ユーザーデータ</param>
        public CreateStatusUserProperties(int tokenSuffix, CoreTweet.User user)
        {
            this.User = new UserOverviewProperties(user);
            this.Media = new StatusMedia();

            this.TokenSuffix = tokenSuffix;
            if (tokenSuffix == 0)
            {
                this._IsCreate = true;
            }
            else
            {
                this._IsCreate = false;
            }
        }

        /// <summary>
        /// メディアをアップロードする
        /// </summary>
        /// <param name="mediaType">メディアタイプ</param>
        /// <param name="fileNames">ファイルパス</param>
        /// <returns></returns>
        public async Task<bool> Upload(MediaType mediaType, IReadOnlyCollection<string> fileNames)
        {
            if (!this.IsCreate)
            {
                return false;
            }

            switch (mediaType)
            {
                //画像ファイル
                case MediaType.Image:
                    return await this.Media.ImageUpload(this.TokenSuffix, fileNames);
                //GIFファイル
                case MediaType.Gif:
                    return await this.Media.AnimationGifUpload(this.TokenSuffix, fileNames);
                //動画ファイル
                case MediaType.Video:
                    return await this.Media.VideoUpload(this.TokenSuffix, fileNames);
                default:
                    return false;
            }
        }

        /// <summary>
        /// ツイートのオンオフの切り替え
        /// </summary>
        public void ToggleIsCreate()
        {
            this.IsCreate = !this._IsCreate;
        }

        #region IsCreate 変更通知プロパティ
        public bool IsCreate
        {
            get
            {
                return this._IsCreate;
            }
            set
            {
                this._IsCreate = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsCreate;
        #endregion

        public UserOverviewProperties User { get; }
        public StatusMedia Media { get; }

        public int TokenSuffix { get; set; }
    }

    public enum MediaType
    {
        Undefined,
        Image,
        Gif,
        Video
    }
}
