﻿using CoreTweet.Streaming;
using Livet;
using System;
using System.Collections.Generic;
using System.Timers;
using TweetGazer.Common;
using System.Collections.ObjectModel;
using TweetGazer.Models.MainWindow;
using System.Windows.Data;
using System.Threading.Tasks;
using MahApps.Metro;
using TweetGazer.ViewModels;

namespace TweetGazer.Models
{
    public class MainWindowModel : NotificationObject, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowModel(MainWindowViewModel mainWindowViewModel)
        {
            this.MainWindowViewModel = mainWindowViewModel;

            this.DebugConsoleWindow = new Views.DebugConsoleWindow();

            //tempフォルダを削除
            CommonMethods.DeleteDirectory(SecretParameters.TemporaryDirectoryPath);

            this.ToastNotifications = new ObservableCollection<ToastNotification>();
            BindingOperations.EnableCollectionSynchronization(this.ToastNotifications, new object());
            
            this.Timers = new List<Timer>();
        }

        /// <summary>
        /// 通知を行う
        /// </summary>
        /// <param name="message">通知内容</param>
        /// <param name="type">通知タイプ</param>
        public async void Notify(string message, NotificationType type)
        {
            try
            {
                await Task.Run(async  () =>
                {
                    this.ToastNotifications.Add(new ToastNotification(message, type));
                    await Task.Delay(5000);
                    this.RemoveNotice();
                });
            }
            catch (Exception e)
            {
                DebugConsole.Write(e);
            }
        }

        /// <summary>
        /// 通知を削除する
        /// </summary>
        /// <param name="noticeNumber">削除する通知番号</param>
        private void RemoveNotice(int noticeNumber = 0)
        {
            if (noticeNumber < this.ToastNotifications.Count)
            {
                this.ToastNotifications.RemoveAt(noticeNumber);
            }
        }

