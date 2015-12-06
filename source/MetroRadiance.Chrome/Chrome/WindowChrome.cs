using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetroRadiance.Chrome.Primitives;

namespace MetroRadiance.Chrome
{
	public class WindowChrome
	{
		private object _top = new GlowingEdge { Position = Dock.Top, };
		private object _left = new GlowingEdge { Position = Dock.Left, };
		private object _right = new GlowingEdge { Position = Dock.Right, };
		private object _bottom = new GlowingEdge { Position = Dock.Bottom, };

		private readonly ChromeWindow _topWindow;
		private readonly ChromeWindow _leftWindow;
		private readonly ChromeWindow _rightWindow;
		private readonly ChromeWindow _bottomWindow;

		private static readonly HashSet<FrameworkElement> _sizingElements = new HashSet<FrameworkElement>();

		public static WindowChrome Default => new WindowChrome();

		#region Content wrappers

		public object Left
		{
			get { return this._left; }
			set
			{
				this._left = value;
				if (this._leftWindow != null)
				{
					this._leftWindow.Content = value;
					this._leftWindow.Update();
				}
			}
		}

		public object Right
		{
			get { return this._right; }
			set
			{
				this._right = value;
				if (this._rightWindow != null)
				{
					this._rightWindow.Content = value;
					this._rightWindow.Update();
				}
			}
		}

		public object Top
		{
			get { return this._top; }
			set
			{
				this._top = value;
				if (this._topWindow != null)
				{
					this._topWindow.Content = value;
					this._topWindow.Update();
				}
			}
		}

		public object Bottom
		{
			get { return this._bottom; }
			set
			{
				this._bottom = value;
				if (this._bottomWindow != null)
				{
					this._bottomWindow.Content = value;
					this._bottomWindow.Update();
				}
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

		public WindowChrome()
		{
			this._topWindow = new TopChromeWindow();
			this._leftWindow = new LeftChromeWindow();
			this._rightWindow = new RightChromeWindow();
			this._bottomWindow = new BottomChromeWindow();
		}

		public void Attach(Window window)
		{
			this.Detach();
			this.AttachContent();

			this._topWindow.Attach(window);
			this._leftWindow.Attach(window);
			this._rightWindow.Attach(window);
			this._bottomWindow.Attach(window);
		}

		public void Attach(ExternalWindow window)
		{
			this.Detach();
			this.AttachContent();

			this._topWindow.Attach(window);
			this._leftWindow.Attach(window);
			this._rightWindow.Attach(window);
			this._bottomWindow.Attach(window);
		}

		private void AttachContent()
		{
			this._topWindow.Content = this._top;
			this._leftWindow.Content = this._left;
			this._rightWindow.Content = this._right;
			this._bottomWindow.Content = this._bottom;
		}

		public void Detach()
		{
			this._topWindow.Detach();
			this._leftWindow.Detach();
			this._rightWindow.Detach();
			this._bottomWindow.Detach();
		}
	}
}
