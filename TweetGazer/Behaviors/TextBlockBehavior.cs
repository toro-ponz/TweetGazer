using CoreTweet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TweetGazer.Behaviors
{
    public static class TextBlockBehavior
    {
        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static HyperlinkTextProperties GetSource(DependencyObject element)
        {
            if (element == null)
                return null;
            return element.GetValue(SourceProperty) as HyperlinkTextProperties;
        }

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static void SetSource(DependencyObject element, HyperlinkTextProperties value)
        {
            if (element == null)
                return;
            element.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(HyperlinkTextProperties), typeof(TextBlockBehavior), new PropertyMetadata(null, Source_Changed));

        [Localizable(false)]
        private static void Source_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as TextBlock;
            var source = e.NewValue as HyperlinkTextProperties;
            if (element == null)
                return;

            if (source == null || source.Text == null || String.IsNullOrEmpty(source.Text))
            {
                element.Visibility = Visibility.Collapsed;
                return;
            }

            var text = Escape(ReplaceUrl(source.Text, source.Urls));

            var index = 0;

            //ハッシュタグの正規表現
            var hashTagRegex = new Regex(@"(^|\s|\W)(?<HashTag>#\w+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //メンションの正規表現
            var mentionRegex = new Regex(@"(^|[^A-Za-z0-9_])(?<Mention>@[A-Za-z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //URLの正規表現
            var urlRegex = new Regex(@"(?<Url>https?://([\w-]+\.)+[\w-]+(/[-a-zA-Z0-9_./?%&=+~:@#]*)?)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            while (true)
            {
                var hashTagRegexMatch = hashTagRegex.Match(text);
                var mentionRegexMatch = mentionRegex.Match(text);
                var urlRegexMatch = urlRegex.Match(text);

                if (hashTagRegexMatch.Success && (!mentionRegexMatch.Success || hashTagRegexMatch.Index < mentionRegexMatch.Index) && (!urlRegexMatch.Success || hashTagRegexMatch.Index < urlRegexMatch.Index))
                {
                    element.Inlines.Add(new Run()
                    {
                        Text = text.Substring(0, hashTagRegexMatch.Groups["HashTag"].Index)
                    });
                    index = hashTagRegexMatch.Groups["HashTag"].Index + hashTagRegexMatch.Groups["HashTag"].Length;
                    var hyperlink = new Hyperlink()
                    {
                        NavigateUri = new Uri(hashTagRegexMatch.Groups["HashTag"].Value, UriKind.Relative),
                        Foreground = new SolidColorBrush(Colors.DodgerBlue)
                    };
                    //ハッシュタグをクリックした時
                    hyperlink.RequestNavigate += (s, eventArgs) =>
                    {
                        source.HashtagCommand.Execute(eventArgs);
                    };

                    hyperlink.Inlines.Add(new Run()
                    {
                        Text = hashTagRegexMatch.Groups["HashTag"].Value
                    });
                    element.Inlines.Add(hyperlink);
                    text = text.Substring(index);
                }
                else if (mentionRegexMatch.Success && (!urlRegexMatch.Success || mentionRegexMatch.Index < urlRegexMatch.Index))
                {
                    element.Inlines.Add(new Run()
                    {
                        Text = text.Substring(0, mentionRegexMatch.Groups["Mention"].Index),
                    });
                    index = mentionRegexMatch.Groups["Mention"].Index + mentionRegexMatch.Groups["Mention"].Length;
                    var hyperlink = new Hyperlink()
                    {
                        NavigateUri = new Uri(mentionRegexMatch.Groups["Mention"].Value, UriKind.Relative),
                        Foreground = new SolidColorBrush(Colors.DodgerBlue)
                    };
                    //スクリーンネームをクリックした時
                    hyperlink.RequestNavigate += (s, eventArgs) =>
                    {
                        source.MentionCommand.Execute(eventArgs);
                    };

                    hyperlink.Inlines.Add(new Run()
                    {
                        Text = mentionRegexMatch.Groups["Mention"].Value
                    });
                    element.Inlines.Add(hyperlink);
                    text = text.Substring(index);
                }
                else if (urlRegexMatch.Success)
                {
                    element.Inlines.Add(new Run()
                    {
                        Text = text.Substring(0, urlRegexMatch.Groups["Url"].Index)
                    });
                    index = urlRegexMatch.Groups["Url"].Index + urlRegexMatch.Groups["Url"].Length;
                    var hyperlink = new Hyperlink()
                    {
                        NavigateUri = new Uri(urlRegexMatch.Groups["Url"].Value),
                        Foreground = new SolidColorBrush(Colors.DodgerBlue)
                    };

                    //URLをクリックした時
                    hyperlink.RequestNavigate += (s, eventArgs) =>
                    {
                        source.UrlCommand.Execute(eventArgs);
                    };

                    var displayUrl = urlRegexMatch.Groups["Url"].Value;
                    displayUrl = displayUrl.Replace("http://www.", "");
                    displayUrl = displayUrl.Replace("https://www.", "");
                    displayUrl = displayUrl.Replace("http://", "");
                    displayUrl = displayUrl.Replace("https://", "");
                    if (displayUrl.Length > 27)
                        displayUrl = displayUrl.Substring(0, 27) + "...";
                    hyperlink.Inlines.Add(new Run()
                    {
                        Text = displayUrl
                    });
                    element.Inlines.Add(hyperlink);
                    text = text.Substring(index);
                }
                else
                {
                    break;
                }
            }

            text = text.TrimEnd('\n', '\r', ' ');
            if (!String.IsNullOrEmpty(text))
            {
                element.Inlines.Add(new Run()
                {
                    Text = text
                });
            }

        }

        private static string Escape(string text)
        {
            return text.Replace(@"&amp;", "&")
                       .Replace(@"&lt;", "<")
                       .Replace(@"&gt;", ">");
        }

        private static string ReplaceUrl(string text, IList<UrlEntity> urls)
        {
            var result = text;
            foreach (var url in urls)
            {
                result = result.Replace(url.Url, url.ExpandedUrl);
            }
            return result;
        }

        private static int CountSurrogatePair(string text)
        {
            var surrogatePairCount = 0;
            var surrogatePairRegex = new Regex(@"[\uD800-\uDBFF][\uDC00-\uDFFF]");
            var surrogatePairMatch = surrogatePairRegex.Match(text);
            while (surrogatePairMatch.Success)
            {
                surrogatePairCount++;
                surrogatePairMatch = surrogatePairMatch.NextMatch();
            }
            return surrogatePairCount;
        }
    }

    public class HyperlinkTextProperties
    {
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
            }
        }
        private string _Text;

        public IList<HashtagEntity> Hashtags
        {
            get
            {
                return this._Hashtags;
            }
            set
            {
                this._Hashtags = value;
            }
        }
        private IList<HashtagEntity> _Hashtags;
        public IList<UserMentionEntity> Mentions
        {
            get
            {
                return this._Mentions;
            }
            set
            {
                this._Mentions = value;
            }
        }
        private IList<UserMentionEntity> _Mentions;
        public IList<UrlEntity> Urls
        {
            get
            {
                return this._Urls;
            }
            set
            {
                this._Urls = value;
            }
        }
        private IList<UrlEntity> _Urls;
        public ICommand HashtagCommand;
        public ICommand MentionCommand;
        public ICommand UrlCommand;
    }
}