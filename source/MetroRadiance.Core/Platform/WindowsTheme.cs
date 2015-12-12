using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using MetroRadiance.Interop.Win32;

namespace MetroRadiance.Platform
{
	/// <summary>
	/// Windows OS のテーマ機能へアクセスできるようにします。
	/// </summary>
	public static class WindowsTheme
	{
		private static event EventHandler<Color> _accentColorChanged;
		private static readonly HashSet<EventHandler<Color>> _handlers = new HashSet<EventHandler<Color>>();
		private static ListenerWindow _listenerWindow;

		/// <summary>
		/// Windows のアクセント カラーが変更されると発生します。
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static event EventHandler<Color> AccentColorChanged
		{
			add { AddListener(value); }
			remove { RemoveListener(value); }
		}

		/// <summary>
		/// 現在の Windows のアクセント カラーを取得します。
		/// </summary>
		/// <returns></returns>
		public static Color GetAccentColor()
		{
			int color;
			bool opaque;

			Dwmapi.DwmGetColorizationColor(out color, out opaque);

			return GetColorFromInt64(color);
		}

		public static Color GetColorFromInt64(long color)
		{
			return Color.FromArgb((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)color);
		}

		/// <summary>
		/// Windows のアクセント カラーが変更されたときに通知を受け取るメソッドを登録します。
		/// </summary>
		/// <param name="callback">Windows のアクセント カラーが変更されたときに通知を受け取るメソッド。</param>
		/// <returns>通知の購読を解除するときに使用する <see cref="IDisposable"/> オブジェクト。</returns>
		public static IDisposable RegisterAccentColorListener(Action<Color> callback)
		{
			EventHandler<Color> handler = (sender, color) => callback?.Invoke(color);
			AccentColorChanged += handler;

			return new AnonymousDisposable(() => AccentColorChanged -= handler);
		}

		private static void AddListener(EventHandler<Color> listener)
		{
			if (_handlers.Add(listener))
			{
				_accentColorChanged += listener;

				if (_listenerWindow == null)
				{
					_listenerWindow = new ListenerWindow(RaiseAccentColorChanged);
					_listenerWindow.Show();
				}
			}
		}

		private static void RemoveListener(EventHandler<Color> listener)
		{
			if (_handlers.Remove(listener))
			{
				_accentColorChanged -= listener;

				if (_handlers.Count == 0)
				{
					_listenerWindow?.Close();
					_listenerWindow = null;
				}
			}
		}

		private static void RaiseAccentColorChanged(Color color)
		{
			_accentColorChanged?.Invoke(typeof(WindowsTheme), color);
		}


		private class ListenerWindow : TransparentWindow
		{
			private readonly Action<Color> _callback;

			public ListenerWindow(Action<Color> callback)
			{
				this.Name = "Windows message listener window";
				this._callback = callback;
			}

			protected override IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
			{
				if (msg == (int)WindowsMessages.WM_DWMCOLORIZATIONCOLORCHANGED)
				{
					var color = GetColorFromInt64((long)wParam);
					this._callback(color);
				}

				return base.WindowProc(hwnd, msg, wParam, lParam, ref handled);
			}
		}

		private class AnonymousDisposable : IDisposable
		{
			private readonly Action _disposeAction;

			public AnonymousDisposable(Action disposeAction)
			{
				this._disposeAction = disposeAction;
			}

			public void Dispose()
			{
				this._disposeAction?.Invoke();
			}
		}
	}
}
