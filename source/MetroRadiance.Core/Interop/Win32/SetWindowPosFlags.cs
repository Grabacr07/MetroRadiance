using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace MetroRadiance.Interop.Win32
{
	[Flags]
	public enum SetWindowPosFlags
	{
		SWP_NOSIZE = 0x1,
		SWP_NOMOVE = 0x2,
		SWP_NOZORDER = 0x4,
		SWP_NOREDRAW = 0x8,
		SWP_NOACTIVATE = 0x10,
		SWP_FRAMECHANGED = 0x20,
		SWP_SHOWWINDOW = 0x0040,
		SWP_NOOWNERZORDER = 0x200,
		SWP_NOSENDCHANGING = 0x0400,
		SWP_ASYNCWINDOWPOS = 0x4000,
	}
}
