using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MetroRadiance.Chrome
{
	public interface IChromeHost
	{
		Window Window { get; }
		Brush ActiveBrush { get; }
		Brush InactiveBrush { get; }
		ChromeMode Mode { get; }
	}
}
