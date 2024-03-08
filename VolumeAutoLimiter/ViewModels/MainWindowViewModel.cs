using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using VolumeAutoLimiter.Models;

namespace VolumeAutoLimiter.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly Window parentWindow;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly NotifyIcon notifyIcon;
        private readonly Settings settings;
        private readonly MMDeviceEnumerator deviceEnumerator = new();
        private MMDevice? currentDevice;
        private Thread? thread;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel(Window window)
        {
            Console.WriteLine("MainWindowViewModel");
            parentWindow = window;

            // 設定値を読み込む
            settings = Settings.Load();
            LimitVolume = settings.Volume;

            // 通知アイコンの設定
            var exePath = Assembly.GetExecutingAssembly().Location;
            ContextMenuStrip contextMenuStrip = new();
            contextMenuStrip.Items.Add((string)App.GetString("open"), null, (s, e) => parentWindow.WindowState = WindowState.Normal);
            contextMenuStrip.Items.Add((string)App.GetString("close"), null, (s, e) =>
            {
                ParentWindow_Closing(null, null);
                Environment.Exit(0);
            });
            notifyIcon = new()
            {
                Visible = true,
                Text = (string)App.GetString("title"),
                Icon = new Icon(Path.Combine(Path.GetDirectoryName(exePath), "sound.ico")),
                ContextMenuStrip = contextMenuStrip, // 右クリック用
            };

            // イベントハンドラの登録
            parentWindow.StateChanged += (s, e) => parentWindow.ShowInTaskbar = parentWindow.WindowState != WindowState.Minimized;
            parentWindow.Loaded += ParentWindow_Loaded;
            parentWindow.Closing += ParentWindow_Closing;

        }

        private void ParentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ParentWindow_Loaded");

            // 画面表示準備ができたら設定値保存
            Console.WriteLine("    Save settings.");
            Update();

            // スレッドで監視
            Console.WriteLine("    Start MonitorOutputVolume.");
            thread = new Thread(new ParameterizedThreadStart(MonitorOutputVolume));
            thread.Start(cancellationTokenSource.Token);
        }

        private void ParentWindow_Closing(object? sender, CancelEventArgs? e)
        {
            Console.WriteLine("ParentWindow_Closing");

            // 通知アイコンの削除
            Console.WriteLine("    Delete NotifyIcon.");
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            // スレッドの終了
            Console.WriteLine("    Thread cancel.");
            cancellationTokenSource.Cancel();
            Console.WriteLine("        Wait.");
            thread?.Join(3000);
            // 設定値の保存
            Console.WriteLine("    Save settings.");
            Update();
        }

        /// <summary>
        /// デフォルトの再生デバイスを取得
        /// </summary>
        private MMDevice? GetDefaultDevice
        {
            get
            {
                try
                {
                    return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                }
                catch (COMException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 出力音量の監視
        /// </summary>
        /// <param name="obj"></param>
        private void MonitorOutputVolume(object obj)
        {
            Console.WriteLine("MonitorOutputVolume: Started.");

            var token = (CancellationToken)obj;
            while (!token.IsCancellationRequested)
            {
                // デフォルトの再生デバイスを取得
                var device = GetDefaultDevice;
                // デバイス名の更新
                DeviceName = device?.FriendlyName;
                Console.WriteLine($"MonitorOutputVolume: Device -> {DeviceName}");
                // デバイス変更検知
                var deviceChanged = currentDevice?.FriendlyName != device?.FriendlyName;
                if (deviceChanged)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(() => parentWindow.WindowState = WindowState.Normal);
                }
                // 出力音量の算出 (デバイスが変更されたら音量0にする)
                var volume = (int)Math.Round(((deviceChanged) ?
                    0 : (device?.AudioEndpointVolume.MasterVolumeLevelScalar ?? 0)) * 100f);
                // 出力音量の更新
                Console.WriteLine($"MonitorOutputVolume: Current Volume -> {device?.AudioEndpointVolume.MasterVolumeLevelScalar * 100f}");
                currentDevice = device;
                VolumeChange(volume);
                Console.WriteLine($"MonitorOutputVolume: Volume -> {volume} to {OutputVolume}");

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 制限音量が保存されたか
        /// </summary>
        [ObservableProperty]
        private bool savedVolume = false;

        /// <summary>
        /// 制限音量
        /// </summary>
        [ObservableProperty]
        private int limitVolume = 0;

        partial void OnLimitVolumeChanged(int value)
        {
            SavedVolume = false;
        }

        /// <summary>
        /// 出力音量
        /// </summary>
        [ObservableProperty]
        private int outputVolume = 0;

        /// <summary>
        /// 出力音量の変更
        /// </summary>
        [RelayCommand]
        private void VolumeChange(object obj)
        {
            int value = 0;
            int.TryParse(obj.ToString(), out value);
            Console.WriteLine($"VolumeChange: {value}");
            Console.WriteLine($"    Request volume -> {value}.");
            // デバイスが存在しない場合，音量を0にする
            if (currentDevice == null)
            {
                Console.WriteLine($"    Does not exists Device. volume set {value} to 0.");
                value = 0;
            }
            else
            {
                Console.WriteLine($"    Exists Device.");
                // 制限音量を超える場合，音量を制限音量にする
                if (settings.Volume < value)
                {
                    Console.WriteLine($"    Limit volume. volume set {value} to {settings.Volume}.");
                    // 出力音量をLimitVolumeに設定
                    value = settings.Volume;
                }
                currentDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value / 100f;
                Console.WriteLine($"    Set volume -> {value}.");
            }
            OutputVolume = value;
        }

        partial void OnOutputVolumeChanged(int value)
        {
            VolumeChange(value);
        }

        /// <summary>
        /// デバイス名
        /// </summary>
        [ObservableProperty]
        private string? deviceName;

        /// <summary>
        /// 設定値の更新
        /// </summary>
        [RelayCommand]
        void Update()
        {
            // 制限音量が出力音量より大きい場合
            if (LimitVolume < OutputVolume)
            {
                OutputVolume = LimitVolume;
            }
            // 設定値の保存
            settings.Volume = LimitVolume;
            settings.Save();
            SavedVolume = true;
            Console.WriteLine("Settings updated");
        }
    }
}
