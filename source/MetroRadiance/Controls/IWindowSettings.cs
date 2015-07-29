using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MetroRadiance.Core.Win32;

namespace MetroRadiance.Controls
{
	public interface IWindowSettings
	{
		WINDOWPLACEMENT? Placement { get; set; }
		void Reload();
		void Save();
	}


	public class WindowSettings : ApplicationSettingsBase, IWindowSettings
	{
		public WindowSettings(Window window) : base(window.GetType().FullName) { }

		[UserScopedSetting]
		public WINDOWPLACEMENT? Placement
		{
			get { return this["Placement"] != null ? (WINDOWPLACEMENT?)(WINDOWPLACEMENT)this["Placement"] : null; }
			set { this["Placement"] = value; }
		}
	}
}
