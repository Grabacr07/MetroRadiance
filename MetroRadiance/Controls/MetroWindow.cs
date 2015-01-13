using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using MetroRadiance.Chrome.Behaviors;
using MetroRadiance.Core;
using MetroRadiance.Core.Win32;

namespace MetroRadiance.Controls
{
	/// <summary>
	/// Metro スタイル風のウィンドウを表します。
	/// </summary>
	[TemplatePart(Name = PART_ResizeGrip, Type = typeof(FrameworkElement))]
	public class MetroWindow : Window
	{
		// ReSharper disable InconsistentNaming
		private const string PART_ResizeGrip = "PART_ResizeGrip";
		// ReSharper restore InconsistentNaming

		static MetroWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
		}

		/// <summary>
		/// WPF が認識しているシステムの DPI (プライマリ モニターの DPI)。
		/// </summary>
		private Dpi systemDpi;

		/// <summary>
		/// このウィンドウが表示されているモニターの現在の DPI。
		/// </summary>
		internal Dpi currentDpi;

		private HwndSource source;
		private FrameworkElement resizeGrip;
		private FrameworkElement captionBar;

		#region DpiScaleTransform 依存関係プロパティ

		/// <summary>
		/// DPI スケーリングを実現する <see cref="Transform" /> を取得または設定します。
		/// </summary>
		public Transform DpiScaleTransform
		{
			get { return (Transform)this.GetValue(DpiScaleTransformProperty); }
			set { this.SetValue(DpiScaleTransformProperty, value); }
		}

		public static readonly DependencyProperty DpiScaleTransformProperty =
			DependencyProperty.Register("DpiScaleTransform", typeof(Transform), typeof(MetroWindow), new UIPropertyMetadata(Transform.Identity));

		#endregion

		#region WindowChrome 依存関係プロパティ

		/// <summary>
		/// ウインドウの非クライアント領域のカスタマイズ方法を指定した <see cref="WindowChrome"/> を取得または設定します。
		/// </summary>
		public WindowChrome WindowChrome
		{
			get { return (WindowChrome)this.GetValue(WindowChromeProperty); }
			set { this.SetValue(WindowChromeProperty, value); }
		}

		public static readonly DependencyProperty WindowChromeProperty =
			DependencyProperty.Register("WindowChrome", typeof(WindowChrome), typeof(MetroWindow), new UIPropertyMetadata(null));

		#endregion

		#region MetroChromeBehavior 依存関係プロパティ

		/// <summary>
		/// ウィンドウの枠を光らせるための <see cref="MetroChromeBehavior"/> を取得または設定します。
		/// </summary>
		public MetroChromeBehavior MetroChromeBehavior
		{
			get { return (MetroChromeBehavior)this.GetValue(MetroChromeBehaviorProperty); }
			set { this.SetValue(MetroChromeBehaviorProperty, value); }
		}
		public static readonly DependencyProperty MetroChromeBehaviorProperty =
			DependencyProperty.Register("MetroChromeBehavior", typeof(MetroChromeBehavior), typeof(MetroWindow), new UIPropertyMetadata(null, MetroChromeBehaviorChangedCallback));

		private static void MetroChromeBehaviorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (MetroWindow)d;
			var oldBehavior = (MetroChromeBehavior)e.OldValue;
			var newBehavior = (MetroChromeBehavior)e.NewValue;

			if (Equals(oldBehavior, newBehavior)) return;

			var behaviors = Interaction.GetBehaviors(instance);

