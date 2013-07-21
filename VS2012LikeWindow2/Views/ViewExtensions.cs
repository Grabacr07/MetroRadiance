using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
	}
}
