using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace VS2012LikeWindow2.Views
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow
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

			await this.Countdown();

			this.Visibility = Visibility.Visible;
		}

		private Task Countdown()
		{
			return Task.Factory.StartNew(() => Thread.Sleep(2500));
		}
	}
}
