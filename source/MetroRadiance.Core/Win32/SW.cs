
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetroRadiance.Win32
{
	// ReSharper disable InconsistentNaming
	public enum SW
	{
		/// <summary>
		/// ウィンドウを非表示にして、ほかのウィンドウをアクティブにします。
		/// </summary>
		HIDE = 0,

		/// <summary>
		/// ウィンドウをアクティブにし、表示します。ウィンドウが最小化、または最大化されていると、ウィンドウは元の位置とサイズで復元されます。
		/// </summary>
		SHOWNORMAL = 1,

		/// <summary>
		/// ウィンドウをアクティブにし、アイコンとして表示します。
		/// </summary>
		SHOWMINIMIZED = 2,

		/// <summary>
		/// ウィンドウをアクティブにし、最大化します。
		/// </summary>
		SHOWMAXIMIZED = 3,

		/// <summary>
		/// ウィンドウをアイコンとして表示します。 現在アクティブなウィンドウはアクティブなまま表示します。
		/// </summary>
		SHOWNOACTIVATE = 4,

		/// <summary>
		/// ウィンドウをアクティブにし、現在のサイズと位置で表示します。
		/// </summary>
		SHOW = 5,

		/// <summary>
		/// 指定されたウィンドウを最小化して、システム リストにあるトップレベル ウィンドウをアクティブにします。
		/// </summary>
		MINIMIZE = 6,

		/// <summary>
		/// ウィンドウを直前のサイズと位置で表示します。 現在アクティブなウィンドウはアクティブなまま表示します。
		/// </summary>
		SHOWMINNOACTIVE = 7,

		/// <summary>
		/// ウィンドウを現在の状態で表示します。 現在アクティブなウィンドウはアクティブなまま表示します。
		/// </summary>
		SHOWNA = 8,

		/// <summary>
		/// ウィンドウをアクティブにし、表示します。 ウィンドウが最小化、または最大化されていると、ウィンドウは元の位置とサイズに復元されます。
		/// </summary>
		RESTORE = 9,

		/// <summary>
		/// アプリケーションを起動したプログラムが 関数に渡した 構造体で指定された SW_ フラグに従って表示状態を設定します。
		/// </summary>
		SHOWDEFAULT = 10,
	}

	// ReSharper restore InconsistentNaming
}
