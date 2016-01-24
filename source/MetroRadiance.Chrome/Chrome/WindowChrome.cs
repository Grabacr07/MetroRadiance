using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetroRadiance.Chrome.Primitives;
using MetroRadiance.Platform;

namespace MetroRadiance.Chrome
{
	/// <summary>
	/// ウィンドウにアタッチされ、四辺にカスタム UI を表示する機能を提供します。
	/// </summary>
	public class WindowChrome : DependencyObject
	{
		private static readonly HashSet<FrameworkElement> _sizingElements = new HashSet<FrameworkElement>();

		private readonly ChromePart _top = new ChromePart(new TopChromeWindow(), Dock.Top);
		private readonly ChromePart _left = new ChromePart(new LeftChromeWindow(), Dock.Left);
		private readonly ChromePart _right = new ChromePart(new RightChromeWindow(), Dock.Right);
		private readonly ChromePart _bottom = new ChromePart(new BottomChromeWindow(), Dock.Bottom);

		#region Content wrappers

		public object Left
		{
			get { return this._left.Content; }
			set { this._left.Content = value; }
		}

		public object Right
		{
			get { return this._right.Content; }
			set { this._right.Content = value; }
		}

		public object Top
		{
			get { return this._top.Content; }
			set { this._top.Content = value; }
		}

		public object Bottom
		{
			get { return this._bottom.Content; }
			set { this._bottom.Content = value; }
		}

		#endregion

		#region BorderThickness dependency property

		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
			nameof(BorderThickness), typeof(Thickness), typeof(WindowChrome), new PropertyMetadata(default(Thickness), BorderThicknessPropertyCallback));

		public Thickness BorderThickness
		{
			get { return (Thickness)this.GetValue(BorderThicknessProperty); }
			set { this.SetValue(BorderThicknessProperty, value); }
		}

		private static void BorderThicknessPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (WindowChrome)d;
			var newValue = (Thickness)e.NewValue;

			instance._top.Edge.Thickness = newValue.Top;
			instance._left.Edge.Thickness = newValue.Left;
			instance._right.Edge.Thickness = newValue.Right;
			instance._bottom.Edge.Thickness = newValue.Bottom;
		}

		#endregion

		#region OverrideDefaultEdge dependency property

		public static readonly DependencyProperty OverrideDefaultEdgeProperty = DependencyProperty.Register(
			nameof(OverrideDefaultEdge), typeof(bool), typeof(WindowChrome), new PropertyMetadata(false, OverrideDefaultEdgePropertyCallback));

		public bool OverrideDefaultEdge
		{
			get { return (bool)this.GetValue(OverrideDefaultEdgeProperty); }
			set { this.SetValue(OverrideDefaultEdgeProperty, value); }
		}

		private static void OverrideDefaultEdgePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (WindowChrome)d;
			var oldValue = (bool)e.OldValue;
			var newValue = (bool)e.NewValue;

			if (!oldValue && newValue)
			{
				// false -> true
				instance._top.Edge.Visibility = Visibility.Collapsed;
				instance._left.Edge.Visibility = Visibility.Collapsed;
				instance._right.Edge.Visibility = Visibility.Collapsed;
				instance._bottom.Edge.Visibility = Visibility.Collapsed;
			}
			if (oldValue && !newValue)
			{
				// true -> false
				instance._top.Edge.Visibility = Visibility.Visible;
				instance._left.Edge.Visibility = Visibility.Visible;
				instance._right.Edge.Visibility = Visibility.Visible;
				instance._bottom.Edge.Visibility = Visibility.Visible;
			}
		}

		#endregion

		#region SizingMode attached property

		public static readonly DependencyProperty SizingModeProperty = DependencyProperty.RegisterAttached(
			"SizingMode", typeof(SizingMode), typeof(WindowChrome), new PropertyMetadata(SizingMode.None, SizingModeChangedCallback));

		public static void SetSizingMode(FrameworkElement element, SizingMode value)
		{
			element.SetValue(SizingModeProperty, value);
		}

		public static SizingMode GetSizingMode(FrameworkElement element)
		{
			return (SizingMode)element.GetValue(SizingModeProperty);
		}

		private static void SizingModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var element = d as FrameworkElement;
			if (element == null) return;

			var newValue = (SizingMode)e.NewValue;
			if (newValue != SizingMode.None)
			{
				if (_sizingElements.Add(element))
				{
					element.PreviewMouseLeftButtonDown += SizingElementButtonDownCallback;
				}
			}
			else
			{
				if (_sizingElements.Remove(element))
				{
					element.PreviewMouseLeftButtonDown -= SizingElementButtonDownCallback;
				}
			}
		}

		private static void SizingElementButtonDownCallback(object sender, MouseButtonEventArgs args)
		{
			var element = sender as FrameworkElement;
			if (element == null) return;

			(Window.GetWindow(element) as ChromeWindow)?.Resize(GetSizingMode(element));
		}

		#endregion

		#region Instance attached property

		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached(
			"Instance", typeof(WindowChrome), typeof(WindowChrome), new PropertyMetadata(default(WindowChrome), InstanceChangedCallback));

		public static void SetInstance(Window window, WindowChrome value)
		{
			window.SetValue(InstanceProperty, value);
		}

		public static WindowChrome GetInstance(Window window)
		{
			return (WindowChrome)window.GetValue(InstanceProperty);
		}

		private static void InstanceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = d as Window;
			if (window == null) return;

			var oldValue = (WindowChrome)e.OldValue;
			var newValue = (WindowChrome)e.NewValue;

			oldValue?.Detach();
			newValue?.Attach(window);
		}

		#endregion


		/// <summary>
		/// 指定した WPF <see cref="Window"/> に、このクローム UI をアタッチします。
		/// </summary>
		public void Attach(Window window)
		{
			this.Detach();

			this._top.Window.Attach(window);
			this._left.Window.Attach(window);
			this._right.Window.Attach(window);
			this._bottom.Window.Attach(window);
		}

		/// <summary>
		/// 指定したウィンドウに、このクローム UI をアタッチします。
		/// </summary>
		public void Attach(IChromeOwner window)
		{
			this.Detach();

			this._top.Window.Attach(window);
			this._left.Window.Attach(window);
			this._right.Window.Attach(window);
			this._bottom.Window.Attach(window);
		}

		public void Detach()
		{
			this._top.Window.Detach();
			this._left.Window.Detach();
			this._right.Window.Detach();
			this._bottom.Window.Detach();
		}


		private class ChromePart
		{
			private readonly ContentControl _customContentHost;

			public GlowingEdge Edge { get; }

			public ChromeWindow Window { get; }

			public object Content
			{
				get { return this._customContentHost.Content; }
				set
				{
					if (this._customContentHost.Content == value) return;

					this._customContentHost.Content = value;
					this.Window?.Update();
				}
			}

			public ChromePart(ChromeWindow window, Dock position)
			{
				this._customContentHost = new ContentControl();
				this.Edge = new GlowingEdge { Position = position, };
				
				var grid = new Grid();
				grid.Children.Add(this.Edge);
				grid.Children.Add(this._customContentHost);

				this.Window = window;
				this.Window.Content = grid;
			}

		}
	}
}
