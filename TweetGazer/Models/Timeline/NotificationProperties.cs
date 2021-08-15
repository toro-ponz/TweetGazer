using SourceChord.Lighty;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class NotificationProperties
    {
        public NotificationProperties(TimelineModel timelineModel, UserOverviewProperties sentUser, UserOverviewProperties receiveUser, NotificationPropertiesType type, object parameter = null, long? id = null)
        {
            this.TimelineModel = timelineModel;
            this.NotificationPropertiesType = type;
            this.SentUser = sentUser;
            this.ReceiveUser = receiveUser;

            if (parameter is string)
            {
                this.Text = parameter as string;
            }

            if (id != null)
            {
                this.Id = (long)id;
            }

            this.SelectCommand = new RelayCommand(this.Select);
            this.SelectSentUserCommand = new RelayCommand(this.SelectSentUser);
            this.SelectReceiveUserCommand = new RelayCommand(this.SelectReceiveUser);
        }

        private void Select()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                using (var status = new Views.ShowDialogs.ShowStatus(this.TimelineModel, this.Id))
                {
                    LightBox.ShowDialog(mainWindow, status);
                }
            }
        }

        private void SelectSentUser()
        {
            this.TimelineModel.ShowUserTimeline(this.SentUser);
        }

        private void SelectReceiveUser()
        {
            this.TimelineModel.ShowUserTimeline(this.ReceiveUser);
        }

        public NotificationPropertiesType NotificationPropertiesType { get; }

        public ICommand SelectCommand { get; }
        public ICommand SelectSentUserCommand { get; }
        public ICommand SelectReceiveUserCommand { get; }

        public UserOverviewProperties SentUser { get; }
        public UserOverviewProperties ReceiveUser { get; }

        public string Text { get; }

        public bool IsFavorite
        {
            get
            {
                if (this.NotificationPropertiesType == NotificationPropertiesType.Favorited ||
                    this.NotificationPropertiesType == NotificationPropertiesType.RetweetFavorited)
                {
                    return true;
                }

                return false;
            }
        }
        public bool IsRetweet
        {
            get
            {
                if (this.NotificationPropertiesType == NotificationPropertiesType.Retweeted)
                {
                    return true;
                }

                return false;
            }
        }
        public bool IsFollow
        {
            get
            {
                if (this.NotificationPropertiesType == NotificationPropertiesType.Followed)
                {
                    return true;
                }

                return false;
            }
        }

        private long Id;

        private TimelineModel TimelineModel;
    }

    public enum NotificationPropertiesType
    {
        Favorited,
        RetweetFavorited,
        Retweeted,
        RetweetRetweeted,
        Followed
    }
}
