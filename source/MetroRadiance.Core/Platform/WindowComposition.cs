using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Interop.Win32;

namespace MetroRadiance.Platform
{
	public static class WindowComposition
	{
		public static void Set(Window window, AccentState accentState, AccentFlags accentFlags)
		{
			var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
			if (hwndSource == null) return;

			var accent = new AccentPolicy
			{
				AccentState = accentState,
				AccentFlags = accentFlags,
			};
			var accentStructSize = Marshal.SizeOf(accent);
			var accentPtr = Marshal.AllocHGlobal(accentStructSize);
			Marshal.StructureToPtr(accent, accentPtr, false);

			var data = new WindowCompositionAttributeData
			{
				Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
				SizeOfData = accentStructSize,
				Data = accentPtr,
			};
			User32.SetWindowCompositionAttribute(hwndSource.Handle, ref data);

			Marshal.FreeHGlobal(accentPtr);
		}
	}
}
