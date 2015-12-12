using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MetroRadiance.Interop.Win32
{
	public static class Dwmapi
	{
		[DllImport("Dwmapi.dll")]
		public static extern void DwmGetColorizationColor([Out] out int pcrColorization, [Out] out bool pfOpaqueBlend);

		[DllImport("Dwmapi.dll")]
		public static extern void DwmGetWindowAttribute(IntPtr hWnd, DWMWINDOWATTRIBUTE dwAttribute, [Out] out RECT pvAttribute, int cbAttribute);

	}
}
