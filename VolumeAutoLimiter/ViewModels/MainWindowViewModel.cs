using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VolumeAutoLimiter.Models;

namespace VolumeAutoLimiter.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly Window parentWindow;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly Thread thread;

        public MainWindowViewModel(Window window)
        {
            parentWindow = window;
            var settings = Settings.Load();
            limitVolume = settings.Volume;
            parentWindow.Loaded += ParentWindow_Loaded;
            thread = new(new ParameterizedThreadStart(MonitorSystemVolume));
        }

        private void ParentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            thread.Start(cancellationTokenSource.Token);
        }

        ~MainWindowViewModel()
        {
            cancellationTokenSource.Cancel();
            thread.Join();
        }

        /// <summary>
        /// 制限音量
        /// </summary>
        [ObservableProperty]
        private int limitVolume = 0;

        /// <summary>
        /// システム音量
        /// </summary>
        [ObservableProperty]
        private int systemVolume = 0;

        /// <summary>
        /// デバイス名
        /// </summary>
        [ObservableProperty]
        private string deviceName = string.Empty;

        /// <summary>
        /// 設定値の更新
        /// </summary>
        [RelayCommand]
        void Update()
        {
            var settings = Settings.Load();
            settings.Volume = LimitVolume;
            settings.Save();
        }

        private void MonitorSystemVolume(object obj)
        {
            var cancellationToken = (CancellationToken)obj;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // システム音量を取得
                    int currentVolume = GetSystemVolume();

                    // SystemVolumeとLimitVolumeの値が違う場合
                    if (currentVolume != LimitVolume)
                    {
                        // システム音量をLimitVolumeに設定
                        SetSystemVolume(LimitVolume);
                        // SystemVolumeにその値を設定
                        currentVolume = GetSystemVolume();
                    }
                    SystemVolume = currentVolume;
                    DeviceName = GetDeviceName();
                }
                catch (COMException)
                {
                    DeviceName = "None";
                }

                // 一定時間待機
                Thread.Sleep(1000);
            }
        }

        // デバイス名を取得するメソッド
        private string GetDeviceName()
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            return device.FriendlyName;
        }

        // システム音量を取得するメソッド
        private int GetSystemVolume()
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            float currentVolume = device.AudioEndpointVolume.MasterVolumeLevelScalar;

            return (int)(currentVolume * 100);

        }

        // システム音量を設定するメソッド
        private void SetSystemVolume(int volume)
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100f;
        }
    }
}
