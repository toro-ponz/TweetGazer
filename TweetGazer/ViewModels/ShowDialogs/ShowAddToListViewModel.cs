using Livet;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TweetGazer.Common;
using TweetGazer.Models.ShowDialogs;
using TweetGazer.Models.Timeline;

namespace TweetGazer.ViewModels.ShowDialogs
{
    public class ShowAddToListViewModel : ViewModel
    {
        public ShowAddToListViewModel(int tokenSuffix, UserProperties user)
        {
            this.ShowAddToList = new ShowAddToList(tokenSuffix, user);
            this.User = this.ShowAddToList.User;
            this.Lists = this.ShowAddToList.Lists;

            this.ApplyCommand = new RelayCommand(this.Apply);
        }

        private void Apply()
        {
            this.ShowAddToList.Apply();
        }

        public ICommand ApplyCommand { get; }

        public UserProperties User { get; }

        public ObservableCollection<ListProperties> Lists { get; }

        private ShowAddToList ShowAddToList;
    }
}