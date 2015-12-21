## 概要

MetroRadiance version 2.0 で名前空間等の整理やリネームを行ったため、version 1.x との互換性がなくなった。
このため、1.x を参照して作られたアセンブリは、2.0 環境で動作させるためには修正および再ビルドをしなければならなくなった。

通常の MetroRadiance 依存アプリでは、MetroRadiance 本体をアップデートし、適宜修正することで対応可能。
一方で、MetroRadiance 2.0 依存となったアプリケーション上で、MetroRadiance 1.x 依存のアセンブリを動作させることができなくなった。

(プラグイン システムでこの状況が発生しうる。プラグイン システムを搭載した本体が MetroRadiance 2.0、プラグイン側が 1.x、という状況。
この場合、プラグイン側も MetroRadiance 2.0 にアップデートして再ビルドしなければならない)

そこで、1.x 依存のアセンブリを 2.0 環境で動作させるため、RetroRadiance を用意した。


## 実装

[MetroRadiance version 1.2](https://github.com/Grabacr07/MetroRadiance/tree/8aebe400e7e3c29df66ed4b5339dd74fd74515dd) 時点での MetroRadiance、MetroRadiance.Chrome、MetroRadiance.Core の内容がこのアセンブリに含まれる。
Style および Themes 以下のリソースもすべて含まれれている。

ただし、すべての型に対し、以下の属性を付与している。

```csharp
[Obsolete]
[EditorBrowsable(EditorBrowsableState.Never)]
```


## ビルド

ソリューション構成「Release RETRO」を選択すると、MetroRadiance プロジェクトはビルド構成「Release RETRO」となり、RetroRadiance への参照と TypeForwardedTo 属性が有効になる。
この場合のみ、MetroRadiance 1.x 依存プロジェクトは、2.0 を参照したとき、1.x 準拠の型を持つ RetroRadiance に型フォワーディングされ、正常動作できる。

それ以外のソリューション構成、ビルド構成では RetroRadiance への参照と TypeForwardedTo 属性が無効化され、1.x 依存プロジェクトは動作しない。

