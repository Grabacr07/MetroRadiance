using System;
using System.Linq;
using System.Windows;
using MetroRadiance.Interop.Win32;

namespace MetroRadiance.Chrome.Primitives
{
	internal class RightChromeWindow : ChromeWindow
	{
		static RightChromeWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(RightChromeWindow),
				new FrameworkPropertyMetadata(typeof(RightChromeWindow)));
			TitleProperty.OverrideMetadata(
				typeof(RightChromeWindow),
				new FrameworkPropertyMetadata(nameof(RightChromeWindow)));
		}

		private int _topScaledBorderThickness = 0;
		private int _bottomScaledBorderThickness = 0;

		public RightChromeWindow()
		{
			this.SizeToContent = SizeToContent.Width;
		}

		protected override void UpdateDpiResources()
		{
			this._topScaledBorderThickness = this.BorderThickness.Top.DpiRoundX(this.CurrentDpi);
			this._bottomScaledBorderThickness = this.BorderThickness.Bottom.DpiRoundX(this.CurrentDpi);
		}

		protected override int GetLeft(RECT owner)
		{
			return owner.Right;
		}

		protected override int GetTop(RECT owner)
		{
			return owner.Top - this._topScaledBorderThickness;
		}

		protected override int GetWidth(RECT owner)
		{
			return this.ActualWidth.DpiRoundX(this.SystemDpi);
		}

		protected override int GetHeight(RECT owner)
		{
			return owner.Height + this._topScaledBorderThickness + this._bottomScaledBorderThickness;
		}
	}
}
