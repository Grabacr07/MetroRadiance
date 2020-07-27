using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetroRadiance.Platform
{
	/// <summary>
	/// Windows OS のテーマ機能へアクセスできるようにします。
	/// </summary>
	public static class WindowsTheme
	{
		/// <summary>
		/// Windows の既定のアプリテーマ設定と、その変更通知機能へアクセスできるようにします。
		/// </summary>
		public static ThemeValue Theme { get; } = new ThemeValue();

		/// <summary>
		/// Windows の既定のシステムテーマ設定と、その変更通知機能へアクセスできるようにします。
		/// </summary>
		public static SystemThemeValue SystemTheme { get; } = new SystemThemeValue();

		/// <summary>
		/// Windows のアクセント カラー設定と、その変更通知機能へアクセスできるようにします。
		/// </summary>
		public static AccentValue Accent { get; } = new AccentValue();

		public static HighContrastValue HighContrast { get; } = new HighContrastValue();

		public static ColorPrevalenceValue ColorPrevalence { get; } = new ColorPrevalenceValue();

		public static TransparencyValue Transparency { get; } = new TransparencyValue();
		
		/// <summary>
		/// Windows の文字の大きさ設定と、その変更通知機能へアクセスできるようにします。
		/// </summary>
		public static TextScaleFactorValue TextScaleFactor { get; } = new TextScaleFactorValue();
	}
}
