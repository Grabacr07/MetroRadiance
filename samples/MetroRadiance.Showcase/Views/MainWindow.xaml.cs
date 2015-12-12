using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MetroRadiance.Chrome;
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
			////await Task.Delay(2500);

			////var hWnd = NativeMethods.GetActiveWindow();
			//var hWnd = new IntPtr(0x015E0D0C);
			//var external = new ExternalWindow(hWnd);
			//var chrome = new WindowChrome();
			//chrome.Attach(external);
		}
	}
}
