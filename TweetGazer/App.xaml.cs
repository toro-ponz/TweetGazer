using MahApps.Metro;
using System;
using System.Windows;

namespace TweetGazer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeManager.AddAccent("DodgerBlue", new Uri("pack://application:,,,/Resources/Dictionaries/Application/AccentColors/DodgerBlue.xaml"));

            base.OnStartup(e);
        }
    }
}
