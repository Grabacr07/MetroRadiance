using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using ChromeHookCLR;
using MetroRadiance.Chrome.Internal;
using MetroRadiance.Core;
using MetroRadiance.Core.Win32;

namespace MetroRadiance.Chrome
{
	public class ExternalWindow : IWindow, IDisposable
	{
		private static IChromeHookService serviceInstance;
		private readonly IChromeHook external;
		private Rect rect;

		public IntPtr Handle { get; }

		public double Left => this.rect.X;
		public double Top => this.rect.Y;

		public double ActualWidth => this.rect.Width;
		public double ActualHeight => this.rect.Height;

		public bool IsActive => true;

		public WindowState WindowState { get; private set; }

		//public ResizeMode ResizeMode => this.Handle.GetWindowLong().HasFlag(WS.SIZEFRAME)
		//	? ResizeMode.CanResize
		//	: ResizeMode.NoResize;
		// 本当なら☝にしたいけど、別プロセスウィンドウに Resize メッセージ飛ばせないっぽいから保留
		public ResizeMode ResizeMode => ResizeMode.NoResize;

		public Visibility Visibility => NativeMethods.IsWindowVisible(this.Handle)
			? Visibility.Visible
			: Visibility.Hidden;

		public event EventHandler ContentRendered;
		public event EventHandler LocationChanged;
		public event EventHandler SizeChanged;
		public event EventHandler StateChanged;
		public event EventHandler Activated;
		public event EventHandler Deactivated;
		public event EventHandler Closed;

		public ExternalWindow(IntPtr hWnd)
		{
			this.Handle = hWnd;

			if (serviceInstance == null)
			{
				serviceInstance = ServiceFactory.CreateInstance();
			}

			this.external = serviceInstance.Register(hWnd);
			this.external.StateChanged += this.ExternalOnStateChanged;
			this.external.SizeChanged += this.ExternalOnSizeChanged;
			this.external.WindowMoved += this.ExternalOnWindowMoved;
			this.external.Activated += this.ExternalOnActivated;
			this.external.Deactivated += this.ExternalOnDeactivated;
			this.external.Closed += this.ExternalOnClosed;

			this.rect = this.GetExtendFrameBounds();
			//this.IsActive = NativeMethods.GetActiveWindow() == this.Handle;
			this.WindowState = WindowState.Normal;
			if (NativeMethods.IsIconic(hWnd)) this.WindowState = WindowState.Minimized;
			if (NativeMethods.IsZoomed(hWnd)) this.WindowState = WindowState.Maximized;
		}

		public bool Activate()
		{
			return NativeMethods.SetActiveWindow(this.Handle) != IntPtr.Zero;
		}

		private void ExternalOnStateChanged(IChromeHook sender, int state)
		{
			switch (state)
			{
				case 0:
					this.WindowState = WindowState.Normal;
					break;
				case 1:
					this.WindowState = WindowState.Minimized;
					break;
				case 2:
					this.WindowState = WindowState.Maximized;
					break;
			}

			this.StateChanged?.Invoke(this, EventArgs.Empty);
		}

		private void ExternalOnSizeChanged(IChromeHook sender, Size newSize)
		{
			this.rect = this.GetExtendFrameBounds();
			this.SizeChanged?.Invoke(this, EventArgs.Empty);
		}

		private void ExternalOnWindowMoved(IChromeHook sender, Point newLocation)
		{
			this.rect = this.GetExtendFrameBounds();
			this.LocationChanged?.Invoke(this, EventArgs.Empty);
		}

		private void ExternalOnActivated(IChromeHook sender)
		{
			//this.IsActive = true;
			this.Activated?.Invoke(this, EventArgs.Empty);
		}

		private void ExternalOnDeactivated(IChromeHook sender)
		{
			//this.IsActive = false;
			this.Deactivated?.Invoke(this, EventArgs.Empty);
		}

		private void ExternalOnClosed(IChromeHook sender)
		{
			this.external.Dispose();
			this.Closed?.Invoke(this, EventArgs.Empty);
		}

		public void Dispose()
		{
			this.external.Dispose();
		}


		[DllImport("dwmapi.dll")]
		private static extern void DwmGetWindowAttribute(IntPtr hWnd, DWMWINDOWATTRIBUTE dwAttribute, [Out] out RECT pvAttribute, int cbAttribute);

		private Rect GetExtendFrameBounds()
		{
			RECT frameBounds;
			DwmGetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out frameBounds, Marshal.SizeOf(typeof(RECT)));

			var dpi = PerMonitorDpi.GetDpi(this.Handle);
			var l = frameBounds.Left * dpi.ScaleX;
			var t = frameBounds.Top * dpi.ScaleY;
			var w = (frameBounds.Right - frameBounds.Left) * dpi.ScaleX;
			var h = (frameBounds.Bottom - frameBounds.Top) * dpi.ScaleY;

			return new Rect(l, t, w, h);
		}
	}
}
