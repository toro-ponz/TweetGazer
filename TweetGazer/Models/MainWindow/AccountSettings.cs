using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TweetGazer.Common;

namespace TweetGazer.Models.MainWindow
{
    public class AccountSettings : FlyoutBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AccountSettings() : base()
        {
            this.Users = new List<AccountSettingsParameters>();
            this.IsSaving = false;
            this.ReloadUsers();
        }

        /// <summary>
        /// ユーザーの再読み込み
        /// </summary>
        public async void ReloadUsers()
        {
            await Task.Run(() =>
            {
                while (AccountTokens.Users.Count == 0 || this.IsSaving)
                    System.Threading.Thread.Sleep(100);
            });

            this.Users.Clear();
            this.ScreenNames.Clear();
            var i = 0;
            foreach (var user in AccountTokens.Users)
            {
                this.Users.Add(new AccountSettingsParameters(i, user));
                this.ScreenNames.Add("@" + user.ScreenName);
                i++;
            }

            this.TokenSuffix = 0;
        }

        /// <summary>
        /// Flyoutを開くとき
        /// </summary>
        public override void Open()
        {
            base.Open();
            this.ReloadUsers();
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        public async void Save()
        {
            this.IsSaving = true;

            foreach (var user in Users)
            {
                await user.Save();
            }

            this.IsSaving = false;
        }

        /// <summary>
        /// プロフィール画像の選択
        /// </summary>
        public void SelectProfileImage()
        {
            //ファイルダイアログを表示
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "画像ファイル(*.bmp;*.jpg;*.jpeg;.*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif",
                Multiselect = false,
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == false)
                return;

            this.User.ProfileImage = new Uri(ofd.FileName);
        }

        /// <summary>
        /// プロフィールバナーの選択
        /// </summary>
        public void SelectProfileBanner()
        {
            //ファイルダイアログを表示
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "画像ファイル(*.bmp;*.jpg;*.jpeg;.*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif",
                Multiselect = false,
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == false)
                return;

            this.User.ProfileBanner = new Uri(ofd.FileName);
        }

        /// <summary>
        /// プロフィールバナーの削除
        /// </summary>
        public async void RemoveProfileBanner()
        {
            var mainWindow = CommonMethods.MainWindow;
            if (mainWindow != null)
            {
                if (await mainWindow.ShowMessageAsync("確認", "プロフィールバナーを削除しますか？", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    await this.User.RemoveProfileBanner();
                }
            }
        }

        #region User 変更通知プロパティ
        public AccountSettingsParameters User
        {
            get
            {
                if (this.TokenSuffix >= this.Users.Count || this.TokenSuffix == -1)
                    return null;
                return this.Users[TokenSuffix];
            }
            set
            {
                this.Users[TokenSuffix] = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region TokenSuffix 変更通知プロパティ
        public override int TokenSuffix
        {
            get => base.TokenSuffix;
            set
            {
                base.TokenSuffix = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public IList<AccountSettingsParameters> Users;

        private bool IsSaving;
    }
}