using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using VS2012LikeWindow2.Models.Win32;

namespace VS2012LikeWindow2.Views.Chrome
{
	public class CaptionIcon : Button
	{
		static CaptionIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (CaptionIcon), new FrameworkPropertyMetadata(typeof (CaptionIcon)));
		}

		private bool isSystemMenuOpened = false;

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized( e );

			var window = Window.GetWindow(this);
			if (window == null) return;

			window.ContentRendered += window_ContentRendered;
		}

		private void window_ContentRendered(object sender, EventArgs e)
		{
			var window = (Window)sender;
			window.ContentRendered -= window_ContentRendered;

			var source = PresentationSource.FromVisual(window) as HwndSource;
			if (source != null)
			{
				source.AddHook(this.WndProc);
				window.Closing += window_Closing;
			}
		}

		private void window_Closing(object sender, CancelEventArgs e)
		{
			var window = (Window)sender;
			window.Closing -= window_Closing;

			var source = PresentationSource.FromVisual(window) as HwndSource;
			if (source != null)
			{
				source.RemoveHook(this.WndProc);
			}
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.NCLBUTTONDOWN)
			{
				this.isSystemMenuOpened = false;
			}

			return IntPtr.Zero;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			var window = Window.GetWindow(this);
			if (window == null)
			{
				base.OnMouseDown(e);
				return;
			}

			if (e.ChangedButton == MouseButton.Left)
			{
				if (e.ClickCount == 1)
				{
					if (!this.isSystemMenuOpened)
					{
						this.isSystemMenuOpened = true;
						var point = this.PointToScreen(new Point(0, this.ActualHeight));
						SystemCommands.ShowSystemMenu(window, point);
					}
					else
					{
						this.isSystemMenuOpened = false;
					}
				}
				else if (e.ClickCount == 2)
				{
					window.Close();
				}
			}
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseRightButtonUp(e);
			
			var window = Window.GetWindow(this);
			if (window == null) return;

			var point = this.PointToScreen(e.GetPosition(this));
			SystemCommands.ShowSystemMenu(window, point);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			this.isSystemMenuOpened = false;
			base.OnMouseLeave(e);
		}
	}
}
