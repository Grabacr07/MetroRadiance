using Livet;
using MetroRadiance.Chrome;
using MetroRadiance.Interop.Win32;
using MetroRadiance.Platform;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MetroRadiance.Showcase.UI
{
	public partial class ExternalChromeSamples
	{
		private WindowChrome _metroChrome;

		public ExternalChromeSamples()
		{
			this.InitializeComponent();

			this.DataContext = GetWindowViewModels();
		}

		private void HandleMetroChromeClicked(object sender, RoutedEventArgs e)
		{
			var viewModel = (WindowViewModel)this.WindowsListView.SelectedItem;
			if (viewModel == null) return;

			var externalWindow = new ExternalWindow(viewModel.Handle);
			if (this._metroChrome == null)
			{
				this._metroChrome = new WindowChrome();
			}
			this._metroChrome.Attach(externalWindow);
		}

		private void HandleRefreshClicked(object sender, RoutedEventArgs e)
		{
			this.DataContext = GetWindowViewModels();
		}

		private static WindowViewModel[] GetWindowViewModels()
		{
			var currentProcess = Process.GetCurrentProcess();
			var windows = NativeMethods.GetWindows()
				.Where(IsValidWindow)
				.Select(hWnd =>
				{
					uint pid;
					NativeMethods.GetWindowThreadProcessId(hWnd, out pid);

					var process = Process.GetProcessById((int)pid);
					return Tuple.Create(process, hWnd);
				})
				.Where(tuple => tuple.Item1.Id != currentProcess.Id);
			var viewModels = windows.Select(tuple => new WindowViewModel(tuple.Item1, tuple.Item2));
			return viewModels.ToArray();
		}

		private static bool IsValidWindow(IntPtr hWnd)
		{
			// ウィンドウハンドルであるか
			if (!User32.IsWindow(hWnd)) return false;

			// 見えるか
			if (!User32.IsWindowVisible(hWnd)) return false;

			// シェル ウィンドウでないか
			if (NativeMethods.GetShellWindow() == hWnd) return false;

			// 親ウィンドウであるか
			if (NativeMethods.GetAncestor(hWnd, 2 /* GA_ROOT */) != hWnd) return false;

			// 見えないウィンドウでないか
			if (Dwmapi.DwmGetCloaked(hWnd)) return false;

			return true;
		}

		internal static class NativeMethods
		{
			public delegate bool EnumWindowsCallback(IntPtr hWnd, int lParam);

			[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool EnumWindows(EnumWindowsCallback lpEnumFunc, IntPtr lParam);

			public static IntPtr[] GetWindows()
			{
				var windows = new Collection<IntPtr>();
				var ret = EnumWindows((hWnd, _) =>
				{
					windows.Add(hWnd);
					return true;
				}, IntPtr.Zero);
				if (!ret) throw new Win32Exception(Marshal.GetLastWin32Error());
				return windows.ToArray();
			}

			[DllImport("user32.dll", SetLastError = true)]
			public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

			[DllImport("user32.dll", ExactSpelling = true)]
			public static extern IntPtr GetShellWindow();

			[DllImport("user32.dll", ExactSpelling = true)]
			public static extern IntPtr GetAncestor(IntPtr hwnd, uint flags);
		}
	}

	public class WindowViewModel : ViewModel
	{
		public int ProcessId { get; }

		public IntPtr Handle { get; }

		#region Title 変更通知プロパティ

		private string _Title;

		public string Title
		{
			get { return this._Title; }
			set
			{
				if (this._Title != value)
				{
					this._Title = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Icon 変更通知プロパティ

		private ImageSource _Icon;

		public ImageSource Icon
		{
			get { return this._Icon; }
			set
			{
				if (this._Icon != value)
				{
					this._Icon = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public WindowViewModel(Process process, IntPtr hWnd)
		{
			this.ProcessId = process.Id;
			this.Handle = hWnd;
			this.Update();
		}

		public void Update()
		{
			try
			{
				var title = NativeMethods.GetWindowText(this.Handle);
				if (string.IsNullOrWhiteSpace(title))
				{
					title = "(No title)";
				}
				this.Title = title;

				var hIcon = User32.GetClassLong(this.Handle, ClassLongPtrIndex.GCLP_HICON);
				this.Icon = hIcon != IntPtr.Zero
					? Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
					: null;
			}
			catch (Win32Exception) { }
		}

		private static class NativeMethods
		{
			[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			private static extern int GetWindowTextLengthW(IntPtr hWnd);

			[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			private static extern int GetWindowTextW(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

			public static string GetWindowText(IntPtr hWnd)
			{
				var length = GetWindowTextLengthW(hWnd);
				if (length == 0)
				{
					var err = Marshal.GetLastWin32Error();
					if (err != 0)
					{
						throw new Win32Exception(err);
					}
					else
					{
						return string.Empty;
					}
				}

				var builder = new StringBuilder(length);
				var ret = GetWindowTextW(hWnd, builder, length + 1);
				if (ret == 0) throw new Win32Exception(Marshal.GetLastWin32Error());

				return builder.ToString();
			}
		}
	}
}
