using System;

namespace MetroRadiance.Core.Win32
{
	[Flags]
	// ReSharper disable InconsistentNaming
	public enum SWP
	{
		NOSIZE = 0x1,
		NOMOVE = 0x2,
		NOZORDER = 0x4,
		NOREDRAW = 0x8,
		NOACTIVATE = 0x10,
		FRAMECHANGED = 0x20,
		SHOWWINDOW = 0x0040,
		NOOWNERZORDER = 0x200,
		NOSENDCHANGING = 0x0400,
		ASYNCWINDOWPOS = 0x4000,
	}
	// ReSharper restore InconsistentNaming
}
