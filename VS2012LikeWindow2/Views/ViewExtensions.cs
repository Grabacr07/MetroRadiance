using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VS2012LikeWindow2.Views.Chrome;

namespace VS2012LikeWindow2.Views
{
	internal static class ViewExtensions
	{
		/// <summary>
		/// <see cref="T:System.Windows.Point"/> 構造体どうしを乗算します。
		/// </summary>
		/// <param name="multiplicand">被乗数。</param>
		/// <param name="multiplier">乗数。</param>
		/// <returns>乗算結果。</returns>
		public static Point Multiplication(this Point multiplicand, Point multiplier)
		{
			return new Point(multiplicand.X * multiplier.X, multiplicand.Y * multiplier.Y);
		}

		/// <summary>
		/// <see cref="T:System.Windows.Point"/> 構造体どうしを除算します。
		/// </summary>
		/// <param name="dividend">被除数。</param>
		/// <param name="divisor">除数。</param>
		/// <returns>除算結果。</returns>
		public static Point Division(this Point dividend, Point divisor)
		{
			return new Point(dividend.X / divisor.X, dividend.Y / divisor.Y);
		}

		/// <summary>
		/// 現在の <see cref="T:System.Windows.Media.Visual"/> から、DPI 倍率を取得します。
		/// </summary>
		/// <returns>
		/// X/Y それぞれの DPI 倍率を表す <see cref="T:System.Windows.Point"/>
		/// 構造体。取得に失敗した場合、(1.0, 1.0) を返します。
		/// </returns>
		public static Point GetDpiScaleFactor(this Visual visual)
		{
			try
			{
				var source = PresentationSource.FromVisual(visual);
				if (source != null && source.CompositionTarget != null)
				{
					return new Point(
						source.CompositionTarget.TransformToDevice.M11,
						source.CompositionTarget.TransformToDevice.M22);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return new Point(1.0, 1.0);
		}

		/// <summary>
		/// ウィンドウ プロシージャのマウス関連メッセージの lParam パラメーターからスクリーン座標へ変換します。
		/// </summary>
		public static Point ToPoint(this IntPtr lParam)
		{
			return new Point((short)((int)lParam & 0xFFFF), (short)(((int)lParam >> 16) & 0xFFFF));
		}


		/// <summary>
		/// ウィンドウ操作を実行します。
		/// </summary>
		/// <param name="action">実行するウィンドウ操作。</param>
		/// <param name="source">操作を実行しようとしている UI 要素。この要素をホストするウィンドウに対し、<paramref name="action"/> 操作が実行されます。</param>
		public static void Invoke(this WindowAction action, FrameworkElement source)
		{
			var window = Window.GetWindow(source);
			if (window == null) return;

			switch (action)
			{
				case WindowAction.Active:
					window.Activate();
					break;
				case WindowAction.Close:
					window.Close();
					break;
				case WindowAction.Maximize:
					window.WindowState = WindowState.Maximized;
					break;
				case WindowAction.Minimize:
					window.WindowState = WindowState.Minimized;
					break;
				case WindowAction.Normalize:
					window.WindowState = WindowState.Normal;
					break;
				case WindowAction.OpenSystemMenu:
					var point = source.PointToScreen(new Point(0, source.ActualHeight));
					SystemCommands.ShowSystemMenu(window, point);
					break;
			}
		}
	}
}
