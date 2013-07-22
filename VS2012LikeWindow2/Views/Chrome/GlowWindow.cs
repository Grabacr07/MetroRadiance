using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using VS2012LikeWindow2.Models.Win32;

namespace VS2012LikeWindow2.Views.Chrome
{
	/// <summary>
	/// ウィンドウの四辺にアタッチされる発行ウィンドウを表します。
	/// </summary>
	internal class GlowWindow : Window
	{
		static GlowWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (GlowWindow), new FrameworkPropertyMetadata(typeof (GlowWindow)));
		}

		#region IsGlow 依存関係プロパティ

		public bool IsGlow
		{
			get { return (bool)this.GetValue(IsGlowProperty); }
			set { this.SetValue(IsGlowProperty, value); }
		}

		public static readonly DependencyProperty IsGlowProperty =
			DependencyProperty.Register("IsGlow", typeof (bool), typeof (GlowWindow), new UIPropertyMetadata(true));

		#endregion

		#region Orientation 依存関係プロパティ

		public Orientation Orientation
		{
			get { return (Orientation)this.GetValue(OrientationProperty); }
			set { this.SetValue(OrientationProperty, value); }
		}

		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof (Orientation), typeof (GlowWindow), new UIPropertyMetadata(Orientation.Horizontal));

		#endregion

		private IntPtr handle;
		private readonly GlowWindowProcessor processor;

		private readonly Window owner;
		private IntPtr ownerHandle;

		internal GlowWindow(Window owner, GlowWindowProcessor processor)
		{
			this.owner = owner;
			this.processor = processor;

			this.WindowStyle = WindowStyle.None;
			this.AllowsTransparency = true;
			this.ShowActivated = false;
			this.Visibility = Visibility.Collapsed;
			this.ResizeMode = ResizeMode.NoResize;
			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.Left = processor.GetLeft(owner.Left, owner.ActualWidth);
			this.Top = processor.GetTop(owner.Top, owner.ActualHeight);
			this.Width = processor.GetWidth(owner.Left, owner.ActualWidth);
			this.Height = processor.GetHeight(owner.Top, owner.ActualHeight);
			this.Orientation = processor.Orientation;
			this.HorizontalContentAlignment = processor.HorizontalAlignment;
			this.VerticalContentAlignment = processor.VerticalAlignment;
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source != null)
			{
				this.handle = source.Handle;

				var wsex = this.handle.GetWindowLongEx();
				wsex ^= WSEX.APPWINDOW;
				wsex |= WSEX.NOACTIVATE;
				this.handle.SetWindowLongEx(wsex);

				source.AddHook(this.WndProc);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source != null)
			{
				source.RemoveHook(this.WndProc);
			}
		}

		public void Update()
		{
			if (this.owner.WindowState == WindowState.Normal)
			{
				this.UpdateCore();
				this.Visibility = Visibility.Visible;
			}
			else
			{
				this.Visibility = Visibility.Collapsed;
			}
		}

		private void UpdateCore()
		{
			if (this.ownerHandle == IntPtr.Zero)
			{
				this.ownerHandle = new WindowInteropHelper(this.owner).Handle;
			}

			this.IsGlow = this.owner.IsActive;

			var dpiScaleFactor = this.GetDpiScaleFactor();
			var left = (int)Math.Round(processor.GetLeft(owner.Left, owner.ActualWidth) * dpiScaleFactor.X);
			var top = (int)Math.Round(processor.GetTop(owner.Top, owner.ActualHeight) * dpiScaleFactor.Y);
			var width = (int)Math.Round(processor.GetWidth(owner.Left, owner.ActualWidth) * dpiScaleFactor.X);
			var height = (int)Math.Round(processor.GetHeight(owner.Top, owner.ActualHeight) * dpiScaleFactor.Y);
			NativeMethods.SetWindowPos(this.handle, this.ownerHandle, left, top, width, height, SWP.NOACTIVATE);
		}


		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.MOUSEACTIVATE)
			{
				handled = true;
				return new IntPtr(3);
			}

			if (msg == (int)WM.LBUTTONDOWN)
			{
				var ptScreen = lParam.ToPoint();

				NativeMethods.PostMessage(
					this.ownerHandle,
					(uint)WM.NCLBUTTONDOWN,
					(IntPtr)this.processor.GetHitTestValue(ptScreen, this.ActualWidth, this.ActualHeight),
					IntPtr.Zero);
			}
			if (msg == (int)WM.NCHITTEST)
			{
				var ptScreen = lParam.ToPoint();
				var ptClient = this.PointFromScreen(ptScreen);

				this.Cursor = this.processor.GetCursor(ptClient, this.ActualWidth, this.ActualHeight);
			}

			return IntPtr.Zero;
		}
	}
}
