using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace MetroRadiance.Chrome.Internal
{
	internal class WindowWrapper : IWindow
	{
		public Window Window { get; }

		public IntPtr Handle { get; private set; }
		public double Left => this.Window.Left;
		public double Top => this.Window.Top;
		public double ActualWidth => this.Window.ActualWidth;
		public double ActualHeight => this.Window.ActualHeight;
		public bool IsActive => this.Window.IsActive;
		public WindowState WindowState => this.Window.WindowState;
		public ResizeMode ResizeMode => this.Window.ResizeMode;
		public Visibility Visibility => this.Window.Visibility;

		public event EventHandler ContentRendered
		{
			add { this.Window.ContentRendered += value; }
			remove { this.Window.ContentRendered -= value; }
		}
		public event EventHandler LocationChanged
		{
			add { this.Window.LocationChanged += value; }
			remove { this.Window.LocationChanged -= value; }
		}
		public event EventHandler StateChanged
		{
			add { this.Window.StateChanged += value; }
			remove { this.Window.StateChanged -= value; }
		}
		public event EventHandler Activated
		{
			add { this.Window.Activated += value; }
			remove { this.Window.Activated -= value; }
		}
		public event EventHandler Deactivated
		{
			add { this.Window.Deactivated += value; }
			remove { this.Window.Deactivated -= value; }
		}
		public event EventHandler Closed
		{
			add { this.Window.Closed += value; }
			remove { this.Window.Closed -= value; }
		}
		public event EventHandler SizeChanged;

		public WindowWrapper(Window window)
		{
			this.Window = window;
			this.Window.SourceInitialized += (sender, args) =>
			{
				var source = PresentationSource.FromVisual(this.Window) as HwndSource;
				if (source != null) this.Handle = source.Handle;
			};
			this.Window.SizeChanged += (sender, args) => this.SizeChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool Activate() => this.Window.Activate();
	}
}
