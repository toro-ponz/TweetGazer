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
            {
                this.Parameter = parameter;
            }

            this.LoadCommand = new RelayCommand<object>(this.Load);
        }

        /// <summary>
        /// ロード
        /// </summary>
        /// <param name="parameter">パラメーター</param>
        private async void Load(object parameter)
        {
            if (this.Parameter == null || this.Visibility == Visibility.Collapsed)
            {
                return;
            }

            switch (this.Type)
            {
                case LoadingType.ReadMore:
                    await this.TimelineModel.Update((long)this.Parameter);
                    break;
                case LoadingType.ReadMoreReplies:
                case LoadingType.ReadMoreRepliesButton:
                    {
                        if (parameter is ICommand command)
                        {
                            command?.Execute(this.Parameter);
                        }
                    }
                    break;
                case LoadingType.ReadMoreRepliesToMainStatus:
                    {
                        if (parameter is ICommand command)
                        {
                            command?.Execute(this.Parameter);
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

        public ICommand LoadCommand { get; }

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
