using Livet;
using System.Windows;
using System.Windows.Input;
using TweetGazer.Common;

namespace TweetGazer.Models.Timeline
{
    public class LoadingProperties : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timelineModel">TimelineModel</param>
        /// <param name="type">ロードタイプ</param>
        /// <param name="parameter">パラメーター</param>
        public LoadingProperties(TimelineModel timelineModel, LoadingType type, object parameter = null)
        {
            this.TimelineModel = timelineModel;
            this.Type = type;
            if (parameter != null)
                this.Parameter = parameter;

            this.Command = new RelayCommand<object>(this.CommandEntityAsync);
        }

        /// <summary>
        /// ローディング
        /// </summary>
        /// <param name="parameter">パラメーター</param>
        private async void CommandEntityAsync(object parameter)
        {
            if (this.Visibility == Visibility.Collapsed)
                return;

            switch (this.Type)
            {
                case LoadingType.ReadMore:
                    if (this.Parameter != null)
                        await this.TimelineModel.Update((long)this.Parameter);
                    break;
                case LoadingType.ReadMoreReplies:
                case LoadingType.ReadMoreRepliesButton:
                    if (this.Parameter != null)
                    {
                        if (parameter is ICommand command)
                        {
                            command.Execute(this.Parameter);
                        }
                    }
                    break;
                case LoadingType.ReadMoreRepliesToMainStatus:
                    if (this.Parameter != null)
                    {
                        if (parameter is ICommand command)
                        {
                            command.Execute(this.Parameter);
                        }
                    }
                    break;
            }
        }

        #region Visibility 変更通知プロパティ
        public Visibility Visibility
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                this._Visibility = value;
                this.RaisePropertyChanged();
            }
        }
        private Visibility _Visibility;
        #endregion

        public LoadingType Type { get; }

        public ICommand Command { get; }

        public object Parameter { get; set; }

        private TimelineModel TimelineModel;
    }

    public enum LoadingType
    {
        ReadMore,
        ReadMoreReplies,
        ReadMoreRepliesButton,
        ReadMoreRepliesToMainStatus
    }
}