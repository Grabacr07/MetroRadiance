using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MetroRadiance.Interop.Win32
{
	public static class Dwmapi
	{
		[DllImport("Dwmapi.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void DwmGetColorizationColor([Out] out uint pcrColorization, [Out, MarshalAs(UnmanagedType.Bool)] out bool pfOpaqueBlend);

		[DllImport("Dwmapi.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void DwmGetWindowAttribute(IntPtr hWnd, DWMWINDOWATTRIBUTE dwAttribute, [Out] out RECT pvAttribute, int cbAttribute);

	}
}
