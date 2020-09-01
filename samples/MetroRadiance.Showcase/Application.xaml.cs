using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MetroRadiance.UI;

namespace MetroRadiance.Showcase
{
	public partial class Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false; // Debug for MetroRadiance.UI.Controls.DoubleRule
			this.ShutdownMode = ShutdownMode.OnMainWindowClose;

			ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);
		}
	}
}
