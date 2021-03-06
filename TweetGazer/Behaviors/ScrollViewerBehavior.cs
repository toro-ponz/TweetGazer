﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace TweetGazer.Behaviors
{
    public static class ScrollViewerBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static bool? GetAnimationScrolling(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            return element.GetValue(AnimationScrollingProperty) as bool?;
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static void SetAnimationScrolling(DependencyObject element, bool? value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(AnimationScrollingProperty, value);
        }

        public static readonly DependencyProperty AnimationScrollingProperty =
            DependencyProperty.RegisterAttached("AnimationScrolling", typeof(bool?), typeof(ScrollViewerBehavior), new PropertyMetadata(null, AnimationScrolling_Changed));

        private static void AnimationScrolling_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ScrollViewer element)
            {
                if (e.NewValue as bool? == true)
                {
                    CreateAnimation();
                    element.ScrollChanged += ScrollChanged;
                    element.PreviewMouseWheel += PreviewMouseWheel;
                }
                else
                {
                    element.ScrollChanged -= ScrollChanged;
                    element.PreviewMouseWheel -= PreviewMouseWheel;
                }
            }
        }
        
        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static void SetScrollAmount(DependencyObject element, double value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(ScrollAmountProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static double GetScrollAmount(DependencyObject element)
        {
            if (element == null)
            {
                return 0;
            }

            return (double)element.GetValue(ScrollAmountProperty);
        }

        public static readonly DependencyProperty ScrollAmountProperty =
            DependencyProperty.RegisterAttached("ScrollAmount", typeof(double), typeof(ScrollViewerBehavior), new PropertyMetadata(300.0));

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static double GetOffsetMediator(DependencyObject element)
        {
            if (element == null)
            {
                return 0;
            }

            return (double)element.GetValue(OffsetMediatorProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static void SetOffsetMediator(DependencyObject element, double value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(OffsetMediatorProperty, value);
        }

        public static readonly DependencyProperty OffsetMediatorProperty =
            DependencyProperty.RegisterAttached("OffsetMediator", typeof(double), typeof(ScrollViewerBehavior), new PropertyMetadata(0.0d, OffsetMediator_Changed));

        private static void OffsetMediator_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as ScrollViewer;
            if (element == null)
            {
                return;
            }

            element.ScrollToVerticalOffset((double)e.NewValue);
            SetOffsetMediator(element, (double)e.NewValue);
            SetVerticalOffset(element, (double)e.NewValue);
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static double GetVerticalOffset(DependencyObject element)
        {
            if (element == null)
            {
                return 0;
            }

            return (double)element.GetValue(VerticalOffsetProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static void SetVerticalOffset(DependencyObject element, double value)
        {
            if (element == null)
            {
                return;
            }

            element.SetValue(VerticalOffsetProperty, value);
        }

        public static DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerBehavior), new PropertyMetadata(0.0d, VerticalOffset_Changed));

        private static void VerticalOffset_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            var element = sender as ScrollViewer;
            if (element == null)
            {
                return;
            }

            OffsetMediator_Changed(sender, e);
        }

        private static void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 一旦スムーズスクロールをOFFにする
            return;

            if (Properties.Settings.Default.IsSmoothScrolling &&
                Animate(sender, -Math.Sign(e.Delta / 2) * GetScrollAmount(sender as ScrollViewer)))
            {
                e.Handled = true;
            }
        }

        private static void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var element = sender as ScrollViewer;
            if (element == null)
            {
                return;
            }

            SetVerticalOffset(element, e.VerticalOffset);
        }

        private static bool Animate(object sender, double offset)
        {
            var element = sender as ScrollViewer;
            if (element == null)
            {
                return false;
            }

            if (Math.Sign(offset) != Direction)
            {
                Target = GetVerticalOffset(element);
                Direction = Math.Sign(offset);
            }

            Target += offset;
            Target = Math.Max(Math.Min(Target, element.ScrollableHeight), 0);

            Storyboard.SetTarget(Animation, element);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(OffsetMediatorProperty));

            Animation.To = Target;
            Animation.From = GetVerticalOffset(element);

            if (Animation.From != Animation.To)
            {
                element.BeginAnimation(OffsetMediatorProperty, Animation);
                return true;
            }

            return false;
        }

        private static void CreateAnimation()
        {
            if (Animation != null)
            {
                return;
            }

            Animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.2d),
                EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut }
            };
            Animation.Completed += (s, e) => { Direction = 0; };
        }

        private static double Target = 0.0d;
        private static int Direction = 0;
        private static DoubleAnimation Animation;
    }
}
