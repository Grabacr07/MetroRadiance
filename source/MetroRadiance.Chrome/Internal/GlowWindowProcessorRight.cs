using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetroRadiance.Win32;

namespace MetroRadiance.Chrome.Internal
{
	internal class GlowWindowProcessorRight : GlowWindowProcessor
	{
		public override Orientation Orientation => Orientation.Vertical;

		public override HorizontalAlignment HorizontalAlignment => HorizontalAlignment.Left;

		public override VerticalAlignment VerticalAlignment => VerticalAlignment.Stretch;

		public override int GetLeft(int ownerLeft, int ownerWidth, Size glowSize)
		{
			return ownerLeft + ownerWidth;
		}

		public override int GetTop(int ownerTop, int ownerHeight, Size glowSize)
		{
			return ownerTop - glowSize.Height;
		}

		public override int GetWidth(int ownerLeft, int ownerWidth, Size glowSize)
		{
			return glowSize.Width;
		}

		public override int GetHeight(int ownerTop, int ownerHeight, Size glowSize)
		{
			return ownerHeight + glowSize.Height * 2;
		}

		public override HitTestValues GetHitTestValue(Point point, double actualWidht, double actualHeight)
		{
			var rightTop = new Rect(0, 0, actualWidht, EdgeSize);
			var rightBottom = new Rect(0, actualHeight - EdgeSize, actualWidht, EdgeSize);

			return rightTop.Contains(point)
				? HitTestValues.HTTOPRIGHT
				: rightBottom.Contains(point) ? HitTestValues.HTBOTTOMRIGHT : HitTestValues.HTRIGHT;
		}

		public override Cursor GetCursor(Point point, double actualWidht, double actualHeight)
		{
			var rightTop = new Rect(0, 0, actualWidht, EdgeSize);
			var rightBottom = new Rect(0, actualHeight - EdgeSize, actualWidht, EdgeSize);

			return rightTop.Contains(point)
				? Cursors.SizeNESW
				: rightBottom.Contains(point) ? Cursors.SizeNWSE : Cursors.SizeWE;
		}
	}
}
