using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Behaviors
{
    public static class GifMediaElementBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static Uri GetSource(DependencyObject element)
        {
            if (element == null)
                return null;
            return element.GetValue(SourceProperty) as Uri;
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetSource(DependencyObject element, Uri value)
        {
            if (element == null)
                return;
            element.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(Uri), typeof(GifMediaElementBehavior), new PropertyMetadata(null, Source_ChangedAsync));

        private static async void Source_ChangedAsync(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as MediaElement;
            var url = e.NewValue as Uri;
            if (element == null || url == null)
                return;

            string fileName = DateTime.Now.Ticks.ToString() + new Random().Next().ToString() + ".mp4";
            var filePath = Directory.GetCurrentDirectory() + Common.SecretParameters.TemporaryDirectoryPath  + fileName;

            await Task.Run(() =>
            {
                System.Net.WebClient wc = new System.Net.WebClient();

                //ディレクトリが存在しない場合作成する
                if (!Directory.GetParent(filePath).Exists)
                {
                    Directory.CreateDirectory(Directory.GetParent(filePath).FullName);
                }

                try
                {
                    wc.DownloadFile(url, Common.SecretParameters.TemporaryDirectoryPath + fileName);
                }
                catch (Exception ex)
                {
                    DebugConsole.Write(ex);
                }
                finally
                {
                    wc.Dispose();
                }
            });

            element.LoadedBehavior = MediaState.Manual;
            element.UnloadedBehavior = MediaState.Close;
            element.MediaEnded += (s, eventArgs) =>
            {
                if (Properties.Settings.Default.IsGifLoopPlay)
                    Play(element);
            };
            element.Source = new Uri(filePath);

            if (Properties.Settings.Default.IsGifAutoPlay)
                Play(element);
        }

        public static ICommand PlayCommand { get; set; } = new Common.RelayCommand<MediaElement>((mediaElement) =>
        {
            try
            {
                mediaElement.Play();
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        });

        private static void Play(MediaElement mediaElement)
        {
            try
            {
                mediaElement.Position = TimeSpan.FromTicks(1);
                mediaElement.Play();
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }
    }
}