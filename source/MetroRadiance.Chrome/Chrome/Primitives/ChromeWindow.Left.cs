using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetroRadiance.Chrome.Primitives
{
	public class LeftChromeWindow : ChromeWindow
	{
		protected override int GetLeft(Dpi dpi)
		{
			return this.Owner.Left.DpiRoundX(dpi) - this.GetWidth(dpi);
		}

		protected override int GetTop(Dpi dpi)
		{
			return this.Owner.Top.DpiRoundY(dpi);
		}

		protected override int GetWidth(Dpi dpi)
		{
			return this.GetContentSizeOrDefault(x => x.Width).DpiRoundX(dpi);
		}

		protected override int GetHeight(Dpi dpi)
		{
			return this.Owner.ActualHeight.DpiRoundY(dpi);
		}
	}
}
