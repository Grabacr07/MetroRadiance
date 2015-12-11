using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using MetroRadiance.Properties;
using MetroRadiance.Win32;

namespace MetroRadiance.Chrome.Primitives
{
	internal abstract class ChromeWindow : Window
	{
		static ChromeWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(ChromeWindow),
				new FrameworkPropertyMetadata(typeof(ChromeWindow)));
		}

		public static double DefaultSize { get; set; } = 8.0;

		private HwndSource _source;
		private IntPtr _handle;
		private bool _closed;
		private WindowState _ownerPreviewState;

		/// <summary>WPF が認識しているシステム DPI。Per-Monitor DPI 対応プラットフォームで実行されている場合は null。</summary>
		private Dpi? _systemDpi;

		protected Dpi Dpi => this._systemDpi ?? PerMonitorDpi.GetDpi(this.Owner.Handle);

		internal new IWindow Owner { get; private set; }

		internal SizingMode SizingMode { get; set; }

		public Thickness Offset { get; set; } = new Thickness(DefaultSize);


		protected ChromeWindow()
		{
			this.Title = nameof(ChromeWindow);
			this.WindowStyle = WindowStyle.None;
			this.AllowsTransparency = true;
			this.ShowActivated = false;
			this.ShowInTaskbar = false;
			this.Visibility = Visibility.Collapsed;
			this.Background = Brushes.Transparent;
			this.ResizeMode = ResizeMode.NoResize;
			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.Width = .0;
			this.Height = .0;
		}


		public void Attach(Window window)
		{
			var binding = new Binding(nameof(this.BorderBrush)) { Source = window, };
			this.SetBinding(BackgroundProperty, binding);

			var wrapper = WindowWrapper.Create(window);
			var initialShow = window.IsLoaded;

			this.Attach(wrapper, initialShow);
		}

		public void Attach(ExternalWindow window)
		{
			this.Background = Brushes.OrangeRed;

			this.Attach(window, true);
		}

		internal void Attach(IWindow window, bool initialShow)
		{
			this.Detach();

			this.Owner = window;
			this.Owner.StateChanged += this.OwnerStateChangedCallback;
			this.Owner.LocationChanged += this.OwnerLocationChangedCallback;
			this.Owner.SizeChanged += this.OwnerSizeChangedCallback;
			this.Owner.Activated += this.OwnerActivatedCallback;
			this.Owner.Deactivated += this.OwnerDeactivatedCallback;
			this.Owner.Closed += this.OwnerClosedCallback;
			
			if (initialShow)
			{
				this._ownerPreviewState = this.Owner.WindowState;
				this.Show();
				this.Update();
			}
			else
			{
				this.Owner.ContentRendered += this.OwnerContentRenderedCallback;
			}
		}

		public void Detach()
		{
			if (this.Owner == null) return;

			this.Owner.StateChanged -= this.OwnerStateChangedCallback;
			this.Owner.LocationChanged -= this.OwnerLocationChangedCallback;
			this.Owner.SizeChanged -= this.OwnerSizeChangedCallback;
			this.Owner.Activated -= this.OwnerActivatedCallback;
			this.Owner.Deactivated -= this.OwnerDeactivatedCallback;
			this.Owner.Closed -= this.OwnerClosedCallback;
			this.Owner.ContentRendered -= this.OwnerContentRenderedCallback;
			this.Owner = null;
		}

		public void Update()
		{
			if (this.Owner == null || this._closed) return;

			if (this.Owner.Visibility == Visibility.Hidden)
			{
				this.Visibility = Visibility.Hidden;
				this.UpdateCore();
			}
			else if (this.Owner.WindowState == WindowState.Normal)
			{
				if (this._ownerPreviewState == WindowState.Minimized
					&& SystemParameters.MinimizeAnimation)
				{
					Action<Task> action = t =>
					{
						if (t.IsCompleted)
						{
							this.Visibility = Visibility.Visible;
							this.UpdateCore();
						}
						else if (t.IsFaulted)
						{
							t.Exception.Dump();
						}
					};

					// 最小化から復帰 && 最小化アニメーションが有効の場合
					// アニメーションが完了しウィンドウが表示されるまで遅延させる (それがだいたい 250 ミリ秒くらい)
					Task.Delay(Settings.Default.DelayForMinimizeToNormal)
						.ContinueWith(action, TaskScheduler.FromCurrentSynchronizationContext())
						.ContinueWith(t => t.Exception.Dump(), TaskContinuationOptions.OnlyOnFaulted);
				}
				else
				{
					this.Visibility = Visibility.Visible;
					this.UpdateCore();
				}
			}
			else
			{
				this.Visibility = Visibility.Collapsed;
			}
		}

		private void UpdateCore()
		{
			var dpi = this.Dpi;
			var left = this.GetLeft(dpi);
			var top = this.GetTop(dpi);
			var width = this.GetWidth(dpi);
			var height = this.GetHeight(dpi);

			NativeMethods.SetWindowPos(this._handle, this.Owner.Handle, left, top, width, height, SWP.NOACTIVATE);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source == null) throw new InvalidOperationException("HwndSource is missing.");

			this._source = source;
			this._source.AddHook(this.WndProc);
			this._systemDpi = PerMonitorDpi.IsSupported ? (Dpi?)null : (this.GetSystemDpi() ?? Dpi.Default);

			var wndStyle = source.Handle.GetWindowLongEx();
			var gclStyle = source.Handle.GetClassLong(ClassLongFlags.GclStyle);

			this._handle = source.Handle;
			this._handle.SetWindowLongEx(wndStyle | WSEX.TOOLWINDOW);
			this._handle.SetClassLong(ClassLongFlags.GclStyle, gclStyle | ClassStyles.DblClks);

			var wrapper = this.Owner as WindowWrapper;
			if (wrapper != null)
			{
				base.Owner = wrapper.Window;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			this._source.RemoveHook(this.WndProc);
			this._closed = true;
		}

		protected abstract int GetLeft(Dpi dpi);
		protected abstract int GetTop(Dpi dpi);
		protected abstract int GetWidth(Dpi dpi);
		protected abstract int GetHeight(Dpi dpi);
		protected virtual IntPtr? WndProcOverride(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			return null;
		}

		protected T GetContentValueOrDefault<T>(Func<FrameworkElement, T> valueSelector, T @default)
		{
			var element = this.Content as FrameworkElement;
			if (element == null) return @default;

			var contentControl = this.Content as ContentControl;
			if (contentControl?.Content is FrameworkElement)
			{
				return valueSelector((FrameworkElement)contentControl.Content);
			}

			return valueSelector(element);
		}

		protected double GetContentSizeOrDefault(Func<FrameworkElement, double> sizeSelector)
		{
			return this.GetContentValueOrDefault(sizeSelector, DefaultSize).SpecifiedOrDefault(DefaultSize);
		}

		private void OwnerContentRenderedCallback(object sender, EventArgs eventArgs)
		{
			if (this._closed) return;

			this._ownerPreviewState = this.Owner.WindowState;
			this.Show();
			this.Update();
		}

		private void OwnerStateChangedCallback(object sender, EventArgs eventArgs)
		{
			if (this._closed) return;
			this.Update();
			this._ownerPreviewState = this.Owner.WindowState;
		}

		private void OwnerLocationChangedCallback(object sender, EventArgs eventArgs)
		{
			this.Update();
		}

		private void OwnerSizeChangedCallback(object sender, EventArgs eventArgs)
		{
			this.Update();
		}

		private void OwnerActivatedCallback(object sender, EventArgs eventArgs)
		{
			this.Update();
		}

		private void OwnerDeactivatedCallback(object sender, EventArgs eventArgs)
		{
			this.Update();
		}

		private void OwnerClosedCallback(object sender, EventArgs eventArgs)
		{
			this.Close();
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.MOUSEACTIVATE)
			{
				handled = true;
				return new IntPtr(3);
			}

			return this.WndProcOverride(hwnd, msg, wParam, lParam, ref handled) ?? IntPtr.Zero;
		}

		internal void Resize(SizingMode mode)
		{
			if (!this.Owner.IsActive)
			{
				this.Owner.Activate();
			}

			NativeMethods.PostMessage(this.Owner.Handle, (uint)WM.NCLBUTTONDOWN, (IntPtr)mode, IntPtr.Zero);
		}
	}
}
