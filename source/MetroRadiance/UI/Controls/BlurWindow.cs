using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
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
			ResizeModeProperty.OverrideMetadata(typeof(BlurWindow), new FrameworkPropertyMetadata(ResizeMode.CanMinimize));
			WindowStyleProperty.OverrideMetadata(typeof(BlurWindow), new FrameworkPropertyMetadata(WindowStyle.None));
			AllowsTransparencyProperty.OverrideMetadata(typeof(BlurWindow), new FrameworkPropertyMetadata(true));
		}

		private HwndSource _source;

		#region ThemeMode 依存関係プロパティ

		public BlurWindowThemeMode ThemeMode
		{
			get { return (BlurWindowThemeMode)this.GetValue(ThemeModeProperty); }
			set { this.SetValue(ThemeModeProperty, value); }
		}
		public static readonly DependencyProperty ThemeModeProperty =
			DependencyProperty.Register("ThemeMode", typeof(BlurWindowThemeMode), typeof(BlurWindow), new UIPropertyMetadata(BlurWindowThemeMode.Default, ThemeModeChangedCallback));

		private static void ThemeModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (BlurWindow)d;
			instance.RemoveThemeCallback((BlurWindowThemeMode)e.OldValue);
			instance.AddThemeCallback((BlurWindowThemeMode)e.NewValue);
			instance.OnThemeModeChanged(e);
			instance.HandleThemeChanged();
		}

		protected virtual void OnThemeModeChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		#endregion

		#region BlurOpacity 依存関係プロパティ

		public double BlurOpacity
		{
			get { return (double)this.GetValue(BlurOpacityProperty); }
			set { this.SetValue(BlurOpacityProperty, value); }
		}
		public static readonly DependencyProperty BlurOpacityProperty =
			DependencyProperty.Register("BlurOpacity", typeof(double), typeof(BlurWindow), new UIPropertyMetadata(0.8, BlurOpacityChangedCallback));

		private static void BlurOpacityChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (BlurWindow)d;
			instance.OnBlurOpacityChanged(e);
			instance.HandleThemeChanged();
		}

		protected virtual void OnBlurOpacityChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		#endregion

		#region DrawBorders 依存関係プロパティ

		public AccentFlags BordersFlag
		{
			get { return (AccentFlags)this.GetValue(BordersFlagProperty); }
			set { this.SetValue(BordersFlagProperty, value); }
		}
		public static readonly DependencyProperty BordersFlagProperty =
			DependencyProperty.Register("BordersFlag", typeof(AccentFlags), typeof(BlurWindow), new UIPropertyMetadata(AccentFlags.DrawAllBorders, BordersFlagChangedCallback));

		private static void BordersFlagChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (BlurWindow)d;
			instance.OnBordersFlagChanged(e);
			instance.HandleThemeChanged();
		}

		protected virtual void OnBordersFlagChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		#endregion

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowsTheme.HighContrast.Changed += this.HandleThemeBooleanChanged;
			WindowsTheme.Transparency.Changed += this.HandleThemeBooleanChanged;
			AddThemeCallback(this.ThemeMode);

			this.HandleThemeChanged();

			this._source = PresentationSource.FromVisual(this) as HwndSource;
			if (this._source == null) return;

			var hWnd = this._source.Handle;
			var wndStyle = User32.GetWindowLong(hWnd);
			User32.SetWindowLong(hWnd, wndStyle & ~WindowStyles.WS_SYSMENU);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			WindowsTheme.HighContrast.Changed -= this.HandleThemeBooleanChanged;
			WindowsTheme.Transparency.Changed -= this.HandleThemeBooleanChanged;
			RemoveThemeCallback(this.ThemeMode);
		}

		private void AddThemeCallback(BlurWindowThemeMode themeMode)
		{
			switch (themeMode)
			{
				case BlurWindowThemeMode.Default:
					WindowsTheme.Theme.Changed += this.HandleThemeValueChanged;
					break;
					
				case BlurWindowThemeMode.Accent:
					WindowsTheme.Accent.Changed += this.HandleThemeColorChanged;
					break;

				case BlurWindowThemeMode.System:
					WindowsTheme.Accent.Changed += this.HandleThemeColorChanged;
					WindowsTheme.ColorPrevalence.Changed += this.HandleThemeBooleanChanged;
					if (WindowsTheme.SystemTheme.IsDynamic)
					{
						WindowsTheme.SystemTheme.Changed += this.HandleThemeValueChanged;
					}
					break;

				default:
					break;
			}
		}

		private void RemoveThemeCallback(BlurWindowThemeMode themeMode)
		{
			switch (themeMode)
			{
				case BlurWindowThemeMode.Default:
					WindowsTheme.Theme.Changed -= this.HandleThemeValueChanged;
					break;
					
				case BlurWindowThemeMode.Accent:
					WindowsTheme.Accent.Changed -= this.HandleThemeColorChanged;
					break;

				case BlurWindowThemeMode.System:
					WindowsTheme.Accent.Changed -= this.HandleThemeColorChanged;
					WindowsTheme.ColorPrevalence.Changed -= this.HandleThemeBooleanChanged;
					if (WindowsTheme.SystemTheme.IsDynamic)
					{
						WindowsTheme.SystemTheme.Changed -= this.HandleThemeValueChanged;
					}
					break;

				default:
					break;
			}
		}

		private void HandleThemeBooleanChanged(object sender, bool value)
			=> this.HandleThemeChanged();

		private void HandleThemeColorChanged(object sender, Color value)
			=> this.HandleThemeChanged();

		private void HandleThemeValueChanged(object sender, Platform.Theme value)
			=> this.HandleThemeChanged();

		internal protected virtual void HandleThemeChanged()
		{
			if (WindowsTheme.HighContrast.Current)
			{
				this.ToHighContrast();
			}
			else if (!WindowsTheme.Transparency.Current)
			{
				this.ToDefault();
			}
			else
			{
				this.ToBlur();
			}
		}

		internal protected void ToHighContrast()
		{
			WindowComposition.Disable(this);
			this.ChangeProperties(
				ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.ApplicationBackground),
				ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemText),
				SystemColors.WindowFrameColor,
				this.GetBordersFlagAsThickness(2));
		}

		internal protected void ToDefault()
		{
			Color background, foreground;
			this.GetColors(out background, out foreground);

			WindowComposition.Disable(this);
			this.ChangeProperties(background, foreground, SystemColors.WindowFrameColor, this.GetBordersFlagAsThickness(1));
		}

		internal protected void ToBlur()
		{
			Color background, foreground;
			this.GetColors(out background, out foreground);

			background.A = (byte)(background.A * this.BlurOpacity);
			WindowComposition.EnableBlur(this, this.BordersFlag);
			this.ChangeProperties(background, foreground, Colors.Transparent, new Thickness());
		}

		internal protected void GetColors(out Color background, out Color foreground)
		{
			var colorPrevalence = WindowsTheme.ColorPrevalence.Current;
			switch (this.ThemeMode)
			{
				case BlurWindowThemeMode.Light:
					background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.LightChromeMedium);
					foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextLightTheme);
					break;

				case BlurWindowThemeMode.Dark:
					background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.DarkChromeMedium);
					foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextDarkTheme);
					break;

				case BlurWindowThemeMode.Accent:
					background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemAccentDark1);
					foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextDarkTheme);
					break;

				case BlurWindowThemeMode.System:
					if (colorPrevalence)
					{
						background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemAccentDark1);
						foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextDarkTheme);
					}
					else if (WindowsTheme.SystemTheme.Current == Platform.Theme.Light)
					{
						background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.LightChromeMedium);
						foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextLightTheme);
					}
					else
					{
						background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.DarkChromeMedium);
						foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextDarkTheme);
					}
					break;

				default:
					if (WindowsTheme.Theme.Current == Platform.Theme.Dark)
					{
						background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.DarkChromeMedium);
						foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextDarkTheme);
					}
					else
					{
						background = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.LightChromeMedium);
						foreground = ImmersiveColor.GetColorByTypeName(ImmersiveColorNames.SystemTextLightTheme);
					}
					break;
			}
		}

		internal protected void ChangeProperties(Color background, Color foreground, Color border, Thickness borderThickness)
		{
			this.Background = new SolidColorBrush(background);
			this.Foreground = new SolidColorBrush(foreground);
			this.BorderBrush = new SolidColorBrush(border);
			this.BorderThickness = borderThickness;
		}

		private Thickness GetBordersFlagAsThickness(double width)
		{
			return new Thickness(
				this.BordersFlag.HasFlag(AccentFlags.DrawLeftBorder) ? width : 0.0,
				this.BordersFlag.HasFlag(AccentFlags.DrawTopBorder) ? width : 0.0,
				this.BordersFlag.HasFlag(AccentFlags.DrawRightBorder) ? width : 0.0,
				this.BordersFlag.HasFlag(AccentFlags.DrawBottomBorder) ? width : 0.0);
		}
	}
}
