using Livet;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TweetGazer.Common;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Models.ShowDialogs
{
    public class ShowListModel : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokenSuffix">アカウント番号</param>
        /// <param name="user">表示対象のユーザー</param>
        public ShowListModel(int tokenSuffix, UserProperties user)
        {
            this.TokenSuffix = tokenSuffix;
            this.User = user;
            this._IsUserList = true;

            this.UserLists = new ObservableCollection<ListProperties>();
            this.AddedLists = new ObservableCollection<ListProperties>();
            BindingOperations.EnableCollectionSynchronization(this.UserLists, new object());
            BindingOperations.EnableCollectionSynchronization(this.AddedLists, new object());

            this.LoadLists();
        }

        /// <summary>
        /// 表示するリストを取得する
        /// </summary>
        public async void LoadLists()
        {
            var userLists = await AccountTokens.LoadListsAsync(this.TokenSuffix, this.User.Id);
            if (userLists != null)
            {
                foreach (var userList in userLists)
                {
                    this.UserLists.Add(new ListProperties(userList));
                }
            }

            var addedLists = await AccountTokens.LoadListMembershipAsync(this.TokenSuffix, this.User.Id);
            if (addedLists != null)
            {
                foreach (var addedList in addedLists)
                {
                    this.AddedLists.Add(new ListProperties(addedList));
                }
            }
        }

        /// <summary>
        /// 表示するリストをユーザーのリストに切り替える
        /// </summary>
        public void SwitchUserLists()
        {
            if (!this.IsUserList)
            {
                this.IsUserList = true;
            }
        }

        /// <summary>
        /// 表示するリストをユーザーが追加されたリストに切り替える
        /// </summary>
        public void SwitchAddedLists()
        {
            if (this.IsUserList)
            {
                this.IsUserList = false;
            }
        }

        #region IsUserList 変更通知プロパティ
        public bool IsUserList
        {
            get
            {
                return this._IsUserList;
            }
            set
            {
                this._IsUserList = value;
                this.RaisePropertyChanged();
            }
        }
        private bool _IsUserList;
        #endregion

        public ObservableCollection<ListProperties> UserLists { get; }
        public ObservableCollection<ListProperties> AddedLists { get; }

        public UserProperties User { get; }

        private int TokenSuffix;
    }
}
