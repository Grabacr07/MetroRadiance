using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using MetroRadiance.Platform;
using MetroRadiance.Properties;

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

		public static double Thickness { get; set; } = 8.0;

		private readonly WindowInteropHelper _windowInteropHelper;
		private HwndSource _source;
		private IntPtr _handle;
		private bool _sourceInitialized;
		private bool _closed;
		private WindowState _ownerPreviewState;
		private Dpi _systemDpi;

		protected Dpi CurrentDpi { get; private set; }

		internal new IChromeOwner Owner { get; private set; }

		internal SizingMode SizingMode { get; set; }

		#region Thickness dependency property

		internal static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
			nameof(Offset), typeof(Thickness), typeof(ChromeWindow), new PropertyMetadata(new Thickness(Thickness)));

		internal Thickness Offset
		{
			get { return (Thickness)this.GetValue(OffsetProperty); }
			set { this.SetValue(OffsetProperty, value); }
		}

		#endregion

		#region DpiScaleTransform dependency property

		public static readonly DependencyProperty DpiScaleTransformProperty = DependencyProperty.Register(
			nameof(DpiScaleTransform), typeof(Transform), typeof(ChromeWindow), new PropertyMetadata(Transform.Identity));

		public Transform DpiScaleTransform
		{
			get { return (Transform)this.GetValue(DpiScaleTransformProperty); }
			set { this.SetValue(DpiScaleTransformProperty, value); }
		}

		#endregion

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
			this._windowInteropHelper = new WindowInteropHelper(this);
		}


		public void Attach(Window window)
		{
			var binding = new Binding(nameof(this.BorderBrush)) { Source = window, };
			this.SetBinding(BackgroundProperty, binding);

			var wrapper = WindowWrapper.Create(window);
			var initialShow = window.IsLoaded;

			this.Attach(wrapper, initialShow);
		}

		public void Attach(IChromeOwner window)
		{
			Action<Color> applyAccent = color =>
				this.Background = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));

			var disposable = WindowsTheme.Accent.RegisterListener(applyAccent);
			this.Closed += (sender, e) => disposable.Dispose();
			applyAccent(WindowsTheme.Accent.Current);

			this.Attach(window, true);
		}

		internal void Attach(IChromeOwner window, bool initialShow)
		{
			this.Detach();

			this._windowInteropHelper.Owner = window.Handle;
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
			this._windowInteropHelper.Owner = IntPtr.Zero;
		}

		public void Update()
		{
			if (this.Owner == null || !this._sourceInitialized || this._closed) return;

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
			var positionDpi = this._systemDpi;

			if (PerMonitorDpi.IsSupported)
			{
				var currentDpi = PerMonitorDpi.GetDpi(this.Owner.Handle);
				if (currentDpi != this.CurrentDpi)
				{
					this.DpiScaleTransform = currentDpi == this._systemDpi
						? Transform.Identity
						: new ScaleTransform((double)currentDpi.X / this._systemDpi.X, (double)currentDpi.Y / this._systemDpi.Y);
					this.CurrentDpi = currentDpi;
				}
			}

			var left = this.GetLeft(positionDpi);
			var top = this.GetTop(positionDpi);
			var width = this.GetWidth(positionDpi);
			var height = this.GetHeight(positionDpi);

			User32.SetWindowPos(this._handle, this.Owner.Handle, left, top, width, height, SetWindowPosFlags.SWP_NOACTIVATE);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source == null) throw new InvalidOperationException("HwndSource is missing.");

			this._source = source;
			this._source.AddHook(this.WndProc);
			this._handle = source.Handle;
			this._systemDpi = this.GetSystemDpi() ?? Dpi.Default;
			this.CurrentDpi = this._systemDpi;

			var wndStyle = User32.GetWindowLongEx(source.Handle);
			var gclStyle = User32.GetClassLong(source.Handle, ClassLongPtrIndex.GCL_STYLE);

			User32.SetWindowLongEx(this._handle, wndStyle | WindowExStyles.WS_EX_TOOLWINDOW);
			User32.SetClassLong(this._handle, ClassLongPtrIndex.GCL_STYLE, gclStyle | WindowClassStyles.CS_DBLCLKS);

			var wrapper = this.Owner as WindowWrapper;
			if (wrapper != null)
			{
				base.Owner = wrapper.Window;
			}

			this._sourceInitialized = true;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			if (!this._sourceInitialized) return;

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
			return this.GetContentValueOrDefault(sizeSelector, Thickness).SpecifiedOrDefault(Thickness);
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
			if (msg == (int)WindowsMessages.WM_MOUSEACTIVATE)
			{
				handled = true;
				return new IntPtr(3);
			}

			//if (msg == (int)WindowsMessages.WM_DPICHANGED)
			//{
			//	System.Diagnostics.Debug.WriteLine("WM_DPICHANGED: " + this.GetType().Name);

			//	var dpiX = wParam.ToLoWord();
			//	var dpiY = wParam.ToHiWord();
			//	this.ChangeDpi(new Dpi(dpiX, dpiY));
			//	handled = true;
			//	return IntPtr.Zero;
			//}

			return this.WndProcOverride(hwnd, msg, wParam, lParam, ref handled) ?? IntPtr.Zero;
		}

		private void ChangeDpi(Dpi dpi)
		{
			if (!PerMonitorDpi.IsSupported) return;

			this.DpiScaleTransform = dpi == this._systemDpi
				? Transform.Identity
				: new ScaleTransform((double)dpi.X / this._systemDpi.X, (double)dpi.Y / this._systemDpi.Y);

			this.CurrentDpi = dpi;
		}

		internal void Resize(SizingMode mode)
		{
			if (!this.Owner.IsActive)
			{
				this.Owner.Activate();
			}

			this.Owner.Resize(mode);
		}
	}
}
