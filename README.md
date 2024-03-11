# VolumeAutoLimiter
English is here -> [README.en.md](README.en.md)

Windows用の音量自動制限アプリ。<br>
接続したイヤホンなどデバイスにより出力音量が変わる場合があります。<br>
このアプリは，そのような場合でもあらかじめ決めた数値になるように調整します。<br>
また，デバイスが切り替わると出力音量を自動で0にし，イヤホン抜けなどによるスピーカーからの音漏れ事故も防ぐことができます。

本アプリ起動時は出力音量を強制的に0にします。<br>
本アプリは1秒周期で出力音量を監視しているため，パソコンの負荷状況により制御が遅れる場合があります。<br>
画面を最小化すると通知領域に収容されます。<br>

## 使い方
1. 起動<br>
    ビルド済みファイル内の VolumeAutoLimiter.exe をダブルクリックします。<br>
1. 画面について<br>
    ![MainWindow](MainWindow.gif)
    1. デバイス名<br>
        現在接続中の出力デバイスが表示されます。<br>
        出力デバイスがない場合は，No Device が表示されます。<br>
    2. 制限音量<br>
        制限音量を超える音量に変更されると，自動でこの音量に設定します。<br>
        制限音量はスライダーで調節できます。<br>
        設定後は 更新ボタンを押下するまで反映されません。<br>
        規定値は0です。
    3. 出力音量<br>
        現在の出力音量が表示されます。<br>
        出力音量はスライダーで調節できます。<br>
        最小音量ボタンは出力音量を0にします。<br>
        制限音量ボタンは出力音量を制限音量にします。<br>

    出力デバイスがない場合は，0 が表示されます。<br>

## LICENSE
[LICENSE](LICENSE)<br>

本アプリは以下のライブラリを使用しています。<br>
- [CommunityToolkit.Mvvm](VolumeAutoLimiter/Licenses/CommunityToolkit.txt)<br>
- [NAudio](VolumeAutoLimiter/Licenses/NAudio.txt)<br>

アイコンは以下を使用しています。<br>
- [アプリアイコン](https://icon-icons.com/ja/%E3%82%A2%E3%82%A4%E3%82%B3%E3%83%B3/%E3%82%B9%E3%83%94%E3%83%BC%E3%82%AB%E3%83%BC-%E3%82%AA%E3%83%BC%E3%83%87%E3%82%A3%E3%82%AA-%E9%9F%B3-%E5%A3%B0/148818)

## 動作環境
Windows 10/11 (x86/x64)<br>