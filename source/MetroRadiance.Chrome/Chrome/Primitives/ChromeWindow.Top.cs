using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroRadiance.Win32;

namespace MetroRadiance.Chrome.Primitives
{
	public class TopChromeWindow : ChromeWindow
	{
		protected override int GetLeft(Dpi dpi)
		{
			return this.Owner.Left.DpiRoundX(dpi) - this.Offset.Left.DpiRoundX(dpi);
		}

		protected override int GetTop(Dpi dpi)
		{
			return this.Owner.Top.DpiRoundY(dpi) - this.GetHeight(dpi);
		}

		protected override int GetWidth(Dpi dpi)
		{
			return this.Owner.ActualWidth.DpiRoundX(dpi) + this.Offset.Left.DpiRoundX(dpi) + this.Offset.Right.DpiRoundX(dpi);
		}

		protected override int GetHeight(Dpi dpi)
		{
			return this.GetContentSizeOrDefault(x => x.Height).DpiRoundY(dpi);
		}

		protected override IntPtr? WndProcOverride(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.LBUTTONDBLCLK)
			{
				NativeMethods.SendMessage(this.Owner.Handle, WM.NCLBUTTONDBLCLK, (IntPtr)HitTestValues.HTTOP, IntPtr.Zero);
			}

			return null;
		}
	}
}
