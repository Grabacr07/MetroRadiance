using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MetroRadiance.Interop.Win32
{
	public static class UxTheme
	{
		[DllImport("uxtheme.dll", EntryPoint = "#94")]
		internal static extern int GetImmersiveColorSetCount();

		[DllImport("uxtheme.dll", EntryPoint = "#95")]
		internal static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);

		[DllImport("uxtheme.dll", EntryPoint = "#96", CharSet = CharSet.Unicode)]
		internal static extern uint GetImmersiveColorTypeFromName(string name);

		[DllImport("uxtheme.dll", EntryPoint = "#98")]
		internal static extern uint GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

		[DllImport("uxtheme.dll", EntryPoint = "#100", CharSet = CharSet.Unicode)]
		internal static extern IntPtr GetImmersiveColorNamedTypeByIndex(uint dwIndex);
	}
}
