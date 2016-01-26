# MetroRadiance

[![Build status](https://img.shields.io/appveyor/ci/Grabacr07/MetroRadiance/master.svg?style=flat-square)](https://ci.appveyor.com/project/Grabacr07/MetroRadiance)
[![NuGet](https://img.shields.io/nuget/v/MetroRadiance.Core.svg?style=flat-square)](https://www.nuget.org/packages/MetroRadiance.Core/)
[![Downloads](https://img.shields.io/nuget/dt/MetroRadiance.Core.svg?style=flat-square)](https://www.nuget.org/packages/MetroRadiance.Core/)
[![License](https://img.shields.io/github/license/Grabacr07/MetroRadiance.svg?style=flat-square)](https://github.com/Grabacr07/MetroRadiance/blob/master/LICENSE.txt)

Visual Studio 2012 ～ 2015 のようなウィンドウを作るための WPF 向け UI コントロール ライブラリ。

![ss150730085651kd](https://cloud.githubusercontent.com/assets/1779073/8972861/0e3eed28-3699-11e5-9bfe-18af42a6ed73.png)

### MetroRadiance.Core

MetroRadiance コア ライブラリ。

* Win32 API ラッパー
* Per-Monitor DPI 計算
* Windows テーマ機能
  - アクセント カラーの取得
  - アクセント カラー変更イベント
  - Light/Dark テーマ判定 (Windows 10)

```csharp
using MetroRadiance.Platform;
```

```csharp
// Subscribe accent color change event from Windows theme.
var disposable = WindowsTheme.RegisterAccentColorListener(color =>
{
    // apply color to your app.
});

// Unsubscribe color change event.
disposable.Dispose();
```

* HSV 色空間サポート

```csharp
using MetroRadiance.Media;
```

```csharp
// Get Windows accent color (using MetroRadiance.Platform;)
var rgbColor = WindowsTheme.GetAccentColor();

// Convert from RGB to HSV color.
var hsvColor = rgbColor.ToHsv();
hsvColor.V *= 0.8;

// Convert from HSV to RGB color.
var newColor = hsvColor.ToRgb();
```

### MetroRadiance.Chrome

Window 向け Chrome ライブラリ。

* 任意の Window に Visual Studio のような光る枠を付与する
  - MetroRadiance.Chrome.WindowChrome

```XAML
<Window xmlns:chrome="http://schemes.grabacr.net/winfx/2014/chrome">
    <chrome:WindowChrome.Instance>
        <chrome:WindowChrome />
    </chrome:WindowChrome.Instance>
</Window>
```

### MetroRadiance

カスタム コントロール ライブラリ。
* カスタム コントロール
* カスタム ビヘイビア
* カスタム コンバーター
* スタイル切り替え

(書きかけ...)
