using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MetroRadiance.Chrome;
using MetroRadiance.Core.Win32;
using Microsoft.Win32;

namespace VS2012LikeWindow2.Views
{
	partial class MainWindow
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		private void OpenFileDialog(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			if (dialog.ShowDialog() ?? false)
			{

			}
		}

		private async void Hide(object sender, RoutedEventArgs e)
		{
			this.Hide();

			await Task.Delay(2500);

			this.Visibility = Visibility.Visible;
		}

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			this.SetGlowingForActiveWindow();
		}

		private async void SetGlowingForActiveWindow()
		{
			//await Task.Delay(2500);

			//var hWnd = NativeMethods.GetActiveWindow();
			var hWnd = new IntPtr(0x02310A08);
			var chrome = new GlowingChrome(hWnd)
			{
				ActiveBrush = new SolidColorBrush(Colors.DeepSkyBlue),
				InactiveBrush = new SolidColorBrush(Colors.Salmon),
				ChromeMode = ChromeMode.VisualStudio2013,
			};
			chrome.Show();
		}
	}
}
