using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MetroRadiance.Core.Win32
{
	public static class NativeExtensions
	{
		public static ushort ToLoWord(this IntPtr dword)
		{
			return (ushort)((uint)dword & 0xffff);
		}

		public static ushort ToHiWord(this IntPtr dword)
		{
			return (ushort)((uint)dword >> 16);
		}
	}
}
