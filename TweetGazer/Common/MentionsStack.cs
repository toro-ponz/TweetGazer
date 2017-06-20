﻿using CoreTweet;
using System.Collections.ObjectModel;
using System.Windows.Data;
using TweetGazer.Models.Timeline;

namespace TweetGazer.Common
{
    public static class MentionsStack
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public static void Initialize()
        {
            if (Mentions == null)
            {
                Mentions = new ObservableCollection<Mention>();
                BindingOperations.EnableCollectionSynchronization(Mentions, new object());
            }
        }

        /// <summary>
        /// メンションをスタックに積む
        /// </summary>
        /// <param name="user">受け取ったユーザー</param>
        /// <param name="status">送られてきたツイート</param>
        public static void StackMention(User user, Status status)
        {
            if (Mentions == null)
            {
                Mentions = new ObservableCollection<Mention>();
                BindingOperations.EnableCollectionSynchronization(Mentions, new object());
            }
            Mentions.Add(new Mention()
            {
                User = new UserOverviewProperties(user),
                Status = status
            });
        }

        /// <summary>
        /// スタックをクリアする
        /// </summary>
        public static void Clear()
        {
            if (Mentions != null)
                Mentions.Clear();
        }

        public static ObservableCollection<Mention> Mentions { get; private set; }
    }
}