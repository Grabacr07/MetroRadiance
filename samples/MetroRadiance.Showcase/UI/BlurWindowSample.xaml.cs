using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace MetroRadiance.Showcase.UI
{
	public partial class BlurWindowSample
	{
		public BlurWindowSample()
		{
			this.InitializeComponent();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			this.DragMove();
		}
	}
}
