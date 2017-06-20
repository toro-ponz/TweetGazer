using CoreTweet;
using SourceChord.Lighty;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TweetGazer.Behaviors;
using TweetGazer.Common;
using TweetGazer.Views;

namespace TweetGazer.Models.Timeline
{
    public class QuotationStatusProperties
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="status">引用されたツイート内容</param>
        public QuotationStatusProperties(Status status)
        {
            this.Media = new List<MediaProperties>();
            this.MediaColumnWidth = new List<GridLength>()
            {
                new GridLength(0),
                new GridLength(0),
                new GridLength(0),
                new GridLength(0)
            };
            this.SelectMediaCommand = new RelayCommand<int>(this.SelectMedia);

            if (status == null)
                return;

            if (status.Text != null)
                this.Text = status.Text;
            else if (status.FullText != null)
                this.Text = status.FullText;
            else
                this.Text = "";

            this.User = new UserOverviewProperties(status.User);

            if (status.Entities.Media != null)
            {
                var i = 0;
                foreach (var media in status.ExtendedEntities.Media)
                {
                    this.Media.Add(new MediaProperties(media, i));
                    this.MediaColumnWidth[i] = new GridLength(1, GridUnitType.Star);
                    i++;
                }
                this.Text = this.Text.Replace(status.Entities.Media.First().Url, "");
            }
        }

        /// <summary>
        /// メディアの表示
        /// </summary>
        /// <param name="suffix">メディア番号</param>
        private void SelectMedia(int suffix)
        {
            if (this.Media.Count <= suffix)
                return;

            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                //動画を表示
                if (this.Media[suffix].Type == StatusMediaType.Video)
                {
                    using (var showMovie = new Views.ShowDialogs.ShowMovie(this.Media[suffix].Url, false))
                        LightBox.ShowDialog(mainWindow, showMovie);
                }
                //画像を表示
                else
                {
                    using (var showImage = new Views.ShowDialogs.ShowImage(this.Media.Select(x => x.Image).ToList(), suffix))
                        LightBox.ShowDialog(mainWindow, showImage);
                }
            }
        }

        public ICommand SelectMediaCommand { get; }

        public IList<GridLength> MediaColumnWidth { get; }
        public IList<MediaProperties> Media { get; }

        public UserOverviewProperties User { get; }

        public string Text { get; }
    }
}