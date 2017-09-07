using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
            var itemsSource = new List<ListProperties>();

            foreach (var list in lists)
            {
                itemsSource.Add(
                    new ListProperties(
                        this,
                        new Uri(list.User.ProfileImageUrlHttps),
                        list.Name,
                        list.User.Name,
                        list.Id,
                        list.MemberCount
                    )
                );
            }

            var contentControl = new ContentControl()
            {
                Content = Application.Current.FindResource("AddTimelineExtraGridLists") as Grid,
                DataContext = itemsSource
            };

            this.ExtraGrid.First().Children.Add(contentControl);
        }

        /// <summary>
        /// 自身のフォローを取得する
        /// </summary>
        private async void SetUsers()
        {
            this.ExtraGrid.Clear();
            this.ExtraGrid.Add(new Grid());

            var users = await AccountTokens.LoadFriendsAsync(this.TokenSuffix);
            var itemsSource = new ObservableCollection<UserProperties>();
            BindingOperations.EnableCollectionSynchronization(itemsSource, new object());

            foreach (var user in users)
            {
                if (user.Id != null)
                {
                    itemsSource.Add(new UserProperties(this)
                    {
                        Name = user.Name,
                        ScreenName = user.ScreenName,
                        Description = user.Description,
                        ProfileImageUrlHttps = user.ProfileImageUrlHttps,
                        Id = (long)user.Id
                    });
                }
            }

            var contentControl = new ContentControl()
            {
                Content = Application.Current.FindResource("AddTimelineExtraGridUsers") as Grid,
                DataContext = new UsersModel(this, itemsSource, this.TokenSuffix)
            };
            this.ExtraGrid.First().Children.Add(contentControl);
        }

        /// <summary>
        /// 現在のトレンドを取得する
        /// </summary>
        private async void SetTrends()
        {
            this.ExtraGrid.Clear();
            this.ExtraGrid.Add(new Grid());

            var trendResult = await AccountTokens.LoadTrendsAsync(TokenSuffix);
            var itemsSource = new List<TrendProperties>();

            var i = 1;
            foreach (var trends in trendResult)
            {
                foreach (var trend in trends)
                {
                    var item = new TrendProperties(this)
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

            var contentControl = new ContentControl()
            {
                Content = Application.Current.FindResource("AddTimelineExtraGridTrends") as Grid,
                DataContext = new TrendsModel(this, itemsSource)
            };
            this.ExtraGrid.First().Children.Add(contentControl);
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
            public ListProperties(AddTimeline addTimeline, Uri url, string listName, string userName, long id, int count)
            {
                this.AddTimeline = addTimeline;

                this.Icon = url;
                this.ListName = listName;
                this.UserName = userName;
                this.Id = id;
                this.MemberCount = count;

                this.SelectCommand = new RelayCommand(this.Select);
            }

            private void Select()
            {
                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.List,
                    ListName = this.ListName,
                    ListNumber = this.Id
                };
                this.AddTimeline.CreateTimeline(page);
                this.AddTimeline.ExtraGrid.First().Children.Clear();
            }

            public ICommand SelectCommand { get; }

            public Uri Icon { get; }

            public string ListName { get; }
            public string UserName { get; }

            public long Id { get; }

            public int MemberCount { get; }

            private AddTimeline AddTimeline { get; }
        }

        private class UserProperties
        {
            public UserProperties(AddTimeline addTimeline)
            {
                this.AddTimeline = addTimeline;

                this.SelectCommand = new RelayCommand(this.Select);
            }

            private void Select()
            {
                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.User,
                    TargetUserId = Id,
                    TargetUserName = Name
                };
                this.AddTimeline.CreateTimeline(page);
                this.AddTimeline.ExtraGrid.First().Children.Clear();
            }

            public ICommand SelectCommand { get; }

            public string Name { get; set; }
            public string ScreenName { get; set; }
            public string Description { get; set; }
            public string ProfileImageUrlHttps { get; set; }
            public long Id { get; set; }

            private AddTimeline AddTimeline;
        }

        private class TrendProperties
        {
            public TrendProperties(AddTimeline addTimeline)
            {
                this.AddTimeline = addTimeline;

                this.SelectCommand = new RelayCommand(this.Select);
            }

            private void Select()
            {
                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.Search,
                    SearchText = this.Name
                };
                this.AddTimeline.CreateTimeline(page);
                this.AddTimeline.ExtraGrid.First().Children.Clear();
            }

            public ICommand SelectCommand { get; }
            public Visibility CountVisibility { get; set; }

            public string Name { get; set; }
            public int Rank { get; set; }
            public int Count { get; set; }

            private AddTimeline AddTimeline;
        }

        private class UsersModel
        {
            public UsersModel(AddTimeline addTimeline, ObservableCollection<UserProperties> users, int tokenSuffix)
            {
                this.AddTimeline = addTimeline;

                this.Users = users;
                this.TokenSuffix = tokenSuffix;

                this.SearchCommand = new RelayCommand(this.Search);
            }

            private async void Search()
            {
                if (String.IsNullOrEmpty(this.Text))
                    return;

                this.Users.Clear();
                foreach (var user in await AccountTokens.LoadSearchedUsersAsync(this.TokenSuffix, this.Text))
                {
                    if (user.Id != null)
                    {
                        this.Users.Add(new UserProperties(this.AddTimeline)
                        {
                            Name = user.Name,
                            ScreenName = user.ScreenName,
                            Description = user.Description,
                            ProfileImageUrlHttps = user.ProfileImageUrlHttps,
                            Id = (long)user.Id
                        });
                    }
                }
            }

            public ICommand SearchCommand { get; }

            public ObservableCollection<UserProperties> Users { get; }

            public string Text { get; set; }

            private int TokenSuffix;

            private AddTimeline AddTimeline;
        }

        private class TrendsModel
        {
            public TrendsModel(AddTimeline addTimeline, List<TrendProperties> trends)
            {
                this.AddTimeline = addTimeline;

                this.Trends = trends;

                this.SearchCommand = new RelayCommand(this.Search);
            }

            private void Search()
            {
                if (String.IsNullOrEmpty(this.Text))
                    return;

                var page = new TimelinePageData()
                {
                    TimelineType = TimelineType.Search,
                    SearchText = this.Text
                };
                this.AddTimeline.CreateTimeline(page);
                this.AddTimeline.ExtraGrid.First().Children.Clear();
            }

            public ICommand SearchCommand { get; }

            public List<TrendProperties> Trends { get; }

            public string Text { get; set; }

            private AddTimeline AddTimeline;
        }
    }
}