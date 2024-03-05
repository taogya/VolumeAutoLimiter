# VolumeAutoLimiter
Windows用の音量自動制限アプリ。<br>
接続したイヤホンなどデバイスによりシステム音量が変わる場合があります。<br>
このアプリは，そのような場合でもあらかじめ決めた数値になるように調整します。

## 使い方
1. 起動<br>
    ビルド済みファイル内の VolumeAutoLimiter.exe をダブルクリックします。
1. 画面について<br>
    ![MainWindow](MainWindow.png)
    1. デバイス名<br>
        現在接続中の出力デバイスが表示されます。<br>
        出力デバイスがない場合は，None が表示されます。
    1. 制限音量<br>
        制限音量を超える音量に変更されると，自動でこの音量に設定します。<br>
        0～100 の整数で設定します。<br>
        設定後は 更新ボタンを押下してください。<br>
        規定値は0です。
    1. システム音量<br>
        現在のシステム音量が表示されます。<br>
    出力デバイスがない場合は，0 が表示されます。

## LICENSE
[LICENSE](LICENSE)<br>

また，本アプリは以下のライブラリを使用しています。
- [CommunityToolkit.Mvvm](VolumeAutoLimiter/Licenses/CommunityToolkit.txt)
- [NAudio](VolumeAutoLimiter/Licenses/NAudio.txt)