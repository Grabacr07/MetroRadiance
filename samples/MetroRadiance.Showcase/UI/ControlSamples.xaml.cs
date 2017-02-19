using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetroRadiance.Showcase.UI
{
	partial class ControlSamples
	{
		public ControlSamples()
		{
			this.InitializeComponent();
			this.DataContext = new SampleValues();
		}
	}

	public class SampleValues
	{
		public int Int32 { get; set; } = 32;
	}
}
