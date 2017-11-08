using System.Windows.Controls;
using TweetGazer.Models.Timeline;
using TweetGazer.ViewModels;

namespace TweetGazer.Views
{
    /// <summary>
    /// Timeline.xaml の相互作用ロジック
    /// </summary>
    public partial class Timeline : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Timeline()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">タイムラインデータ</param>
        public Timeline(TimelineData data)
        {
            this.InitializeComponent();

            this.TimelineViewModel = new TimelineViewModel(data);
            this.DataContext = this.TimelineViewModel;
        }
        
        public TimelineViewModel TimelineViewModel { get; set; }
    }
}
