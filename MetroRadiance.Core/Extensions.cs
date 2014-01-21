using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MetroRadiance.Core.Win32;

namespace MetroRadiance.Core
{
	public static class Extensions
	{
		/// <summary>
		/// <see cref="Point"/> 構造体どうしを乗算します。
		/// </summary>
		/// <param name="multiplicand">被乗数。</param>
		/// <param name="multiplier">乗数。</param>
		/// <returns>乗算結果。</returns>
		public static Point Multiplication(this Point multiplicand, Point multiplier)
		{
			return new Point(multiplicand.X * multiplier.X, multiplicand.Y * multiplier.Y);
		}

		/// <summary>
		/// <see cref="Point"/> 構造体どうしを除算します。
		/// </summary>
		/// <param name="dividend">被除数。</param>
		/// <param name="divisor">除数。</param>
		/// <returns>除算結果。</returns>
		public static Point Division(this Point dividend, Point divisor)
		{
			return new Point(dividend.X / divisor.X, dividend.Y / divisor.Y);
		}

		/// <summary>
		/// 現在の <see cref="Visual"/> から、WPF が認識しているシステム DPI を取得します。
		/// </summary>
		/// <returns>
		/// X 軸 および Y 軸それぞれの DPI 設定値を表す <see cref="Dpi"/> 構造体。
		/// </returns>
		public static Dpi GetSystemDpi(this Visual visual)
		{
			var source = PresentationSource.FromVisual(visual);
			if (source != null && source.CompositionTarget != null)
			{
				return new Dpi(
					(uint)(Dpi.Default.X * source.CompositionTarget.TransformToDevice.M11),
					(uint)(Dpi.Default.Y * source.CompositionTarget.TransformToDevice.M22));
			}

			return Dpi.Default;
		}
	}
}
