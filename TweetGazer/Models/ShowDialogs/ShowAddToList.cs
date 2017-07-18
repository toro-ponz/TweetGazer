using System.Collections.ObjectModel;
using System.Windows.Data;
using TweetGazer.Common;
using TweetGazer.Models.Timeline;
using TweetGazer.ViewModels;

namespace TweetGazer.Models.ShowDialogs
{
    public class ShowAddToList
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokenSuffix">アカウント番号</param>
        /// <param name="user">操作対象のユーザー</param>
        public ShowAddToList(int tokenSuffix, UserProperties user)
        {
            this.TokenSuffix = tokenSuffix;
            this.User = user;
            this.Lists = new ObservableCollection<ListProperties>();
            BindingOperations.EnableCollectionSynchronization(this.Lists, new object());

            this.LoadLists();
        }

        /// <summary>
        /// 変更を適用する
        /// </summary>
        public async void Apply()
        {
            foreach (var list in this.Lists)
            {
                // 変更されていた場合
                if (list.IsChanged)
                {
                    CoreTweet.ListResponse listResponse = null;

                    // リストへの追加
                    if (list.IsAdded)
                        listResponse = await AccountTokens.CreateListMemberAsync(this.TokenSuffix, list.Id, this.User.Id);
                    // リストからの削除
                    else
                        listResponse = await AccountTokens.DestroyListMemberAsync(this.TokenSuffix, list.Id, this.User.Id);

                    var mainWindow = CommonMethods.MainWindow;
                    if (mainWindow != null)
                    {
                        if (listResponse == null)
                        {
                            list.IsAdded = list._IsAdded;
                            (mainWindow.DataContext as MainWindowViewModel).Notify("リストの編集に失敗しました．", MainWindow.NotificationType.Error);
                        }
                        else
                        {
                            list._IsAdded = list.IsAdded;
                            (mainWindow.DataContext as MainWindowViewModel).Notify("リストの編集に成功しました．", MainWindow.NotificationType.Success);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 自身が作成したリストを取得する
        /// </summary>
        private async void LoadLists()
        {
            // 操作対象のユーザーの関わるリストの内、自分が作成したものを取得
            var userAddedLists = await AccountTokens.LoadListMembershipAsync(this.TokenSuffix, this.User.Id, true);
            // 自分のリストを取得
            var lists = await AccountTokens.LoadListsAsync(this.TokenSuffix);

            if (userAddedLists == null || lists == null)
                return;

            foreach (var list in lists)
            {
                // 自分の作成したリスト以外を弾く
                if (list.User.Id == AccountTokens.Users[this.TokenSuffix].Id)
                {
                    var isAdded = false;
                    foreach (var l in userAddedLists.Result)
                    {
                        // 既に追加されているか判定
                        if (list.Id == l.Id)
                        {
                            isAdded = true;
                            break;
                        }
                    }
                    this.Lists.Add(new ListProperties(list, isAdded));
                }
            }
        }
        
        public UserProperties User { get; }

        public ObservableCollection<ListProperties> Lists { get; }

        private int TokenSuffix;
    }
}