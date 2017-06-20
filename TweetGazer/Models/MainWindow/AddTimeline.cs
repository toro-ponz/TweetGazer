using CoreTweet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TweetGazer.Common;
using TweetGazer.Models.Timeline;
using TweetGazer.ViewModels;

namespace TweetGazer.Models.MainWindow
{
    public class AddTimeline : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mainWindowViewModel">MainWindowViewModel</param>
        public AddTimeline(MainWindowViewModel mainWindowViewModel) : base()
        {
            this.MainWindowViewModel = mainWindowViewModel;
            this.ExtraGrid = new ObservableCollection<Grid>
            {
                new Grid()
            };
            BindingOperations.EnableCollectionSynchronization(this.ExtraGrid, new object());
        }

        /// <summary>
        /// Flyoutを開くとき
        /// </summary>
        public override void Open()
        {
            this.ExtraGrid.Clear();
            this.ExtraGrid.Add(new Grid());
            this.ScreenNames.Clear();
            foreach (var user in AccountTokens.Users)
                this.ScreenNames.Add("@" + user.ScreenName);
        }
        
        /// <summary>
        /// ホームタイムラインの追加
        /// </summary>
        public void AddHomeTimeline()
        {
            var page = new TimelinePageData()
            {
                TimelineType = TimelineType.Home
            };

            this.CreateTimeline(page);
        }

        /// <summary>
        /// 自分自身のユーザータイムラインの追加
        /// </summary>
        public void AddOwnTimeline()
        {
            var page = new TimelinePageData()
            {
                TimelineType = TimelineType.User,
                TargetUserId = AccountTokens.Users[this.TokenSuffix].Id,
                TargetUserName = AccountTokens.Users[this.TokenSuffix].Name,
                TargetUserScreenName = AccountTokens.Users[this.TokenSuffix].ScreenName
            };

            this.CreateTimeline(page);
        }

        /// <summary>
        /// ユーザータイムラインの追加
        /// </summary>
        public void AddUserTimeline()
        {
            this.SetUsers();
        }

        /// <summary>
        /// リストタイムラインの追加
        /// </summary>
        public void AddListTimeline()
        {
            this.SetLists();
        }

        /// <summary>
        /// メンションタイムラインの追加
        /// </summary>
        public void AddMentionTimeline()
        {
            var page = new TimelinePageData()
            {
                TimelineType = TimelineType.Mention
            };

            this.CreateTimeline(page);
        }

        /// <summary>
        /// いいねタイムラインの追加
        /// </summary>
        public void AddFavoriteTimeline()
        {
            var page = new TimelinePageData()
            {
                TimelineType = TimelineType.Favorite
            };

            this.CreateTimeline(page);
        }

        /// <summary>
        /// 通知タイムラインの追加
        /// </summary>
        public void AddNoticeTimeline()
        {

        }

        /// <summary>
        /// トレンドタイムラインの追加
        /// </summary>
        public void AddTrendTimeline()
        {
            var page = new TimelinePageData()
            {
                TimelineType = TimelineType.Trend
            };

            this.CreateTimeline(page);
        }

        /// <summary>
        /// 検索タイムラインの追加
        /// </summary>
        public void AddSearchTimeline()
        {
            this.SetTrends();
        }

        /// <summary>
        /// ダイレクトメッセージタイムラインの追加
        /// </summary>
        public void AddDirectMessageTimeline()
        {

        }

        /// <summary>
        /// タイムラインの生成
        /// </summary>
        /// <param name="data">タイムラインデータ</param>
        private void CreateTimeline(TimelinePageData page)
        {
            var data = new TimelineData()
            {
                TokenSuffix = this.TokenSuffix,
                UserId = AccountTokens.Users[this.TokenSuffix].Id,
                ScreenName = AccountTokens.Users[this.TokenSuffix].ScreenName,
                Pages = new List<TimelinePageData>()
                {
                    page
                }
            };
            this.MainWindowViewModel.Timelines.AddTimeline(data);
            this.IsOpen = false;
        }

