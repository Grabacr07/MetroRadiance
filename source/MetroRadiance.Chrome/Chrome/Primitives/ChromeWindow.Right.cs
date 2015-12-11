using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MetroRadiance.Chrome.Primitives
{
	internal class RightChromeWindow : ChromeWindow
	{
		public RightChromeWindow()
		{
			this.SizeToContent = SizeToContent.Width;
		}

		protected override int GetLeft(Dpi dpi)
		{
			return this.Owner.Left.DpiRoundX(dpi) + this.Owner.ActualWidth.DpiRoundX(dpi);
		}

		protected override int GetTop(Dpi dpi)
		{
			return this.Owner.Top.DpiRoundY(dpi);
		}

		protected override int GetWidth(Dpi dpi)
		{
			return this.GetContentSizeOrDefault(x => x.ActualWidth).DpiRoundX(dpi);
		}

		protected override int GetHeight(Dpi dpi)
		{
			return this.Owner.ActualHeight.DpiRoundY(dpi);
		}
	}
}
