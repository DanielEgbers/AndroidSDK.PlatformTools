# AndroidSDK.PlatformTools
Enables the use of [Android SDK Platform Tools](https://developer.android.com/studio/releases/platform-tools) as [.NET Core Global Tools](https://aka.ms/global-tools)

#### Advantages
- Installed on your machine in a default location that is included in the PATH environment variable or in a custom location
- NO Android SDK required
- NO Java JRE/SDK required

#### Prerequisites
- Windows 8.1 or later versions
- [.NET Core 2.1 Runtime/SDK or later versions](https://dotnet.microsoft.com/download)

## Deployment

### ADB (Android Debug Bridge)

[![AndroidSDK.PlatformTools.ADB](https://img.shields.io/nuget/v/AndroidSDK.PlatformTools.ADB.svg?style=flat-square)](https://www.nuget.org/packages/AndroidSDK.PlatformTools.ADB)

#### Install
```
dotnet tool install --global AndroidSDK.PlatformTools.ADB
```

#### Update / Reinstall
```
dotnet tool update --global AndroidSDK.PlatformTools.ADB
```

#### Uninstall
```
dotnet tool uninstall --global AndroidSDK.PlatformTools.ADB
```

### Fastboot

[![AndroidSDK.PlatformTools.Fastboot](https://img.shields.io/nuget/v/AndroidSDK.PlatformTools.Fastboot.svg?style=flat-square)](https://www.nuget.org/packages/AndroidSDK.PlatformTools.Fastboot)

#### Install
```
dotnet tool install --global AndroidSDK.PlatformTools.Fastboot
```

#### Update / Reinstall
```
dotnet tool update --global AndroidSDK.PlatformTools.Fastboot
```

#### Uninstall
```
dotnet tool uninstall --global AndroidSDK.PlatformTools.Fastboot
```