        /// <summary>
        /// デバッグコンソールを開く
        /// </summary>
        public void DebugConsoleOpen()
        {
            if (this.DebugConsoleWindow.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.DebugConsoleWindow.Show();
            }
            else
            {
                this.DebugConsoleWindow.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// ウィンドウのカラーを変更する
        /// </summary>
        public void ChangeColors()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                try
                {
                    var accentColor = Properties.Settings.Default.AccentColor;
                    var baseColor = Properties.Settings.Default.BaseColor;

                    if (accentColor == null || baseColor == null)
                    {
                        return;
                    }

                    ThemeManager.ChangeAppStyle(
                            mainWindow.Resources,
                            ThemeManager.GetAccent(accentColor),
                            ThemeManager.GetAppTheme(baseColor));
                }
                catch (Exception e)
                {
                    this.Notify("アクセントカラー・テーマカラーの変更中にエラーが発生しました", NotificationType.Error);
                    DebugConsole.Write(e);
                }
            }
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        /// <param name="disposing">マネージリソースを破棄するか否か</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            if (this.Timers != null)
            {
                foreach (var timer in this.Timers)
                {
                    timer.Dispose();
                }
            }

            this.DebugConsoleWindow.Visibility = System.Windows.Visibility.Collapsed;
            this.DebugConsoleWindow.Close();

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// メンションを受け取ったとき
        /// </summary>
        /// <param name="tokenSuffix">アカウント番号</param>
        /// <param name="statusMessage">メンション</param>
        private void ReceiveMention(int tokenSuffix, StatusMessage statusMessage)
        {
            if (statusMessage.Status.CurrentUserRetweet != null)
            {
                return;
            }

            if (Properties.Settings.Default.IsNotify)
            {
                var text = "";
                if (statusMessage.Status.ExtendedTweet != null && !string.IsNullOrEmpty(statusMessage.Status.ExtendedTweet.FullText))
                {
                    text = statusMessage.Status.ExtendedTweet.FullText;
                }
                else if (statusMessage.Status.FullText != null && !string.IsNullOrEmpty(statusMessage.Status.FullText))
                {
                    text = statusMessage.Status.FullText;
                }
                else
                {
                    text = statusMessage.Status.Text;
                }

                CommonMethods.PlaySoundEffect(SoundEffect.Notification1);
                this.Notify(statusMessage.Status.User.Name + "さんからのメンション\n" + text, NotificationType.Normal);
            }

            if (statusMessage.Status.RetweetedStatus != null)
            {
                NotificationsStack.StackNotification(
                    statusMessage.Status.User,
                    statusMessage.Status.RetweetedStatus.User,
                    Timeline.NotificationPropertiesType.Retweeted,
                    statusMessage.Status.User.Name + "さんにツイートがリツイートされました。\n" + statusMessage.Status.RetweetedStatus.Text,
                    statusMessage.Status.RetweetedStatus.Id);
            }
            else
            {
                MentionsStack.StackMention(statusMessage.Status);
            }
        }

        /// <summary>
        /// イベントが流れてきたとき
        /// </summary>
        /// <param name="tokenSuffix">アカウント番号</param>
        /// <param name="eventMessage">イベント</param>
        private void ReceiveEventMessage(int tokenSuffix, EventMessage eventMessage)
        {
            if (eventMessage.Source.Id == AccountTokens.Users[tokenSuffix].Id)
            {
                return;
            }

            var isNotify = false;
            var isPlaySound = false;
            var text = "";

            switch (eventMessage.Event)
            {
                case EventCode.AccessRevoked:
                    break;
                case EventCode.Block:
                    break;
                case EventCode.Favorite:
                    isNotify = true;
                    isPlaySound = true;
                    text = eventMessage.Source.Name + "さんにいいねされました。\n" + eventMessage.TargetStatus.Text;
                    NotificationsStack.StackNotification(eventMessage.Source, eventMessage.Target, Timeline.NotificationPropertiesType.Favorited, text, eventMessage.TargetStatus.Id);
                    break;
                case EventCode.FavoritedRetweet:
                    isNotify = true;
                    isPlaySound = true;
                    text = eventMessage.Source.Name + "さんにリツイートをいいねされました。\n" + eventMessage.TargetStatus.Text;
                    NotificationsStack.StackNotification(eventMessage.Source, eventMessage.Target, Timeline.NotificationPropertiesType.RetweetFavorited, text, eventMessage.TargetStatus.Id);
                    break;
                case EventCode.Follow:
                    isNotify = true;
                    isPlaySound = true;
                    text = eventMessage.Source.Name + "さんにフォローされました。";
                    NotificationsStack.StackNotification(eventMessage.Source, eventMessage.Target, Timeline.NotificationPropertiesType.Followed, text);
                    break;
                case EventCode.ListCreated:
                    break;
                case EventCode.ListDestroyed:
                    break;
                case EventCode.ListMemberAdded:
                    break;
                case EventCode.ListMemberRemoved:
                    break;
                case EventCode.ListUpdated:
                    break;
                case EventCode.ListUserSubscribed:
                    break;
                case EventCode.ListUserUnsubscribed:
                    break;
                case EventCode.Mute:
                    break;
                case EventCode.QuotedTweet:
                    isNotify = true;
                    isPlaySound = true;
                    text = eventMessage.Source.Name + "さんに引用されました。\n" + eventMessage.TargetStatus.Text;
                    break;
                case EventCode.RetweetedRetweet:
                    isNotify = true;
                    isPlaySound = true;
                    text = eventMessage.Source.Name + "さんにリツイートをリツイートされました。\n" + eventMessage.TargetStatus.Text;
                    break;
                case EventCode.Unblock:
                    break;
                case EventCode.Unfavorite:
                    break;
                case EventCode.Unfollow:
                    break;
                case EventCode.Unmute:
                    break;
                case EventCode.UserUpdate:
                    break;
            }

            if (isPlaySound)
            {
                CommonMethods.PlaySoundEffect(SoundEffect.Notification1);
            }
            if (isNotify)
            {
                this.Notify(text, NotificationType.Normal);
            }
        }

        /// <summary>
        /// DMが流れてきたとき
        /// </summary>
        /// <param name="directMessage">DM</param>
        private void ReceiveDirectMessage(CoreTweet.DirectMessage directMessage)
        {

        }

        public ObservableCollection<ToastNotification> ToastNotifications { get; }

        private List<IDisposable> Disposables;
        private List<Timer> Timers;

        private Views.DebugConsoleWindow DebugConsoleWindow;
        private MainWindowViewModel MainWindowViewModel;
    }
}
