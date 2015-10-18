using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using MetroRadiance.Chrome.Internal;

namespace MetroRadiance.Chrome
{
	[Notify]
	public class GlowingChrome : IChromeSettings, INotifyPropertyChanged, IDisposable
	{
		private readonly IntPtr hWnd;
		private GlowWindow left, right, top, bottom;
		private ExternalWindow window;

		public Brush ActiveBrush
		{
			get { return this.activeBrush; }
			set { this.SetProperty(ref this.activeBrush, value, activeBrushPropertyChangedEventArgs); }
		}

		public Brush InactiveBrush
		{
			get { return this.inactiveBrush; }
			set { this.SetProperty(ref this.inactiveBrush, value, inactiveBrushPropertyChangedEventArgs); }
		}

		public ChromeMode ChromeMode
		{
			get { return this.chromeMode; }
			set { this.SetProperty(ref this.chromeMode, value, chromeModePropertyChangedEventArgs); }
		}

		[NonNotify]
		private bool IsNotNull => this.left != null && this.right != null && this.top != null && this.bottom != null;

		public GlowingChrome(IntPtr hWnd)
		{
			this.hWnd = hWnd;
		}

		public void Show()
		{
			if (this.IsNotNull)
			{
				this.left.IsGlowing = this.right.IsGlowing = this.top.IsGlowing = this.bottom.IsGlowing = true;
			}
			else
			{
				this.window = new ExternalWindow(this.hWnd);
				this.left = new GlowWindow(this.window, this, new GlowWindowProcessorLeft(), true);
				this.right = new GlowWindow(this.window, this, new GlowWindowProcessorRight(), true);
				this.top = new GlowWindow(this.window, this, new GlowWindowProcessorTop(), true);
				this.bottom = new GlowWindow(this.window, this, new GlowWindowProcessorBottom(), true);
			}
		}

		public void Hide()
		{
			if (this.IsNotNull)
			{
				this.left.IsGlowing = this.right.IsGlowing = this.top.IsGlowing = this.bottom.IsGlowing = false;
			}
		}

		public void Dispose()
		{
			if (this.IsNotNull)
			{
				this.left.Close();
				this.right.Close();
				this.top.Close();
				this.bottom.Close();
			}
			this.window?.Dispose();
		}

		#region NotifyPropertyChangedGenerator

		public event PropertyChangedEventHandler PropertyChanged;

		private Brush activeBrush;
		private static readonly PropertyChangedEventArgs activeBrushPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(ActiveBrush));
		private Brush inactiveBrush;
		private static readonly PropertyChangedEventArgs inactiveBrushPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(InactiveBrush));
		private ChromeMode chromeMode;
		private static readonly PropertyChangedEventArgs chromeModePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(ChromeMode));

		private void SetProperty<T>(ref T field, T value, PropertyChangedEventArgs ev)
		{
			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				this.PropertyChanged?.Invoke(this, ev);
			}
		}

		#endregion
	}
}
