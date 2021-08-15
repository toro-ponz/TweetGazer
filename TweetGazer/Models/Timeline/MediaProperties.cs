using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CoreTweet;
using TweetGazer.Behaviors;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class MediaProperties
    {
        public MediaProperties(MediaEntity media, int suffix)
        {
            this.MediaNumber = suffix;

            this.PlayCommand = new RelayCommand<object>(this.PlayCommandEntity);
            this.LoopPlayCommand = new RelayCommand<object>(this.LoopPlayCommandEntity);
            if (media == null)
            {
                return;
            }

            this.Image = new ImageProperties(media.MediaUrlHttps, true, media.Sizes.Large.Width, media.Sizes.Large.Height);

            // 画像のとき
            if (media.Type == "photo")
            {
                this.Url = new Uri(media.MediaUrlHttps);
                this.Type = StatusMediaType.Image;
            }

            if (media.VideoInfo == null || media.VideoInfo.Variants == null || media.VideoInfo.Variants.Count() == 0)
            {
                return;
            }

            var url = media.VideoInfo.Variants.Where(x => x.Bitrate == media.VideoInfo.Variants.Max(y => y.Bitrate)).First().Url;
            this.Url = new Uri(url);

            // アニメーションGIFのとき
            if (media.Type == "animated_gif")
            {
                this.Image.IsLoad = false;
                this.Type = StatusMediaType.AnimationGif;
            }
            // 動画の時
            else if (media.Type == "video")
            {
                this.Type = StatusMediaType.Video;
                if (media.VideoInfo.DurationMillis == null)
                {
                    return;
                }

                this.Time = new TimeSpan((int)media.VideoInfo.DurationMillis * TimeSpan.TicksPerMillisecond);
            }
        }

        private void PlayCommandEntity(object sender)
        {
            if (sender is MediaElement mediaElement)
            {
                mediaElement.Position = TimeSpan.FromTicks(1);
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

        private void LoopPlayCommandEntity(object sender)
        {
            this.PlayCommandEntity(sender);
        }

        public StatusMediaType Type { get; }
        public ICommand PlayCommand { get; }
        public ICommand LoopPlayCommand { get; }

        public ImageProperties Image { get; }
        public Uri Url { get; }
        public TimeSpan Time { get; }

        public int MediaNumber { get; }
    }

    public enum StatusMediaType
    {
        Image,
        AnimationGif,
        Video
    }
}
