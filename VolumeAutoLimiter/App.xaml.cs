using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using VolumeAutoLimiter.Models;

namespace VolumeAutoLimiter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

#if DEBUG
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
#endif
        public static string Name { get; } = Assembly.GetExecutingAssembly().GetName().Name;
        public static Mutex Mutex = new(false, App.Name);

        /// <summary>
        /// 重複起動しているか
        /// </summary>
        /// <returns>true: 重複起動</returns>
        public static bool IsDuplicateRunning()
        {
            bool ret = !App.Mutex.WaitOne(0, false);
            // 重複起動時はMutexを閉じる
            if (ret) App.Mutex.Close();

            Console.WriteLine($"AppName: {App.Name}");
            Console.WriteLine($"Duplicate: {ret}");

            return ret;
        }

        /// <summary>
        /// 表示する言語の取得
        /// </summary>
        /// <returns>表示する言語のResourceDictionary</returns>
        public static ResourceDictionary GetLanguage()
        {
            // https://ja.wikipedia.org/wiki/ISO_639-1%E3%82%B3%E3%83%BC%E3%83%89%E4%B8%80%E8%A6%A7
            var cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return GetLanguage(cultureCode);
        }

        /// <summary>
        /// 表示する言語の取得
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns>表示する言語のResourceDictionary</returns>
        public static ResourceDictionary GetLanguage(string cultureCode)
        {
            var dictionary = new ResourceDictionary();
            try
            {
                var uri = GlobalParameter.GetLanguagePath(cultureCode);
                dictionary.Source = new Uri(uri, UriKind.Relative);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Language Uri Error: {ex}");
                Console.WriteLine($"Used default {GlobalParameter.DefaultCultureCode}.");
                var uri = GlobalParameter.GetLanguagePath();
                dictionary.Source = new Uri(uri, UriKind.Relative);
            }

            return dictionary;
        }

        /// <summary>
        /// 言語文字列の取得
        /// </summary>
        public static String GetString(string key)
        {
            return (string)GetLanguage()[key];
        }

        /// <summary>
        /// アプリ開始時に処理する関数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {

#if DEBUG
            // デバッグ用コンソールの表示
            AllocConsole();
            Console.SetWindowSize(30, 10);
            Console.OutputEncoding = System.Text.Encoding.UTF8;
#else
            // 想定外の例外を処理するイベントハンドラを登録
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

            // 重複起動の確認
            if (IsDuplicateRunning())
            {
                MessageBox.Show("This application is already running.", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(1);
            }


            // 言語設定
            var language = GetLanguage();
            Resources.MergedDictionaries[0] = language;

        }

        /// <summary>
        /// アプリ終了時に処理する関数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Mutexを解放してからアプリを閉じる
            App.Mutex.ReleaseMutex();
        }

        /// <summary>
        /// 想定外の例外を処理する関数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            Exception exObj = (Exception)ex.ExceptionObject;
            MessageBox.Show(exObj.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
    }
}
