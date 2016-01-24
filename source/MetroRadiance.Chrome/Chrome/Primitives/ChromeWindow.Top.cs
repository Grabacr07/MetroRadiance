using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;

namespace MetroRadiance.Chrome.Primitives
{
	internal class TopChromeWindow : ChromeWindow
	{
		public TopChromeWindow()
		{
			this.SizeToContent = SizeToContent.Height;
		}

		protected override int GetLeft(Dpi dpi)
		{
			return this.Owner.Left.DpiRoundX(dpi) - this.Offset.Left.DpiRoundX(this.CurrentDpi);
		}

		protected override int GetTop(Dpi dpi)
		{
			return this.Owner.Top.DpiRoundY(dpi) - this.GetHeight(this.CurrentDpi);
		}

		protected override int GetWidth(Dpi dpi)
		{
			return this.Owner.ActualWidth.DpiRoundX(dpi) + this.Offset.Left.DpiRoundX(this.CurrentDpi) + this.Offset.Right.DpiRoundX(this.CurrentDpi);
		}

		protected override int GetHeight(Dpi dpi)
		{
			return this.GetContentSizeOrDefault(x => x.ActualHeight).DpiRoundY(dpi);
		}

		protected override IntPtr? WndProcOverride(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WindowsMessages.WM_LBUTTONDBLCLK)
			{
				User32.SendMessage(this.Owner.Handle, WindowsMessages.WM_NCLBUTTONDBLCLK, (IntPtr)HitTestValues.HTTOP, IntPtr.Zero);
			}

			return null;
		}
	}
}
