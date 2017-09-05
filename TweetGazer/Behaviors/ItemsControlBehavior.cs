using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TweetGazer.Behaviors
{
    class ItemsControlBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static Types GetAddingScroll(ItemsControl element)
        {
            return (Types)element.GetValue(AddingScrollProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static void SetAddingScroll(ItemsControl element, Types value)
        {
            element.SetValue(AddingScrollProperty, value);
        }

        public static readonly DependencyProperty AddingScrollProperty =
            DependencyProperty.RegisterAttached("AddingScroll", typeof(Types), typeof(ItemsControlBehavior), new PropertyMetadata(Types.Others, AddingScroll_Changed));

        private static void AddingScroll_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl == null)
                return;

            itemsControl.ItemContainerGenerator.ItemsChanged += async (s, eventArgs) =>
            {
                switch (eventArgs.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        Add(itemsControl, eventArgs);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        break;
                }
            };
        }

        public static async void Add(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl == null)
                return;

            // 最上位への追加でない場合リターン
            if (e.Position.Index > 0)
                return;

            ScrollViewer scrollViewer = null;
            var controlTemplate = itemsControl.Template;
            if (controlTemplate != null)
                scrollViewer = controlTemplate.FindName("ScrollViewer", itemsControl) as ScrollViewer;

            if (scrollViewer != null)
            {
                var height = scrollViewer.ExtentHeight;
                var verticalOffset = ScrollViewerBehavior.GetVerticalOffset(scrollViewer);
                var type = GetAddingScroll(itemsControl);
                // 少しでもスクロールされている場合、その表示内容が同じ位置に来るように再計算
                if (type == Types.Timeline && verticalOffset != 0 || type == Types.Statuses)
                {
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < 3000; i++)
                        {
                            // 描画されるまで最大3秒待つ
                            if (height != scrollViewer.ExtentHeight)
                                break;
                            System.Threading.Thread.Sleep(1);
                        }
                    });

                    // 追加後の高さ
                    var newHeight = scrollViewer.ExtentHeight;
                    // 増えた高さ分だけスクロール位置を下げる
                    ScrollViewerBehavior.SetVerticalOffset(scrollViewer, ScrollViewerBehavior.GetVerticalOffset(scrollViewer) + (newHeight - height));
                }
            }
        }
    }

    public enum Types
    {
        Timeline,
        Statuses,
        Others
    }
}