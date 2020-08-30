using System;
using System.Linq;
using System.Windows;
using MetroRadiance.Interop.Win32;

namespace MetroRadiance.Chrome.Primitives
{
	internal class LeftChromeWindow : ChromeWindow
	{
		static LeftChromeWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(LeftChromeWindow),
				new FrameworkPropertyMetadata(typeof(LeftChromeWindow)));
			TitleProperty.OverrideMetadata(
				typeof(LeftChromeWindow),
				new FrameworkPropertyMetadata(nameof(LeftChromeWindow)));
		}

		private int _topScaledBorderThickness = 0;
		private int _bottomScaledBorderThickness = 0;

		public LeftChromeWindow()
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
			return owner.Left - this.GetWidth(owner);
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

		protected override void OwnerSizeChangedCallback(object sender, EventArgs eventArgs)
		{
			this.UpdateSize();
		}

		protected override void ChangeSettings()
		{
			this.UpdateLocation();
		}
	}
}
