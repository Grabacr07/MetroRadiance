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
	internal class GlowWindowProcessorTop : GlowWindowProcessor
	{
		public override Orientation Orientation => Orientation.Horizontal;

		public override HorizontalAlignment HorizontalAlignment => HorizontalAlignment.Stretch;

		public override VerticalAlignment VerticalAlignment => VerticalAlignment.Bottom;

		public override int GetLeft(int ownerLeft, int ownerWidth, Size glowSize)
		{
			return ownerLeft;
		}

		public override int GetTop(int ownerTop, int ownerHeight, Size glowSize)
		{
			return ownerTop - glowSize.Height;
		}

		public override int GetWidth(int ownerLeft, int ownerWidth, Size glowSize)
		{
			return ownerWidth;
		}

		public override int GetHeight(int ownerTop, int ownerHeight, Size glowSize)
		{
			return glowSize.Height;
		}

		public override HitTestValues GetHitTestValue(Point point, double actualWidht, double actualHeight)
		{
			var topLeft = new Rect(0, 0, EdgeSize - GlowSize, actualHeight);
			var topRight = new Rect(actualWidht - EdgeSize + GlowSize, 0, EdgeSize - GlowSize, actualHeight);

			return topLeft.Contains(point)
				? HitTestValues.HTTOPLEFT
				: topRight.Contains(point) ? HitTestValues.HTTOPRIGHT : HitTestValues.HTTOP;
		}

		public override Cursor GetCursor(Point point, double actualWidht, double actualHeight)
		{
			var topLeft = new Rect(0, 0, EdgeSize - GlowSize, actualHeight);
			var topRight = new Rect(actualWidht - EdgeSize + GlowSize, 0, EdgeSize - GlowSize, actualHeight);

			return topLeft.Contains(point)
				? Cursors.SizeNWSE
				: topRight.Contains(point) ? Cursors.SizeNESW : Cursors.SizeNS;
		}
	}
}
