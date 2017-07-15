using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TweetGazer.Common;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Models.MainWindow
{
    public class TimelinesGrid : NotificationObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimelinesGrid()
        {
            this.Grid = new ObservableCollection<Grid>
            {
                new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                }
            };
            BindingOperations.EnableCollectionSynchronization(this.Grid, new object());
            this.Timelines = new List<Views.Timeline>();
        }

        /// <summary>
        /// タイムラインを追加する
        /// </summary>
        /// <param name="data">タイムラインデータ</param>
        public void AddTimeline(TimelineData data)
        {
            if (data == null)
                return;

            data.GridWidth = new GridLength(1.0, GridUnitType.Star);

            //Gridが空の時はGridSplitterを挿入しない
            if (this.Grid.First().Children.Count != 0)
            {
                //列幅のリセット
                for (int i = 0; i < this.Timelines.Count; i++)
                {
                    this.Timelines[i].TimelineViewModel.SetGridWidth(new GridLength(1.0, GridUnitType.Star));
                }

                this.Grid.First().ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(2)
                });
                var gridSpliter = new GridSplitter()
                {
                    Width = 2,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                gridSpliter.SetResourceReference(Control.BackgroundProperty, "AccentColorBrush");
                System.Windows.Controls.Grid.SetColumn(gridSpliter, this.Grid.First().Children.Count);
                this.Grid.First().Children.Add(gridSpliter);
            }

            //タイムラインを生成
            var timeline = new Views.Timeline(data);
            //タイムライン用のカラムを生成
            var columnDefinition = new ColumnDefinition()
            {
                Width = new GridLength(1, GridUnitType.Star)
            };
            //タイムラインの幅をカラムにバインド
            var width = new Binding("Timelines.Timelines[" + this.Timelines.Count + "].TimelineViewModel.GridWidth")
            {
                Mode = BindingMode.TwoWay
            };
            columnDefinition.SetBinding(ColumnDefinition.WidthProperty, width);
            this.Grid.First().ColumnDefinitions.Add(columnDefinition);
            data.ColumnIndex = this.Timelines.Count;
            System.Windows.Controls.Grid.SetColumn(timeline, this.Grid.First().Children.Count);
            this.Grid.First().Children.Add(timeline);
            this.Timelines.Add(timeline);
        }

        /// <summary>
        /// タイムラインを削除する
        /// </summary>
        /// <param name="columnIndex">カラム番号</param>
        public void RemoveTimeline(int columnIndex)
        {
            if (this.Timelines.Count <= columnIndex)
                return;

            var index = columnIndex * 2;
            //要素が1つのみの場合
            if (index == 0 && this.Grid.First().Children.Count == 1)
            {
                this.Timelines.RemoveAt(columnIndex);
                this.Grid.First().Children.RemoveAt(index);
                this.Grid.First().ColumnDefinitions.RemoveAt(index);
            }
            //最も右のタイムラインの場合
            else if (index == this.Grid.First().Children.Count - 1)
            {
                this.Timelines.RemoveAt(columnIndex);
                this.Grid.First().Children.RemoveAt(index);
                this.Grid.First().ColumnDefinitions.RemoveAt(index);
                //GridSplitterの削除
                this.Grid.First().Children.RemoveAt(index - 1);
                this.Grid.First().ColumnDefinitions.RemoveAt(index - 1);
            }
            //その他の場合はタイムラインの右のGridSplitterを削除
            else
            {
                this.Timelines.RemoveAt(columnIndex);
                this.Grid.First().Children.RemoveAt(index);
                this.Grid.First().ColumnDefinitions.RemoveAt(index);
                //GridSplitterの削除
                this.Grid.First().Children.RemoveAt(index);
                this.Grid.First().ColumnDefinitions.RemoveAt(index);
                //Grid.Columnの再計算
                for (int i = index; i < this.Grid.First().Children.Count; i++)
                {
                    System.Windows.Controls.Grid.SetColumn(this.Grid.First().Children[i], i);
                    //タイムライン幅の再バインディング
                    if (i % 2 == 0)
                    {
                        var width = new Binding("Timelines.Timelines[" + (int)(i / 2) + "].TimelineViewModel.GridWidth")
                        {
                            Mode = BindingMode.TwoWay
                        };
                        this.Grid.First().ColumnDefinitions[i].SetBinding(ColumnDefinition.WidthProperty, width);
                    }
                }
                //タイムラインのColumnIndex値の再計算
                for (int i = columnIndex; i < this.Timelines.Count; i++)
                {
                    this.Timelines[i].TimelineViewModel.SetColumnIndex(i);
                }
            }

        }

        /// <summary>
        /// 各タイムラインの横幅を設定する
        /// </summary>
        /// <param name="gridWidth">各横幅のリスト</param>
        public void SetGridWidth(IList<GridLength> gridWidth)
        {
            if (gridWidth == null || this.Timelines.Count != gridWidth.Count)
                return;

            for (int i = 0; i < this.Timelines.Count; i++)
            {
                this.Timelines[i].TimelineViewModel.SetGridWidth(gridWidth[i]);
            }
        }

        /// <summary>
        /// 前回終了時のカラムを読み込む
        /// </summary>
        /// <returns></returns>
        public bool LoadColumnData()
        {
            if (!File.Exists(SecretParameters.TimelineColumnFilePath))
                return false;

            var loadedText = "";
            //ファイルから読み込み
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(SecretParameters.TimelineColumnFilePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream))
                {
                    fileStream = null;
                    loadedText = CommonMethods.DecryptString(streamReader.ReadToEnd(), SecretParameters.TimelineColumnEncryptionKey);
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }

            var regex = new Regex(@"<TimelineColumn>(?<Data>.+?)</TimelineColumn>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = regex.Match(loadedText);
            int i = 0;
            var gridWidth = new List<GridLength>();
            while (match.Success)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TimelineData));
                using (var reader = new StringReader(match.Groups["Data"].Value))
                {
                    TimelineData item = (TimelineData)serializer.Deserialize(reader);
                    gridWidth.Add(item.GridWidth);
                    this.AddTimeline(item);
                    match = match.NextMatch();
                }
                i++;
            }

            if (this.Timelines.Count == 0)
                return false;

            this.SetGridWidth(gridWidth);
            return true;
        }

        /// <summary>
        /// 終了時にカラムを保存する
        /// </summary>
        /// <returns></returns>
        public bool SaveColumnData()
        {
            //書き込むトークン文字列を生成
            var writeText = "";

            foreach (var tl in this.Timelines)
            {
                writeText += "<TimelineColumn>" + tl.TimelineViewModel.Serialize() + "</TimelineColumn>\n";
            }
            writeText = CommonMethods.EncryptString(writeText, SecretParameters.TimelineColumnEncryptionKey);

            //ディレクトリが存在しない場合作成する
            if (!Directory.GetParent(SecretParameters.TimelineColumnFilePath).Exists)
            {
                Directory.CreateDirectory(Directory.GetParent(SecretParameters.TimelineColumnFilePath).FullName);
            }

            //ファイルへの書き込み
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(SecretParameters.TimelineColumnFilePath, FileMode.Create, FileAccess.Write);
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    fileStream = null;
                    streamWriter.Write(writeText);
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return false;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }

            return true;
        }

        public IList<Views.Timeline> Timelines { get; }

        public ObservableCollection<Grid> Grid { get; }
    }
}