using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;
using TweetGazer.Models.MainWindow;

namespace TweetGazer.Common
{
    /// <summary>
    /// 特定のクラスに依存しない再利用可能な処理を持つ静的クラス
    /// </summary>
    public static class CommonMethods
    {
        /// <summary>
        /// 文字列を暗号化する
        /// </summary>
        /// <param name="sourceString">暗号化する文字列</param>
        /// <param name="password">暗号化に使用するパスワード</param>
        /// <returns>暗号化された文字列</returns>
        public static string EncryptString(string text, string password)
        {
            //RijndaelManagedオブジェクトを作成
            using (var rijndael = new System.Security.Cryptography.RijndaelManaged())
            {
                //パスワードから共有キーと初期化ベクタを作成
                GenerateKeyFromPassword(password, rijndael.KeySize, out byte[] key, rijndael.BlockSize, out byte[] iv);
                rijndael.Key = key;
                rijndael.IV = iv;

                //文字列をバイト型配列に変換する
                byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(text);

                //対称暗号化オブジェクトの作成
                using (var encryptor = rijndael.CreateEncryptor())
                {
                    //バイト型配列を暗号化する
                    byte[] encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);

                    //バイト型配列を文字列に変換して返す
                    return System.Convert.ToBase64String(encBytes);
                }
            }
        }

        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <returns>復号化された文字列</returns>
        public static string DecryptString(string text, string password)
        {
            //RijndaelManagedオブジェクトを作成
            using (var rijndael = new System.Security.Cryptography.RijndaelManaged())
            {
                //パスワードから共有キーと初期化ベクタを作成
                GenerateKeyFromPassword(password, rijndael.KeySize, out byte[] key, rijndael.BlockSize, out byte[] iv);
                rijndael.Key = key;
                rijndael.IV = iv;

                //文字列をバイト型配列に戻す
                byte[] strBytes = System.Convert.FromBase64String(text);

                //対称暗号化オブジェクトの作成
                using (var decryptor = rijndael.CreateDecryptor())
                {
                    //バイト型配列を復号化する
                    //復号化に失敗すると例外CryptographicExceptionが発生
                    byte[] decBytes = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);

                    //バイト型配列を文字列に戻して返す
                    return System.Text.Encoding.UTF8.GetString(decBytes);
                }
            }
        }

        /// <summary>
        /// パスワードから共有キーと初期化ベクタを生成する
        /// </summary>
        /// <param name="password">基になるパスワード</param>
        /// <param name="keySize">共有キーのサイズ（ビット）</param>
        /// <param name="key">作成された共有キー</param>
        /// <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
        /// <param name="iv">作成された初期化ベクタ</param>
        private static void GenerateKeyFromPassword(string password, int keySize, out byte[] key, int blockSize, out byte[] iv)
        {
            //パスワードから共有キーと初期化ベクタを作成する
            //saltを決める
            byte[] salt = System.Text.Encoding.UTF8.GetBytes("saltは必ず8バイト以上");
            //Rfc2898DeriveBytesオブジェクトを作成する
            using (var deriveBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt))
            {
                deriveBytes.IterationCount = 1000;
                //共有キーと初期化ベクタを生成する
                key = deriveBytes.GetBytes(keySize / 8);
                iv = deriveBytes.GetBytes(blockSize / 8);
            }
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="sound">再生するSEの名称</param>
        public static void PlaySoundEffect(SoundEffect sound)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            SoundPlayer player = null;
            switch (sound)
            {
                case SoundEffect.Notification1:
                    player = new SoundPlayer(assembly.GetManifestResourceStream("TweetGazer.Assets.Sounds.notification01.wav"));
                    break;
                case SoundEffect.Notification2:
                    player = new SoundPlayer(assembly.GetManifestResourceStream("TweetGazer.Assets.Sounds.notification02.wav"));
                    break;
            }

            if (player != null)
            {
                player.Play();
            }
        }

        /// <summary>
        /// 現在メッセージ待ち行列の中にあるUIメッセージを処理する
        /// </summary>
        public static void UpdateUI()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// 通知を行う
        /// </summary>
        /// <param name="message">通知内容</param>
        /// <param name="type">通知タイプ</param>
        public static void Notify(string message, NoticeType type = NoticeType.Normal)
        {
            var mainWindow = MainWindow;
            if (mainWindow != null)
            {
                (mainWindow.DataContext as ViewModels.MainWindowViewModel).Notify(message, type);
            }
        }

        /// <summary>
        /// 2つの時間の差を計算し文字列として表現する
        /// </summary>
        /// <param name="currentTime">大きい方の時間</param>
        /// <param name="time">小さい方の時間</param>
        /// <returns>時間の差</returns>
        public static string CalculateTime(DateTimeOffset currentTime, DateTimeOffset time)
        {
            //TimeSpanを計算する
            var differenceTime = currentTime - time;
            //24時間未満
            if (differenceTime.Days == 0)
            {
                //1時間未満
                if (differenceTime.Hours == 0)
                {
                    //1分未満
                    if (differenceTime.Minutes == 0)
                        return differenceTime.Seconds.ToString(CultureInfo.InvariantCulture) + "秒";
                    else
                        return differenceTime.Minutes.ToString(CultureInfo.InvariantCulture) + "分";
                }
                else
                {
                    return differenceTime.Hours.ToString(CultureInfo.InvariantCulture) + "時間";
                }
            }
            //1週間以内
            else if (differenceTime.Days < 8)
            {
                return differenceTime.Days.ToString(CultureInfo.InvariantCulture) + "日";
            }
            else
            {
                //タイムゾーンをPCで定義されているものにする
                var localTime = CalculateLocalTime(time.DateTime);
                //1年以内
                if (differenceTime.Days < 365)
                    return localTime.Month.ToString(CultureInfo.InvariantCulture) + "月" + localTime.Day.ToString(CultureInfo.InvariantCulture) + "日";

                //それ以前の場合は年月日を返す
                return localTime.Year.ToString(CultureInfo.InvariantCulture) + "年" + localTime.Month.ToString(CultureInfo.InvariantCulture) + "月" + localTime.Day.ToString(CultureInfo.InvariantCulture) + "日";
            }
        }

        /// <summary>
        /// UTC時刻をローカル時刻に変換する
        /// </summary>
        /// <param name="time">UTC時刻</param>
        /// <returns>ローカル時刻</returns>
        public static DateTime CalculateLocalTime(DateTime time)
        {
            return time.Add(TimeZoneInfo.Local.BaseUtcOffset);
        }

        /// <summary>
        /// ビットマップを非同期で読み込む
        /// </summary>
        /// <param name="url">読み込むビットマップのURI</param>
        /// <returns>ビットマップを読み込むタスク</returns>
        public static async Task<BitmapImage> DownloadImageAsync(Uri url)
        {
            try
            {
                using (var web = new HttpClient())
                {
                    var bytes = await web.GetByteArrayAsync(url).ConfigureAwait(false);
                    using (var stream = new MemoryStream(bytes))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }
        }

        /// <summary>
        /// 指定サイズの空画像データを生成する
        /// </summary>
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        /// <returns>画像データ</returns>
        public static ImageSource CreateTemporaryImageAsync(int width, int height)
        {
            if (width <= 0 || height <= 0)
                return null;
            
            using (var bitmap = new Bitmap(width, height))
            {
                var handle = bitmap.GetHbitmap();
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return null;
                }
                finally
                {
                    NativeMethods.DeleteObject(handle);
                }
            }
        }

        /// <summary>
        /// リソースディレクトリを読み込む
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>読み込んだリソースディレクトリ</returns>
        public static ResourceDictionary LoadEmbededResourceDictionary(string path)
        {
            Uri uri = new Uri(path);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            XamlReader reader = new XamlReader();
            //読み込んだファイルをResourceDictionaryとして返す
            return reader.LoadAsync(info.Stream) as ResourceDictionary;
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        /// <param name="targetDirectoryPath">指定ディレクトリのパス</param>
        public static void DeleteDirectory(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
                return;

            try
            {
                //ディレクトリ以外の全ファイルを削除
                string[] filePaths = Directory.GetFiles(targetDirectoryPath);
                foreach (string filePath in filePaths)
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }

                //ディレクトリの中のディレクトリも再帰的に削除
                string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
                foreach (string directoryPath in directoryPaths)
                {
                    DeleteDirectory(directoryPath);
                }

                //中が空になったらディレクトリ自身も削除
                Directory.Delete(targetDirectoryPath, false);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
        
        /// <summary>
        /// 更新されていないか確認する
        /// </summary>
        /// <returns>更新があるか否か</returns>
        public static bool CheckUpdate()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    using (var stream = wc.OpenRead("https://toro-ponz.github.io/tweetgazer/version"))
                    {
                        StreamReader sr = new StreamReader(stream, Encoding.GetEncoding(51932));

                        var asm = Assembly.GetExecutingAssembly();
                        //バージョンの取得
                        var version = asm.GetName().Version;
                        if (version.ToString() == sr.ReadToEnd())
                            return false;
                        else
                            return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                return true;
            }
        }

        /// <summary>
        /// 最初の起動かどうか判定する
        /// </summary>
        /// <returns>成否</returns>
        public static bool CheckFirstBoot()
        {
            try
            {
                //トークンファイルが存在するなら起動済み
                if (System.IO.File.Exists(SecretParameters.TokensFilePath))
                    return false;
                //そうでないなら初回起動とする
                else
                    return true;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return true;
            }
        }

        /// <summary>
        /// メインウィンドウインスタンスを返す
        /// </summary>
        public static Views.MainWindow MainWindow
        {
            get
            {
                try
                {
                    var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is Views.MainWindow);
                    return window as Views.MainWindow;
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    return null;
                }
            }
        }
    }

    public enum SoundEffect
    {
        /// <summary>
        /// 通知音
        /// </summary>
        Notification1,
        /// <summary>
        /// エラー音
        /// </summary>
        Notification2
    }
}