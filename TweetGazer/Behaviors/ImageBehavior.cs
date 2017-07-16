using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TweetGazer.Common;

namespace TweetGazer.Behaviors
{
    public static class ImageBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static ImageProperties GetSource(DependencyObject element)
        {
            if (element == null)
                return null;
            return element.GetValue(SourceProperty) as ImageProperties;
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static void SetSource(DependencyObject element, ImageProperties value)
        {
            if (element == null)
                return;
            element.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(ImageProperties), typeof(ImageBehavior), new PropertyMetadata(null, Source_Changed));

        private static async void Source_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as Image;
            var imageProperties = e.NewValue as ImageProperties;
            if (element == null || imageProperties == null)
                return;

            //画像サイズを元にロード中の仮画像を生成
            if (imageProperties.Height != 0 && imageProperties.Width != 0)
            {
                element.Source = CommonMethods.CreateTemporaryImageAsync(imageProperties.Width, imageProperties.Height);
            }

            //画像のロード
            if (imageProperties.IsLoad)
            {
                var image = await CommonMethods.DownloadImageAsync(imageProperties.Source);
                if (image != null)
                {
                    element.Source = image;
                }
            }
        }

        [AttachedPropertyBrowsableForType(typeof(ImageBrush))]
        public static ImageProperties GetImageSource(DependencyObject element)
        {
            if (element == null)
                return null;
            return element.GetValue(ImageSourceProperty) as ImageProperties;
        }

        [AttachedPropertyBrowsableForType(typeof(ImageBrush))]
        public static void SetImageSource(DependencyObject element, ImageProperties value)
        {
            if (element == null)
                return;
            element.SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.RegisterAttached("ImageSource", typeof(ImageProperties), typeof(ImageBehavior), new PropertyMetadata(null, ImageSource_Changed));

        private static async void ImageSource_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as ImageBrush;
            var imageProperties = e.NewValue as ImageProperties;
            if (element == null || imageProperties == null)
                return;

            //画像サイズを元にロード中の仮画像を生成
            if (imageProperties.Height != 0 && imageProperties.Width != 0)
            {
                element.ImageSource = CommonMethods.CreateTemporaryImageAsync(imageProperties.Width, imageProperties.Height);
            }

            //画像のロード
            if (imageProperties.IsLoad)
            {
                var image = await CommonMethods.DownloadImageAsync(imageProperties.Source);
                if (image != null)
                {
                    element.ImageSource = image;
                }
            }
        }
    }

    public class ImageProperties
    {
        public ImageProperties(Uri url, bool isLoad, int width = -1, int height = -1)
        {
            Initialize(url, isLoad, width, height);
        }

        public ImageProperties(string url, bool isLoad, int width = -1, int height = -1)
        {
            try
            {
                Initialize(new Uri(url), isLoad, width, height);
            }
            catch
            {
                this.Source = null;
                this.Width = 0;
                this.Height = 0;
            }
        }

        private void Initialize(Uri url, bool isLoad, int width, int height)
        {
            this.Source = url;
            this.IsLoad = isLoad;
            this.Width = width;
            this.Height = height;
        }

        private async void LoadSizeAsync()
        {
            try
            {
                using (var web = new HttpClient())
                {
                    var bytes = await web.GetByteArrayAsync(this.Source).ConfigureAwait(false);
                    using (var stream = new MemoryStream(bytes))
                    {
                        var bitmap = new System.Drawing.Bitmap(stream);
                        this.Width = bitmap.Width;
                        this.Height = bitmap.Height;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }
        }

        public Uri Source { get; set; }
        public bool IsLoad { get; set; }
        public int Width
        {
            get
            {
                if (_Width == -1)
                {
                    LoadSizeAsync();
                    return _Width;
                }
                else
                    return _Width;
            }
            set
            {
                _Width = value;
            }
        }
        public int Height
        {
            get
            {
                if (_Height == -1)
                {
                    LoadSizeAsync();
                    return _Height;
                }
                else
                    return _Height;
            }
            set
            {
                _Height = value;
            }
        }

        private int _Width;
        private int _Height;
    }
}