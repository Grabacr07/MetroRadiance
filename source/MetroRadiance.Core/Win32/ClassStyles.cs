using System;

namespace MetroRadiance.Core.Win32
{
	[Flags]
	public enum ClassStyles
	{
		// ReSharper disable InconsistentNaming
		ByteAlignClient = 0x1000,
		ByteAlignWindow = 0x2000,
		ClassDC = 0x40,
		DblClks = 8,
		DropShadow = 0x20000,
		GlobalClass = 0x4000,
		HRedraw = 2,
		Ime = 0x10000,
		NoClose = 0x200,
		OwnDC = 0x20,
		ParentDC = 0x80,
		SaveBits = 0x800,
		VRedraw = 1,
		// ReSharper restore InconsistentNaming
	}
}
