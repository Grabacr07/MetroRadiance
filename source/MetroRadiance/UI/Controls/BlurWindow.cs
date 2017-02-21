using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using MetroRadiance.Platform;

namespace MetroRadiance.UI.Controls
{
	public class BlurWindow : Window
	{
		static BlurWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurWindow), new FrameworkPropertyMetadata(typeof(BlurWindow)));
			WindowStyleProperty.OverrideMetadata(typeof(BlurWindow), new FrameworkPropertyMetadata(WindowStyle.None));
			AllowsTransparencyProperty.OverrideMetadata(typeof(BlurWindow), new FrameworkPropertyMetadata(true));
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowComposition.Set(this, AccentState.ACCENT_ENABLE_BLURBEHIND, AccentFlags.DrawAllBorders);

			WindowsTheme.HighContrast.Changed += this.HandleThemeChanged;
			WindowsTheme.Transparency.Changed += this.HandleThemeChanged;
			WindowsTheme.ColorPrevalence.Changed += this.HandleThemeChanged;

			this.HandleThemeChanged(null, false);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowsTheme.HighContrast.Changed -= this.HandleThemeChanged;
			WindowsTheme.Transparency.Changed -= this.HandleThemeChanged;
			WindowsTheme.ColorPrevalence.Changed -= this.HandleThemeChanged;
		}

		private void HandleThemeChanged(object sender, bool value)
		{
			if (WindowsTheme.HighContrast.Current)
			{
				this.ToHighContrast();
			}
			else
			{
				this.ToBlur(
					WindowsTheme.Transparency.Current,
					WindowsTheme.ColorPrevalence.Current);
			}
		}

		private void ToHighContrast()
		{
			this.ChangeProperties(
				ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.ApplicationBackground),
				ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemText),
				SystemColors.WindowFrameColor,
				new Thickness(2));
		}

		private void ToBlur(bool transparency, bool colorPrevalence)
		{
			var background = colorPrevalence
				? ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemAccentDark1)
				: ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.DarkChromeMedium);
			if (transparency) background.A = (byte)(background.A * 0.8);

			var foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextDarkTheme);

			this.ChangeProperties(background, foreground, Colors.Transparent, new Thickness());
		}

		private void ChangeProperties(Color background, Color foreground, Color border, Thickness borderThickness)
		{
			this.Background = new SolidColorBrush(background);
			this.Foreground = new SolidColorBrush(foreground);
			this.BorderBrush = new SolidColorBrush(border);
			this.BorderThickness = borderThickness;
		}
	}
}
