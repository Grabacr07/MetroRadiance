using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using MetroRadiance.Interop.Win32;
using MetroRadiance.Media;
using Microsoft.Win32;

namespace MetroRadiance.Platform
{
	public enum Theme
	{
		Dark = 0,
		Light = 1,
	}

	public class ThemeValue : WindowsThemeValue<Theme>
	{
		internal override Theme GetValue()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "AppsUseLightTheme";

			return Registry.GetValue(keyName, valueName, null) as int? == 0 ? Theme.Dark : Theme.Light;
		}

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WindowsMessages.WM_SETTINGCHANGE)
			{
				var systemParmeter = Marshal.PtrToStringAuto(lParam);
				if (systemParmeter == "ImmersiveColorSet")
				{
					this.Update(this.GetValue());
					handled = true;
				}
			}

			return IntPtr.Zero;
		}
	}

	public class SystemThemeValue : ThemeValue
	{
		internal override Theme GetValue()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "SystemUsesLightTheme";

			return (Registry.GetValue(keyName, valueName, null) as int? ?? (int)base.GetValue()) == 0 ? Theme.Dark : Theme.Light;
		}
	}

	public class AccentValue : WindowsThemeValue<Color>
	{
		internal override Color GetValue()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\DWM";
			const string valueName = "ColorizationColor";
			int color;

			var colorizationColor = Registry.GetValue(keyName, valueName, null) as int?;
			if (colorizationColor != null)
			{
				color = colorizationColor.Value;
			}
			else
			{
				bool opaque;
				Dwmapi.DwmGetColorizationColor(out color, out opaque);
			}

			return ColorHelper.GetColorFromInt64(color);
		}

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WindowsMessages.WM_DWMCOLORIZATIONCOLORCHANGED)
			{
				var color = ColorHelper.GetColorFromInt64((long)wParam);
				this.Update(color);
				handled = true;
			}

			return IntPtr.Zero;
		}
	}


	public sealed class HighContrastValue : WindowsThemeValue<bool>
	{
		internal override bool GetValue() => System.Windows.SystemParameters.HighContrast;

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WindowsMessages.WM_THEMECHANGED)
			{
				this.Update(this.GetValue());
				handled = true;
			}

			return IntPtr.Zero;
		}
	}

	public sealed class ColorPrevalenceValue : WindowsThemeValue<bool>
	{
		internal override bool GetValue()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "ColorPrevalence";

			return Registry.GetValue(keyName, valueName, null) as int? != 0;
		}

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WindowsMessages.WM_SETTINGCHANGE)
			{
				var systemParmeter = Marshal.PtrToStringAuto(lParam);
				if (systemParmeter == "ImmersiveColorSet")
				{
					this.Update(this.GetValue());
					handled = true;
				}
			}

			return IntPtr.Zero;
		}
	}

	public sealed class TransparencyValue : WindowsThemeValue<bool>
	{
		internal override bool GetValue()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "EnableTransparency";

			return Registry.GetValue(keyName, valueName, null) as int? != 0;
		}

		internal override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WindowsMessages.WM_SETTINGCHANGE)
			{
				var systemParmeter = Marshal.PtrToStringAuto(lParam);
				if (systemParmeter == "ImmersiveColorSet")
				{
					this.Update(this.GetValue());
					handled = true;
				}
			}

			return IntPtr.Zero;
		}
	}
}
