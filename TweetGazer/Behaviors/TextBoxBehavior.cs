using System.Windows;
using System.Windows.Controls;

namespace TweetGazer.Behaviors
{
    public class TextBoxBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static CaretPosition GetCaretPosition(DependencyObject element)
        {
            if (element == null)
                return CaretPosition.Undefined;
            return (CaretPosition)element.GetValue(CaretPositionProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static void SetCaretPosition(DependencyObject element, CaretPosition value)
        {
            if (element == null)
                return;
            element.SetValue(CaretPositionProperty, value);
        }

        public static readonly DependencyProperty CaretPositionProperty =
            DependencyProperty.RegisterAttached("CaretPosition", typeof(CaretPosition), typeof(TextBoxBehavior), new PropertyMetadata(CaretPosition.Undefined, CaretPosition_Changed));

        private static void CaretPosition_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as TextBox;
            if (element == null || !(e.NewValue is CaretPosition))
                return;

            var caretPosition = (CaretPosition)e.NewValue;

            if (element.Focusable)
                element.Focus();

            switch (caretPosition)
            {
                case CaretPosition.Top:
                    element.CaretIndex = 0;
                    break;
                case CaretPosition.Last:
                    element.CaretIndex = element.Text.Length;
                    break;
            }
        }
    }

    public enum CaretPosition
    {
        Undefined,
        Top,
        Last
    }
}