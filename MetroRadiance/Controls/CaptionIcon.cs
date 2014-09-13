using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MetroRadiance.Core.Win32;

namespace MetroRadiance.Controls
{
	public class CaptionIcon : Button
	{
		static CaptionIcon()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptionIcon), new FrameworkPropertyMetadata(typeof(CaptionIcon)));
		}

		private bool isSystemMenuOpened;


		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var window = Window.GetWindow(this);
			if (window == null) return;

			window.SourceInitialized += this.Initialize;
		}

		private void Initialize(object sender, EventArgs e)
		{
			var window = (Window)sender;
			window.SourceInitialized -= this.Initialize;

			var source = PresentationSource.FromVisual(window) as HwndSource;
			if (source != null)
			{
				source.AddHook(this.WndProc);
				window.Closed += (o, args) => source.RemoveHook(this.WndProc);
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
			var window = Window.GetWindow(this) as MetroWindow;
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
						SystemCommands.ShowSystemMenu(window, new Point(point.X / window.currentDpi.ScaleX, point.Y / window.currentDpi.ScaleY));
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
			var window = Window.GetWindow(this) as MetroWindow;
			if (window == null)
			{
				base.OnMouseRightButtonUp(e);
				return;
			}

			var point = this.PointToScreen(e.GetPosition(this));
			SystemCommands.ShowSystemMenu(window, new Point(point.X / window.currentDpi.ScaleX, point.Y / window.currentDpi.ScaleY));
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			this.isSystemMenuOpened = false;
			base.OnMouseLeave(e);
		}
	}
}
