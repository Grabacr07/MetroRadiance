using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MetroRadiance.UI.Controls
{
	public static class ThemeHelper
	{
		private static readonly Dictionary<FrameworkElement, IDisposable> registeredElements = new Dictionary<FrameworkElement, IDisposable>();

		private static void AddResources(FrameworkElement element)
			=> registeredElements[element] = ThemeService.Current.Register(element.Resources);

		private static void RemoveResources(FrameworkElement element)
		{
			IDisposable disposable;
			if (registeredElements.TryGetValue(element, out disposable))
			{
				registeredElements.Remove(element);
				disposable.Dispose();
			}
		}

		#region HasThemeResources 添付プロパティ

		public static readonly DependencyProperty HasThemeResourcesProperty = DependencyProperty.RegisterAttached(
			"HasThemeResources",
			typeof(bool), 
			typeof(ThemeService),
			new PropertyMetadata(false, HasThemeResourcesChangedCallback));

		public static void SetHasThemeResources(FrameworkElement element, bool value)
		{
			element.SetValue(HasThemeResourcesProperty, value);
		}

		public static bool GetHasThemeResources(FrameworkElement element)
		{
			return (bool)element.GetValue(HasThemeResourcesProperty);
		}

		private static void HasThemeResourcesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
		{
			if (DesignerProperties.GetIsInDesignMode(d)) return;

			var element = (FrameworkElement)d;
			var oldValue = (bool)args.OldValue;
			var newValue = (bool)args.NewValue;

			Action logic = () =>
			{
				if (oldValue && !newValue)
				{
					// true -> false
					element.Unloaded -= ElementUnloadedCallback;
					RemoveResources(element);
				}
				else if (!oldValue && newValue)
				{
					// false -> true
					FrameworkElementLoadedWeakEventManager.AddHandler(element, ElementLoadedCallback);
				}
			};

			if (element.IsInitialized)
			{
				logic();
			}
			else
			{
				EventHandler handler = null;
				handler = (sender, e) =>
				{
					element.Initialized -= handler;
					logic();
				};
				element.Initialized += handler;
			}
		}

		private static void ElementLoadedCallback(object sender, RoutedEventArgs e)
		{
			var element = (FrameworkElement)sender;
			AddResources(element);
			element.Unloaded += ElementUnloadedCallback;
		}

		private static void ElementUnloadedCallback(object sender, RoutedEventArgs e)
		{
			var element = (FrameworkElement)sender;
			element.Unloaded -= ElementUnloadedCallback;
			RemoveResources(element);
		}

		#endregion

		private sealed class FrameworkElementLoadedWeakEventManager : WeakEventManager
		{
			/// <summary>
			/// Add a handler for the given source's event.
			/// </summary>
			public static void AddHandler(FrameworkElement source, EventHandler<RoutedEventArgs> handler)
			{
				if (source == null) throw new ArgumentNullException(nameof(source));
				if (handler == null) throw new ArgumentNullException(nameof(handler));

				CurrentManager.ProtectedAddHandler(source, handler);
			}

			/// <summary>
			/// Remove a handler for the given source's event.
			/// </summary>
			public static void RemoveHandler(FrameworkElement source, EventHandler<RoutedEventArgs> handler)
			{
				if (source == null) throw new ArgumentNullException(nameof(source));
				if (handler == null) throw new ArgumentNullException(nameof(handler));

				CurrentManager.ProtectedRemoveHandler(source, handler);
			}

			private static FrameworkElementLoadedWeakEventManager CurrentManager
			{
				get
				{
					var managerType = typeof(FrameworkElementLoadedWeakEventManager);
					var manager = (FrameworkElementLoadedWeakEventManager)GetCurrentManager(managerType);
					if (manager == null)
					{
						manager = new FrameworkElementLoadedWeakEventManager();
						SetCurrentManager(managerType, manager);
					}
					return manager;
				}
			}

			private FrameworkElementLoadedWeakEventManager()
			{ }

			protected override ListenerList NewListenerList()
			{
				return new ListenerList<RoutedEventArgs>();
			}

			protected override void StartListening(object source)
			{
				var element = (FrameworkElement)source;
				element.Loaded += this.OnElementLoaded;
			}

			protected override void StopListening(object source)
			{
				var element = (FrameworkElement)source;
				element.Loaded -= this.OnElementLoaded;
			}

			private void OnElementLoaded(object sender, RoutedEventArgs e)
			{
				this.DeliverEvent(sender, e);
			}
		}
	}
}