        /// <summary>
        /// 自身の持つリストを取得する
        /// </summary>
        private async void SetLists()
        {
            this.ExtraGrid.Clear();
            this.ExtraGrid.Add(new Grid());

            var lists = await AccountTokens.LoadListsAsync(this.TokenSuffix);
            var listBox = new ListBox();
            var itemsSource = new ObservableCollection<ListProperties>();

            foreach (var list in lists)
            {
                itemsSource.Add(new ListProperties()
                {
                    Icon = new Uri(list.User.ProfileImageUrlHttps),
                    ListName = list.Name,
                    Id = list.Id,
                    CreateUser = list.User.Name,
                    MemberCount = list.MemberCount
                });
            }

            listBox.Background = new SolidColorBrush(Colors.Transparent);
            listBox.ItemTemplate = CommonMethods.LoadEmbededResourceDictionary("pack://application:,,,/Resources/Dictionaries/AddTimelineListItems.xaml")["Lists"] as DataTemplate;
            listBox.ItemsSource = itemsSource;
            listBox.SelectionChanged += (sender, e) =>
            {
                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.List,
                    ListName = itemsSource[listBox.SelectedIndex].ListName,
                    ListNumber = itemsSource[listBox.SelectedIndex].Id
                };
                this.CreateTimeline(page);
                this.ExtraGrid.First().Children.RemoveAt(0);
            };
            this.ExtraGrid.First().Children.Add(listBox);
            this.RaisePropertyChanged(nameof(ExtraGrid));
        }

        /// <summary>
        /// 自身のフォローを取得する
        /// </summary>
        private async void SetUsers()
        {
            this.ExtraGrid.Clear();
            this.ExtraGrid.Add(new Grid());

            var users = await AccountTokens.LoadFriendsAsync(this.TokenSuffix);
            var listBox = new ListBox();
            var itemsSource = new ObservableCollection<User>();
            var textBox = new TextBox()
            {
                Height = 30.0d,
                Margin = new Thickness(5, 5, 40, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var button = new Button()
            {
                Width = 30.0d,
                Height = 30.0d,
                Margin = new Thickness(0, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = new ContentControl()
                {
                    ContentTemplate = Application.Current.FindResource("AccentColorSearchIcon") as DataTemplate
                }
            };

            textBox.KeyUp += async (sender, e) =>
            {
                if (e.Key == Key.Enter && textBox.Text != "")
                {
                    itemsSource.Clear();
                    foreach (var user in await AccountTokens.LoadSearchedUsersAsync(this.TokenSuffix, textBox.Text))
                    {
                        itemsSource.Add(user);
                    }
                }
            };
            button.Click += (sender, e) =>
            {

            };
            this.ExtraGrid.First().RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            this.ExtraGrid.First().Children.Add(textBox);
            this.ExtraGrid.First().Children.Add(button);

            var i = 1;
            foreach (var user in users)
            {
                itemsSource.Add(user);
                i++;
            }

            listBox.Background = new SolidColorBrush(Colors.Transparent);
            listBox.ItemTemplate = CommonMethods.LoadEmbededResourceDictionary("pack://application:,,,/Resources/Dictionaries/AddTimelineListItems.xaml")["Users"] as DataTemplate;
            listBox.ItemsSource = itemsSource;
            listBox.SelectionChanged += (sender, e) =>
            {
                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.User,
                    TargetUserId = itemsSource[listBox.SelectedIndex].Id,
                    TargetUserName = itemsSource[listBox.SelectedIndex].Name
                };
                this.CreateTimeline(page);
                this.ExtraGrid.First().Children.RemoveAt(0);
            };
            Grid.SetRow(listBox, 1);
            this.ExtraGrid.First().RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1.0, GridUnitType.Star)
            });
            this.ExtraGrid.First().Children.Add(listBox);
        }

        /// <summary>
        /// 現在のトレンドを取得する
        /// </summary>
        private async void SetTrends()
        {
            this.ExtraGrid.Clear();
            this.ExtraGrid.Add(new Grid());

            var trendResult = await AccountTokens.LoadTrendsAsync(TokenSuffix);
            var listBox = new ListBox();
            var itemsSource = new ObservableCollection<TrendProperties>();
            var textBox = new TextBox()
            {
                Height = 30.0d,
                Margin = new Thickness(5, 5, 40, 5),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var button = new Button()
            {
                Width = 30.0d,
                Height = 30.0d,
                Margin = new Thickness(0, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = new ContentControl()
                {
                    ContentTemplate = Application.Current.FindResource("AccentColorSearchIcon") as DataTemplate
                }
            };

            textBox.KeyUp += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    var page = new TimelinePageData()
                    {
                        TimelineType = TimelineType.Search,
                        SearchText = textBox.Text
                    };
                    this.CreateTimeline(page);
                }
            };
            button.Click += (sender, e) =>
            {
                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.Search,
                    SearchText = textBox.Text
                };
                this.CreateTimeline(page);
            };
            this.ExtraGrid.First().RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            this.ExtraGrid.First().Children.Add(textBox);
            this.ExtraGrid.First().Children.Add(button);

            var i = 1;
            foreach (var trends in trendResult)
            {
                foreach (var trend in trends)
                {
                    var item = new TrendProperties()
                    {
                        Rank = i,
                        Name = trend.Name,
                        CountVisibility = Visibility.Collapsed
                    };
                    if (trend.TweetVolume != null)
                    {
                        item.Count = (int)trend.TweetVolume;
                        item.CountVisibility = Visibility.Visible;
                    }
                    itemsSource.Add(item);
                    i++;
                }
            }

            listBox.Background = new SolidColorBrush(Colors.Transparent);
            listBox.ItemTemplate = CommonMethods.LoadEmbededResourceDictionary("pack://application:,,,/Resources/Dictionaries/AddTimelineListItems.xaml")["Trends"] as DataTemplate;
            listBox.ItemsSource = itemsSource;
            listBox.SelectionChanged += (sender, e) =>
            {
                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.Search,
                    SearchText = itemsSource[listBox.SelectedIndex].Name
                };
                this.CreateTimeline(page);
                this.ExtraGrid.First().Children.RemoveAt(0);
            };
            Grid.SetRow(listBox, 1);
            this.ExtraGrid.First().RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1.0, GridUnitType.Star)
            });
            this.ExtraGrid.First().Children.Add(listBox);
        }

        /// <summary>
        /// DM追加ボタンを押したとき
        /// </summary>
        private void SetDirectMessages()
        {

        }

        public ObservableCollection<Grid> ExtraGrid { get; }
        private MainWindowViewModel MainWindowViewModel;
        
        private class ListProperties
        {
            public Uri Icon { get; set; }
            public string ListName { get; set; }
            public long Id { get; set; }
            public string CreateUser { get; set; }
            public int MemberCount { get; set; }
        }

        private class TrendProperties
        {
            public int Rank { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
            public Visibility CountVisibility { get; set; }
        }
    }
}