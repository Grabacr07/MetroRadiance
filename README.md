# MetroRadiance

[![Build status](https://img.shields.io/appveyor/ci/Grabacr07/MetroRadiance/master.svg?style=flat-square)](https://ci.appveyor.com/project/Grabacr07/MetroRadiance)
[![NuGet](https://img.shields.io/nuget/v/MetroRadiance.Core.svg?style=flat-square)](https://www.nuget.org/packages/MetroRadiance.Core/)
[![Downloads](https://img.shields.io/nuget/dt/MetroRadiance.Core.svg?style=flat-square)](https://www.nuget.org/packages/MetroRadiance.Core/)
[![License](https://img.shields.io/github/license/Grabacr07/MetroRadiance.svg?style=flat-square)](https://github.com/Grabacr07/MetroRadiance/blob/master/LICENSE.txt)

Visual Studio 2012 ～ 2015 のようなウィンドウを作るための WPF 向け UI コントロール ライブラリ。

![ss150730085651kd](https://cloud.githubusercontent.com/assets/1779073/8972861/0e3eed28-3699-11e5-9bfe-18af42a6ed73.png)

### MetroRadiance.Core

MetroRadiance コア ライブラリ。
* Per-Monitor DPI 計算
* Win32 API ラッパー


### MetroRadiance.Chrome

Window 向け Chrome ライブラリ。
* MetroChromeBehavior (任意の Window に Visual Studio のような光る枠を付与する Behavior)

```XAML
<Window>
    <controls:MetroWindow.MetroChromeBehavior>
        <chrome:MetroChromeBehavior ActiveBrush="{DynamicResource AccentBrushKey}"
                                    nactiveBrush="{DynamicResource BorderBrushKey}"
                                    Mode="Office2013" />
    </controls:MetroWindow.MetroChromeBehavior>
</Window>
```

### MetroRadiance

カスタム コントロール ライブラリ。
* カスタム コントロール
* カスタム ビヘイビア
* カスタム コンバーター
* スタイル切り替え

(書きかけ...)
