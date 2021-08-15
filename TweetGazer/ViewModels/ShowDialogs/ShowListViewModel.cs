using Livet;
using Livet.EventListeners;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.ShowDialogs;
using TweetGazer.Models.Timeline;

namespace TweetGazer.ViewModels.ShowDialogs
{
    public class ShowListViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tokenSuffix">アカウント番号</param>
        /// <param name="user">表示対象のユーザー</param>
        public ShowListViewModel(int tokenSuffix, UserProperties user)
        {
            this.ShowListModel = new ShowListModel(tokenSuffix, user);

            this.User = this.ShowListModel.User;
            this.IsUserList = this.ShowListModel.IsUserList;

            this.CompositeDisposable.Add(
                new PropertyChangedEventListener(this.ShowListModel, (_, __) =>
                {
                    switch (__.PropertyName)
                    {
                        case nameof(this.ShowListModel.IsUserList):
                            this.IsUserList = this.ShowListModel.IsUserList;
                            this.RaisePropertyChanged(() => this.Lists);
                            break;
                    }
                })
            );

            this.SwitchUserListCommand = new RelayCommand(this.SwitchUserLists);
            this.SwitchAddedListCommand = new RelayCommand(this.SwitchAddedLists);
        }

        /// <summary>
        /// 表示するリストをユーザーのリストに切り替える
        /// </summary>
        private void SwitchUserLists()
        {
            this.ShowListModel.SwitchUserLists();
        }

        /// <summary>
        /// 表示するリストをユーザーが追加されたリストに切り替える
        /// </summary>
        private void SwitchAddedLists()
        {
            this.ShowListModel.SwitchAddedLists();
        }

        #region Lists 変更通知プロパティ
        public ObservableCollection<ListProperties> Lists
        {
            get
            {
                if (this.ShowListModel.IsUserList)
                {
                    return this.ShowListModel.UserLists;
                }

                return this.ShowListModel.AddedLists;
            }
        }
        #endregion

        #region IsOwner 変更通知プロパティ
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

        public ICommand SwitchUserListCommand { get; }
        public ICommand SwitchAddedListCommand { get; }

        public UserProperties User { get; }

        private ShowListModel ShowListModel;
    }
}
