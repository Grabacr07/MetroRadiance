
using System;
using System.ComponentModel;

namespace MetroRadiance.Core.Win32
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum ClassLongFlags
	{
		// ReSharper disable InconsistentNaming
		GclpMenuName = -8,
		GclpHBRBackground = -10,
		GclpHCursor = -12,
		GclpHIcon = -14,
		GclpHModule = -16,
		GclCBWndExtra = -18,
		GclCBClsExtra = -20,
		GclpWndProc = -24,
		GclStyle = -26,
		GclpHIconSm = -34,
		GcwAtom = -32
		// ReSharper restore InconsistentNaming
	}
}
