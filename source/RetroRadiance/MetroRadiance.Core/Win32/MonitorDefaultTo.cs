using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MetroRadiance.Core.Win32
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum MonitorDefaultTo
	{
		// ReSharper disable InconsistentNaming
		MONITOR_DEFAULTTONULL = 0,
		MONITOR_DEFAULTTOPRIMARY = 1,
		MONITOR_DEFAULTTONEAREST = 2,
		// ReSharper restore InconsistentNaming
	}
}
