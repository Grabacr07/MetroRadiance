using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroRadiance.Controls
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ITabItem
	{
		string Name { get; }
		int? Badge { get; }
		bool IsSelected { get; set; }
	}
}
