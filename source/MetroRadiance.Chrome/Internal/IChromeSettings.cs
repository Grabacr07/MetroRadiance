using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MetroRadiance.Chrome.Internal
{
	public interface IChromeSettings
	{
		Brush ActiveBrush { get; }

		Brush InactiveBrush { get; }

		ChromeMode ChromeMode { get; }
	}
}