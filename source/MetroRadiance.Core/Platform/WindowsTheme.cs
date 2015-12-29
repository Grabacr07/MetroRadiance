using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using MetroRadiance.Interop.Win32;
using MetroRadiance.Media;
using MetroRadiance.Utilities;
using Microsoft.Win32;

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
		private static bool? _isDarkTheme;

		/// <summary>
		/// 現在実行されている Windows 10 が Dark Theme 設定になっているかどうかを示す値を取得します。
		/// </summary>
		public static bool IsDarkTheme
		{
			get
			{
				if (_isDarkTheme == null)
				{
					const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
					const string valueName = "AppsUseLightTheme";

					_isDarkTheme = Registry.GetValue(keyName, valueName, null) as int? == 0;
				}

				return _isDarkTheme.Value;
			}
		}

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

			return ColorHelper.GetColorFromInt64(color);
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

			return Disposable.Create(() => AccentColorChanged -= handler);
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

			protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
			{
				if (msg == (int)WindowsMessages.WM_DWMCOLORIZATIONCOLORCHANGED)
				{
					var color = ColorHelper.GetColorFromInt64((long)wParam);
					this._callback(color);
				}

				return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
			}
		}
	}
}
