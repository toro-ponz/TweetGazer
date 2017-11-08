using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;

namespace TweetGazer.Behaviors
{
    public static class ReadMoreBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(ProgressRing))]
        public static ICommand GetCommand(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            return element.GetValue(CommandProperty) as ICommand;
        }

        [AttachedPropertyBrowsableForType(typeof(ProgressRing))]
        public static void SetCommand(DependencyObject element, ICommand value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ReadMoreBehavior), new PropertyMetadata(Command_Changed));

        private static void Command_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as ProgressRing;
            var command = e.NewValue as ICommand;
            if (element == null || command == null)
            {
                return;
            }

            element.Loaded += (s, eventArgs) =>
            {
                element.IsActive = true;
                command.Execute(GetCommandParameter(element));
            };
        }

        [AttachedPropertyBrowsableForType(typeof(ProgressRing))]
        public static object GetCommandParameter(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            return element.GetValue(CommandParameterProperty) as object;
        }

        [AttachedPropertyBrowsableForType(typeof(ProgressRing))]
        public static void SetCommandParameter(DependencyObject element, object value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ReadMoreBehavior), new PropertyMetadata(null));
    }
}
