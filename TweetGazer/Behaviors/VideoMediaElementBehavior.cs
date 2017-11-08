using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TweetGazer.Common;

namespace TweetGazer.Behaviors
{
    public static class VideoMediaElementBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static Uri GetSource(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            return element.GetValue(SourceProperty) as Uri;
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetSource(DependencyObject element, Uri value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(Uri), typeof(VideoMediaElementBehavior), new PropertyMetadata(Source_Changed));

        private static async void Source_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as MediaElement;
            var url = e.NewValue as Uri;
            if (element == null)
            {
                return;
            }

            if (url == null)
            {
                SetState(element, States.Finished);
                element.Stop();
                element.Loaded -= Loaded;
                element.Unloaded -= Unloaded;
                element.MediaEnded -= MediaEnded;
                Dispose();
                return;
            }

            SetState(element, States.Initializing);

            element.LoadedBehavior = MediaState.Manual;
            element.UnloadedBehavior = MediaState.Manual;
            element.Loaded += Loaded;
            element.Unloaded += Unloaded;
            element.MediaEnded += MediaEnded;

            string fileName = DateTime.Now.Ticks.ToString() + new Random().Next().ToString() + ".mp4";
            var filePath = Directory.GetCurrentDirectory() + Common.SecretParameters.TemporaryDirectoryPath + fileName;

            await Task.Run(() =>
            {
                var wc = new System.Net.WebClient();

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

            if (GetState(element) == States.Finished)
            {
                return;
            }

            element.Source = new Uri(filePath);

            if (!GetIsAutoPlay(element))
            {
                SetState(element, States.Pauseing);
                return;
            }

            while (!element.NaturalDuration.HasTimeSpan)
            {
                await Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(50);
                });
                Play(element);
                Stop(element);
            }
            Play(element);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static long GetPosition(DependencyObject element)
        {
            if (element == null)
            {
                return 0;
            }

            return (long)element.GetValue(PositionProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetPosition(DependencyObject element, long value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(long), typeof(VideoMediaElementBehavior), new PropertyMetadata(Position_Changed));

        private static void Position_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as MediaElement;
            if (element == null)
            {
                return;
            }

            element.Position = TimeSpan.FromTicks((long)e.NewValue);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static double GetVolume(DependencyObject element)
        {
            if (element == null)
            {
                return 0;
            }

            return (double)element.GetValue(VolumeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetVolume(DependencyObject element, double value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(VolumeProperty, value);
        }

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.RegisterAttached("Volume", typeof(double), typeof(VideoMediaElementBehavior), new PropertyMetadata(1.0d, Volume_Changed));

        private static void Volume_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as MediaElement;
            if (element == null)
            {
                return;
            }

            element.Volume = (double)e.NewValue;
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static double GetVolumeTemporary(DependencyObject element)
        {
            if (element == null)
            {
                return 0;
            }

            return (double)element.GetValue(VolumeTemporaryProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetVolumeTemporary(DependencyObject element, double value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(VolumeTemporaryProperty, value);
        }

        public static readonly DependencyProperty VolumeTemporaryProperty =
            DependencyProperty.RegisterAttached("VolumeTemporary", typeof(double), typeof(VideoMediaElementBehavior));

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static States GetState(DependencyObject element)
        {
            if (element == null)
            {
                return States.Initializing;
            }

            return (States)element.GetValue(StateProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetState(DependencyObject element, States value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(StateProperty, value);
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State", typeof(States), typeof(VideoMediaElementBehavior), new PropertyMetadata(States.Initializing));

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static bool GetIsAutoPlay(DependencyObject element)
        {
            if (element == null)
            {
                return false;
            }

            return (bool)element.GetValue(IsAutoPlayProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetIsAutoPlay(DependencyObject element, bool value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(IsAutoPlayProperty, value);
        }

        public static readonly DependencyProperty IsAutoPlayProperty =
            DependencyProperty.RegisterAttached("IsAutoPlay", typeof(bool), typeof(VideoMediaElementBehavior), new PropertyMetadata(false));

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static bool GetIsMuted(DependencyObject element)
        {
            if (element == null)
            {
                return false;
            }

            return (bool)element.GetValue(IsMutedProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetIsMuted(DependencyObject element, bool value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(IsMutedProperty, value);
        }

        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.RegisterAttached("IsMuted", typeof(bool), typeof(VideoMediaElementBehavior), new PropertyMetadata(false, IsMuted_Changed));

        private static void IsMuted_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as MediaElement;
            if (element == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                SetVolumeTemporary(element, GetVolume(element));
                SetVolume(element, 0.0d);
            }
            else
            {
                SetVolume(element, GetVolumeTemporary(element));
                SetVolumeTemporary(element, GetVolume(element));
            }
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static bool GetIsLoop(DependencyObject element)
        {
            if (element == null)
            {
                return false;
            }

            return (bool)element.GetValue(IsLoopProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(MediaElement))]
        public static void SetIsLoop(DependencyObject element, bool value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(IsLoopProperty, value);
        }

        public static readonly DependencyProperty IsLoopProperty =
            DependencyProperty.RegisterAttached("IsLoop", typeof(bool), typeof(VideoMediaElementBehavior), new PropertyMetadata(Properties.Settings.Default.IsVideoLoopPlay));

        [AttachedPropertyBrowsableForType(typeof(Slider))]
        public static MediaElement GetMediaElement(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            return element.GetValue(MediaElementProperty) as MediaElement;
        }

        [AttachedPropertyBrowsableForType(typeof(Slider))]
        public static void SetMediaElement(DependencyObject element, MediaElement value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(MediaElementProperty, value);
        }

        public static readonly DependencyProperty MediaElementProperty =
            DependencyProperty.RegisterAttached("MediaElement", typeof(MediaElement), typeof(VideoMediaElementBehavior), new PropertyMetadata(MediaElement_Changed));

        private static void MediaElement_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as Slider;
            var mediaElement = e.NewValue as MediaElement;
            if (element == null || mediaElement == null)
            {
                return;
            }

            var value = 0.0d;

            element.DragEnter += (s, eventArgs) =>
            {
                Pause(mediaElement);
            };
            element.DragLeave += (s, eventArgs) =>
            {
                if (GetState(mediaElement) == States.Playing)
                {
                    Play(mediaElement);
                }
            };
            element.ValueChanged += (s, eventArgs) =>
            {
                if (!(GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing))
                {
                    return;
                }

                if (value != eventArgs.NewValue)
                {
                    SetPosition(mediaElement, (long)eventArgs.NewValue * mediaElement.NaturalDuration.TimeSpan.Ticks / (long)element.Maximum - 10000);
                    Play(mediaElement);
                    System.Threading.Thread.Sleep(1);
                    if (GetState(mediaElement) != States.Playing)
                    {
                        Pause(mediaElement);
                    }
                }
            };
            var timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            timer.Tick += (s, eventArgs) =>
            {
                if (mediaElement.NaturalDuration.HasTimeSpan)
                {
                    value = (long)element.Maximum * mediaElement.Position.Ticks / mediaElement.NaturalDuration.TimeSpan.Ticks;
                    element.Value = value;
                }
            };
            Timers.Add(timer);
            timer.Start();

        }

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static MediaElement GetTime(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            return element.GetValue(TimeProperty) as MediaElement;
        }

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static void SetTime(DependencyObject element, MediaElement value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(TimeProperty, value);
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.RegisterAttached("Time", typeof(MediaElement), typeof(VideoMediaElementBehavior), new PropertyMetadata(Time_Changed));

        [Localizable(false)]
        private static void Time_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as TextBlock;
            var mediaElement = e.NewValue as MediaElement;
            if (element == null || mediaElement == null)
            {
                return;
            }

            var timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += (s, eventArgs) =>
            {
                if (mediaElement.NaturalDuration.HasTimeSpan)
                {
                    element.Text = mediaElement.Position.ToString(@"m\:ss", CultureInfo.InvariantCulture) + " / " + mediaElement.NaturalDuration.TimeSpan.ToString(@"m\:ss", CultureInfo.InvariantCulture);
                }
            };
            Timers.Add(timer);
            timer.Start();
        }

        private static void PlayAndPause(MediaElement mediaElement)
        {
            switch (GetState(mediaElement))
            {
                case States.Playing:
                    Pause(mediaElement);
                    break;
                case States.Pauseing:
                    Play(mediaElement);
                    break;
            }
        }

        private static void Play(MediaElement mediaElement)
        {
            try
            {
                switch (GetState(mediaElement))
                {
                    case States.Pauseing:
                        mediaElement.Play();
                        SetState(mediaElement, States.Playing);
                        break;
                }
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        private static void Pause(MediaElement mediaElement)
        {
            try
            {
                switch (GetState(mediaElement))
                {
                    case States.Playing:
                        mediaElement.Pause();
                        SetState(mediaElement, States.Pauseing);
                        break;
                }
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        private static void Stop(MediaElement mediaElement)
        {
            try
            {
                mediaElement.Stop();
                SetState(mediaElement, States.Pauseing);
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        private static void BackToTop(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                mediaElement.Position = TimeSpan.FromTicks(1);
            }
        }

        private static void BackFiveSeconds(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                MovePosition(mediaElement, new TimeSpan(0, 0, -5));
            }
        }

        private static void BackTenSeconds(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                MovePosition(mediaElement, new TimeSpan(0, 0, -10));
            }
        }

        private static void BackThirtySeconds(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                MovePosition(mediaElement, new TimeSpan(0, 0, -30));
            }
        }

        private static void ForwordFiveSeconds(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                MovePosition(mediaElement, new TimeSpan(0, 0, 5));
            }
        }

        private static void ForwordTenSeconds(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                MovePosition(mediaElement, new TimeSpan(0, 0, 10));
            }
        }

        private static void ForwordThirtySeconds(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                MovePosition(mediaElement, new TimeSpan(0, 0, 30));
            }
        }

        private static void ForwordToEnd(MediaElement mediaElement)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                mediaElement.Position = mediaElement.NaturalDuration.TimeSpan;
            }
        }

        private static void MovePosition(MediaElement mediaElement, TimeSpan amount)
        {
            if (GetState(mediaElement) == States.Playing || GetState(mediaElement) == States.Pauseing)
            {
                mediaElement.Position += amount;
            }
        }

        private static void ToggleRepeat(MediaElement mediaElement)
        {
            SetIsLoop(mediaElement, !GetIsLoop(mediaElement));
        }

        private static void Mute(MediaElement mediaElement)
        {
            SetIsMuted(mediaElement, !GetIsMuted(mediaElement));
        }

        private static void Loaded(object sender, RoutedEventArgs e)
        {
            var element = sender as MediaElement;
            if (element == null)
            {
                return;
            }

            if (GetIsAutoPlay(element))
            {
                element.Play();
                element.Pause();
                element.Position = TimeSpan.FromTicks(1);
                element.Play();
            }
        }

        private static void Unloaded(object sender, RoutedEventArgs e)
        {
            var element = sender as MediaElement;
            if (element == null)
            {
                return;
            }

            Stop(element);
        }

        private static void MediaEnded(object sender, RoutedEventArgs e)
        {
            var element = sender as MediaElement;
            if (element == null)
            {
                return;
            }

            if (GetIsLoop(element))
            {
                element.Position = TimeSpan.FromTicks(1);
                Play(element);
            }
            else
            {
                SetState(element, States.Pauseing);
                Stop(element);
            }
        }

        private static void Dispose()
        {
            foreach (var timer in Timers)
            {
                timer.Stop();
            }
            Timers.Clear();
        }

        public static ICommand PlayAndPauseCommand { get; } = new RelayCommand<MediaElement>(PlayAndPause);
        public static ICommand PlayCommand { get; } = new RelayCommand<MediaElement>(Play);
        public static ICommand PauseCommand { get; } = new RelayCommand<MediaElement>(Pause);
        public static ICommand StopCommand { get; } = new RelayCommand<MediaElement>(Stop);
        public static ICommand BackToTopCommand { get; } = new RelayCommand<MediaElement>(BackToTop);
        public static ICommand BackFiveSecondsCommand { get; } = new RelayCommand<MediaElement>(BackFiveSeconds);
        public static ICommand BackTenSecondsCommand { get; } = new RelayCommand<MediaElement>(BackTenSeconds);
        public static ICommand BackThirtySecondsCommand { get; } = new RelayCommand<MediaElement>(BackThirtySeconds);
        public static ICommand ForwordFiveSecondsCommand { get; } = new RelayCommand<MediaElement>(ForwordFiveSeconds);
        public static ICommand ForwordTenSecondsCommand { get; } = new RelayCommand<MediaElement>(ForwordTenSeconds);
        public static ICommand ForwordThirtySecondsCommand { get; } = new RelayCommand<MediaElement>(ForwordThirtySeconds);
        public static ICommand ForwordToEndCommand { get; } = new RelayCommand<MediaElement>(ForwordToEnd);
        public static ICommand ToggleRepeatCommand { get; } = new RelayCommand<MediaElement>(ToggleRepeat);
        public static ICommand MuteCommand { get; } = new RelayCommand<MediaElement>(Mute);

        private static List<DispatcherTimer> Timers = new List<DispatcherTimer>();
    }

    public enum States
    {
        Initializing,
        Playing,
        Pauseing,
        Finished
    }
}
