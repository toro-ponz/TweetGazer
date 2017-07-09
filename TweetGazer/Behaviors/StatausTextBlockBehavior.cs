using CoreTweet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TweetGazer.Behaviors
{
    public static class StatausTextBlockBehavior
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
            DependencyProperty.RegisterAttached("Source", typeof(HyperlinkTextProperties), typeof(StatausTextBlockBehavior), new PropertyMetadata(null, Source_Changed));

        [Localizable(false)]
        private static void Source_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as TextBlock;
            var source = e.NewValue as HyperlinkTextProperties;
            if (element == null)
                return;

            if (source == null || source.Text == null || String.IsNullOrEmpty(Escape(source.Text, source.Media)))
            {
                element.Visibility = Visibility.Collapsed;
                return;
            }

            try
            {
                Replace(element, source);
            }
            catch (Exception ex)
            {
                element.Inlines.Clear();
                element.Inlines.Add(new Run()
                {
                    Foreground = new SolidColorBrush(Colors.Red),
                    Text = "ツイート本文の表示処理において、予期せぬエラーが生じました。\nお手数をおかけしますが、別のクライアントからツイート内容をご確認ください。"
                });
                Debug.Write(ex);
            }
        }

        private static void Replace(TextBlock textBlock, HyperlinkTextProperties links)
        {
            int hashtagSuffix = 0;
            int mentionSuffix = 0;
            int urlSuffix = 0;

            if (links.Hashtags == null)
                links.Hashtags = new List<HashtagEntity>();
            if (links.Mentions == null)
                links.Mentions = new List<UserMentionEntity>();
            if (links.Urls == null)
                links.Urls = new List<UrlEntity>();

            int index;
            int treatedIndex = -1;
            var untreatedHashtagExist = hashtagSuffix < links.Hashtags.Count;
            var untreatedMentionExist = mentionSuffix < links.Mentions.Count;
            var untreatedUrlExist = urlSuffix < links.Urls.Count;
            while (true)
            {
                index = 99999;

                if (untreatedHashtagExist)
                    index = Math.Min(index, links.Hashtags[hashtagSuffix].Indices[0]);
                if (untreatedMentionExist)
                    index = Math.Min(index, links.Mentions[mentionSuffix].Indices[0]);
                if (untreatedUrlExist)
                    index = Math.Min(index, links.Urls[urlSuffix].Indices[0]);

                if (index == 99999)
                    break;

                // サロゲートペアを2文字とした場合のindexの数値を計算
                int surrogateIndex = CalculateSurrogateIndex(links.Text, index);

                // 置換するリンクまでをプレーンテキストとして流し込む
                textBlock.Inlines.Add(new Run()
                {
                    Text = Escape(links.Text.Substring(treatedIndex + 1, surrogateIndex - treatedIndex - 1), links.Media)
                });
                treatedIndex = surrogateIndex;

                // ハッシュタグの置換
                if (untreatedHashtagExist && index == links.Hashtags[hashtagSuffix].Indices[0])
                {
                    ReplaceHashtag(textBlock, links.Hashtags[hashtagSuffix], links.HashtagCommand);
                    treatedIndex += links.Hashtags[hashtagSuffix].Indices[1] - links.Hashtags[hashtagSuffix].Indices[0] - 1;
                    hashtagSuffix++;
                }
                // メンションの置換
                else if (untreatedMentionExist && index == links.Mentions[mentionSuffix].Indices[0])
                {
                    ReplaceMention(textBlock, links.Mentions[mentionSuffix], links.MentionCommand);
                    treatedIndex += links.Mentions[mentionSuffix].Indices[1] - links.Mentions[mentionSuffix].Indices[0] - 1;
                    mentionSuffix++;
                }
                // URLの置換
                else if (untreatedUrlExist && index == links.Urls[urlSuffix].Indices[0])
                {
                    ReplaceUrl(textBlock, links.Urls[urlSuffix], links.UrlCommand);
                    treatedIndex += links.Urls[urlSuffix].Indices[1] - links.Urls[urlSuffix].Indices[0] - 1;
                    urlSuffix++;
                }

                untreatedHashtagExist = hashtagSuffix < links.Hashtags.Count;
                untreatedMentionExist = mentionSuffix < links.Mentions.Count;
                untreatedUrlExist = urlSuffix < links.Urls.Count;
            }

            if (LengthInTextElements(links.Text) > treatedIndex + 1)
            {
                textBlock.Inlines.Add(new Run()
                {
                    Text = Escape(links.Text.Substring(treatedIndex + 1), links.Media)
                });
            }
        }

        private static void ReplaceHashtag(TextBlock textBlock, HashtagEntity hashtag, ICommand command)
        {
            var hyperlink = new Hyperlink()
            {
                NavigateUri = new Uri(hashtag.Text, UriKind.Relative),
                Foreground = new SolidColorBrush(Colors.DodgerBlue)
            };
            hyperlink.RequestNavigate += (sender, e) =>
            {
                command.Execute("#" + hashtag.Text);
            };
            hyperlink.Inlines.Add(new Run()
            {
                Text = "#" + hashtag.Text
            });
            textBlock.Inlines.Add(hyperlink);
        }

        private static void ReplaceMention(TextBlock textBlock, UserMentionEntity mention, ICommand command)
        {
            var hyperlink = new Hyperlink()
            {
                NavigateUri = new Uri("@" + mention.ScreenName, UriKind.Relative),
                Foreground = new SolidColorBrush(Colors.DodgerBlue)
            };
            hyperlink.RequestNavigate += (sender, e) =>
            {
                command.Execute(mention.ScreenName);
            };
            hyperlink.Inlines.Add(new Run()
            {
                Text = "@" + mention.ScreenName
            });
            textBlock.Inlines.Add(hyperlink);
        }

        private static void ReplaceUrl(TextBlock textBlock, UrlEntity url, ICommand command)
        {
            var hyperlink = new Hyperlink()
            {
                NavigateUri = new Uri(url.Url),
                Foreground = new SolidColorBrush(Colors.DodgerBlue)
            };
            hyperlink.RequestNavigate += (sender, e) =>
            {
                command.Execute(url.Url);
            };
            hyperlink.Inlines.Add(new Run()
            {
                Text = url.DisplayUrl
            });
            textBlock.Inlines.Add(hyperlink);
        }

        private static int CalculateSurrogateIndex(string text, int index)
        {
            int i = index;
            while (true)
            {
                if (i > 99999)
                    return -1;

                if (text.Length < i)
                    return text.Length;

                // LengthInTextElementsで正しくカウントできない結合文字をカウントする
                var combiningCharacterCount = 0;
                var combiningCharacterCountRegex =  Regex.Match(text.Substring(0, i), @"\p{M}");
                while (combiningCharacterCountRegex.Success)
                {
                    combiningCharacterCount++;
                    combiningCharacterCountRegex = combiningCharacterCountRegex.NextMatch();
                }

                // サロゲートペア前半で切らせない為に超えた段階で抜ける
                if (LengthInTextElements(text.Substring(0, i)) + combiningCharacterCount > index)
                    break;

                i++;
            }

            // 1文字減らして返す
            return i - 1;
        }

        private static int LengthInTextElements(this string str)
        {
            return new StringInfo(str).LengthInTextElements;
        }

        private static string Escape(string text, IList<MediaEntity> media)
        {
            // ツイート画像のURLを削除
            if (media != null && media.Count != 0)
                text = text.Replace(media[0].Url, "");

            return text.Replace(@"&lt;", "<")
                       .Replace(@"&gt;", ">")
                       .Replace(@"&amp;", "&");
        }
    }

    public class HyperlinkTextProperties
    {
        public string Text;
        
        public IList<HashtagEntity> Hashtags;
        public IList<UserMentionEntity> Mentions;
        public IList<UrlEntity> Urls;
        public IList<MediaEntity> Media;

        public ICommand HashtagCommand;
        public ICommand MentionCommand;
        public ICommand UrlCommand;
    }
}