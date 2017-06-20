using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace TweetGazer.Selectors
{
    public class CreateStatusMediaTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;

            var creatingStatus = container as FrameworkElement;
            var data = item as Uri;
            if (creatingStatus == null || data == null)
                return null;

            var imageRegex = new Regex(@"^.+\.(jpe?g|png)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var imageRegexMatch = imageRegex.Match(data.ToString());
            var gifRegex = new Regex(@"^.+\.gif$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var gifRegexMatch = gifRegex.Match(data.ToString());
            var videoRegex = new Regex(@"^.+\.mp4$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var videoRegexMatch = videoRegex.Match(data.ToString());

            //画像ファイルの時
            if (imageRegexMatch.Success)
                result = creatingStatus.FindResource("ImageTemplate") as DataTemplate;
            //GIFファイルの時
            else if (gifRegexMatch.Success)
            {
                using (var bitmap = new System.Drawing.Bitmap(data.OriginalString))
                {
                    //アニメーションGIFの場合
                    if (System.Drawing.ImageAnimator.CanAnimate(bitmap))
                        result = creatingStatus.FindResource("GifTemplate") as DataTemplate;
                    else
                        result = creatingStatus.FindResource("ImageTemplate") as DataTemplate;
                }
            }
            //動画ファイルの場合
            else if (videoRegexMatch.Success)
                result = creatingStatus.FindResource("VideoTemplate") as DataTemplate;

            return result;
        }
    }
}