			if (oldBehavior != null) behaviors.Remove(oldBehavior);
			if (newBehavior != null) behaviors.Add((Behavior)newBehavior.Clone());
		}

		#endregion

		#region IsRestoringWindowPlacement 依存関係プロパティ

		/// <summary>
		/// ウィンドウの位置とサイズを復元できるようにするかどうかを示す値を取得または設定します。
		/// </summary>
		public bool IsRestoringWindowPlacement
		{
			get { return (bool)this.GetValue(IsRestoringWindowPlacementProperty); }
			set { this.SetValue(IsRestoringWindowPlacementProperty, value); }
		}
		public static readonly DependencyProperty IsRestoringWindowPlacementProperty =
			DependencyProperty.Register("IsRestoringWindowPlacement", typeof(bool), typeof(MetroWindow), new UIPropertyMetadata(false));

		#endregion

		#region WindowSettings 依存関係プロパティ

		/// <summary>
		/// ウィンドウの位置とサイズを保存または復元する方法を指定するオブジェクトを取得または設定します。
		/// </summary>
		public IWindowSettings WindowSettings
		{
			get { return (IWindowSettings)this.GetValue(WindowSettingsProperty); }
			set { this.SetValue(WindowSettingsProperty, value); }
		}
		public static readonly DependencyProperty WindowSettingsProperty =
			DependencyProperty.Register("WindowSettings", typeof(IWindowSettings), typeof(MetroWindow), new UIPropertyMetadata(null));

		#endregion

		#region IsCaptionBar 添付プロパティ

		public static readonly DependencyProperty IsCaptionBarProperty =
			DependencyProperty.RegisterAttached("IsCaptionBar", typeof(bool), typeof(MetroWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, IsCaptionBarChangedCallback));

		public static void SetIsCaptionBar(FrameworkElement element, Boolean value)
		{
			element.SetValue(IsCaptionBarProperty, value);
		}
		public static bool GetIsCaptionBar(FrameworkElement element)
		{
			return (bool)element.GetValue(IsCaptionBarProperty);
		}

		private static void IsCaptionBarChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = d as FrameworkElement;
			if (instance == null) return;

			var window = GetWindow(instance) as MetroWindow;
			if (window == null) return;

			window.captionBar = (bool)e.NewValue ? instance : null;

			instance.Loaded += (sender, args) =>
			{
				var chrome = WindowChrome.GetWindowChrome(window);
				if (chrome != null) chrome.CaptionHeight = instance.ActualHeight;
			};
		}

		#endregion

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			this.source = PresentationSource.FromVisual(this) as HwndSource;
			if (this.source == null) return;

			this.systemDpi = this.GetSystemDpi() ?? Dpi.Default;
			if (PerMonitorDpi.IsSupported)
			{
				this.currentDpi = this.source.GetDpi();
				this.ChangeDpi(this.currentDpi);
				this.source.AddHook(this.WndProc);
			}
			else
			{
				this.currentDpi = this.systemDpi;
			}

			if (this.WindowSettings == null)
			{
				this.WindowSettings = new WindowSettings(this);
			}
			if (this.IsRestoringWindowPlacement)
			{
				this.WindowSettings.Reload();

				if (this.WindowSettings.Placement.HasValue)
				{
					var placement = this.WindowSettings.Placement.Value;
					placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
					placement.flags = 0;
					placement.showCmd = (placement.showCmd == SW.SHOWMINIMIZED ? SW.SHOWNORMAL : placement.showCmd);

					NativeMethods.SetWindowPlacement(this.source.Handle, ref placement);
				}
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.resizeGrip = this.GetTemplateChild(PART_ResizeGrip) as FrameworkElement;
			if (this.resizeGrip != null)
			{
				this.resizeGrip.Visibility = this.ResizeMode == ResizeMode.CanResizeWithGrip
					? Visibility.Visible
					: Visibility.Collapsed;

				WindowChrome.SetIsHitTestVisibleInChrome(this.resizeGrip, true);
			}
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			if (this.captionBar != null) this.captionBar.Opacity = 1.0;
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			if (this.captionBar != null) this.captionBar.Opacity = 0.5;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (!e.Cancel)
			{
				WINDOWPLACEMENT placement;
				var hwnd = new WindowInteropHelper(this).Handle;
				NativeMethods.GetWindowPlacement(hwnd, out placement);

				this.WindowSettings.Placement = this.IsRestoringWindowPlacement ? (WINDOWPLACEMENT?)placement : null;
				this.WindowSettings.Save();
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			if (this.source != null)
			{
				this.source.RemoveHook(this.WndProc);
			}
		}


		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.NCHITTEST)
			{
				if (this.ResizeMode == ResizeMode.CanResizeWithGrip
					&& this.WindowState == WindowState.Normal
					&& this.resizeGrip != null)
				{
					var ptScreen = lParam.ToPoint();
					var ptClient = this.resizeGrip.PointFromScreen(ptScreen);

					var rectTarget = new Rect(0, 0, this.resizeGrip.ActualWidth, this.resizeGrip.ActualHeight);
					if (rectTarget.Contains(ptClient))
					{
						handled = true;
						return (IntPtr)HitTestValues.HTBOTTOMRIGHT;
					}
				}
			}
			else if (msg == (int)WM.DPICHANGED)
			{
				var dpiX = wParam.ToLoWord();
				var dpiY = wParam.ToHiWord();
				this.ChangeDpi(new Dpi(dpiX, dpiY));
				handled = true;
			}

			return IntPtr.Zero;
		}

		private void ChangeDpi(Dpi dpi)
		{
			if (!PerMonitorDpi.IsSupported) return;

			this.DpiScaleTransform = (dpi == this.systemDpi)
				? Transform.Identity
				: new ScaleTransform((double)dpi.X / this.systemDpi.X, (double)dpi.Y / this.systemDpi.Y);

			this.Width = this.Width * dpi.X / this.currentDpi.X;
			this.Height = this.Height * dpi.Y / this.currentDpi.Y;

			this.currentDpi = dpi;
		}
	}
}
