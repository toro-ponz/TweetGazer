using Livet;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TweetGazer.Models.MainWindow;
using TweetGazer.Models.Timeline;

namespace TweetGazer.ViewModels.MainWindow
{
    public class TimelinesGridViewModel : ViewModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimelinesGridViewModel() : base()
        {
            this.TimelinesGrid = new TimelinesGrid();

            this.Grid = TimelinesGrid.Grid;
            this.Timelines = TimelinesGrid.Timelines;
        }

        /// <summary>
        /// 各TLのカラム幅を設定する
        /// </summary>
        /// <param name="width">カラム幅</param>
        public void SetGridWidth(IList<GridLength> width)
        {
            this.TimelinesGrid.SetGridWidth(width);
        }

        /// <summary>
        /// タイムラインを追加する
        /// </summary>
        /// <param name="data">追加するタイムラインデータ</param>
        public void AddTimeline(TimelineData data)
        {
            this.TimelinesGrid.AddTimeline(data);
        }

        /// <summary>
        /// タイムラインを削除する
        /// </summary>
        /// <param name="columnIndex">削除するタイムライン番号</param>
        public void RemoveTimeline(int columnIndex)
        {
            this.TimelinesGrid.RemoveTimeline(columnIndex);
        }

        /// <summary>
        /// 前回終了時のカラムを読み込む
        /// </summary>
        /// <returns>正常に読み込めたか否か</returns>
        public bool LoadColumnData()
        {
            return this.TimelinesGrid.LoadColumnData();
        }

        /// <summary>
        /// 終了時に現在のカラムを保存する
        /// </summary>
        public void SaveColumnData()
        {
            this.TimelinesGrid.SaveColumnData();
        }

        public ObservableCollection<Grid> Grid { get; }
        public IList<Views.Timeline> Timelines { get; }

        private TimelinesGrid TimelinesGrid;
    }
}