
# VolumeAutoLimiter
日本語はここ -> [README.md](README.md)

An automatic volume limiter application for Windows.<br>
The output volume may vary depending on the connected device, such as headphones.<br>
This application adjusts the volume to a predetermined value even in such cases.<br>
It also automatically sets the output volume to 0 when the device is switched, preventing accidents such as sound leakage from speakers due to headphone disconnection.

When this application is launched, it forcibly sets the output volume to 0.<br>
Since this application monitors the output volume at a 1-second interval, the control may be delayed depending on the computer's load.<br>
When minimized, it will be displayed in the notification area.<br>

## Usage
1. Launch<br>
    Double-click the VolumeAutoLimiter.exe file in the built files.<br>
1. About the screen<br>
    ![MainWindow](MainWindow.en.png)
    1. Device Name<br>
        Displays the currently connected output device.<br>
        If there is no output device, "No Device" will be displayed.<br>
    2. Volume Limit<br>
        Automatically sets the volume to this value when it exceeds the volume limit.<br>
        The volume limit can be adjusted with a slider.<br>
        The changes will not be applied until the Update button is pressed.<br>
        The default value is 0.
    3. Output Volume<br>
        Displays the current output volume.<br>
        The output volume can be adjusted with a slider.<br>
        The Minimum Volume button sets the output volume to 0.<br>
        The Limit Volume button sets the output volume to the volume limit.<br>

    If there is no output device, 0 will be displayed.<br>

## LICENSE
[LICENSE](LICENSE)<br>

This application uses the following libraries.<br>
- [CommunityToolkit.Mvvm](VolumeAutoLimiter/Licenses/CommunityToolkit.txt)<br>
- [NAudio](VolumeAutoLimiter/Licenses/NAudio.txt)<br>

The icon is used from the following source.<br>
- [App Icon](https://icon-icons.com/ja/%E3%82%A2%E3%82%A4%E3%82%B3%E3%83%B3/%E3%82%B9%E3%83%94%E3%83%BC%E3%82%AB%E3%83%BC-%E3%82%AA%E3%83%BC%E3%83%87%E3%82%A3%E3%82%AA-%E9%9F%B3-%E5%A3%B0/148818)

## System Requirements
Windows 10/11 (x86/x64)<br>