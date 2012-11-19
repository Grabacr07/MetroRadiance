using System.Windows.Media;

namespace VS2012LikeWindow2.Views
{
	enum Accent
	{
		Purple, Blue, Orange,
	}

	class ThemeService
	{
		private App app;

		public ThemeService(App app)
		{
			this.app = app;
		}

		public void Change(Accent accent)
		{
			SolidColorBrush brush = null;
			switch (accent)
			{
				case Accent.Purple:
					brush = new SolidColorBrush(Color.FromRgb(104, 33, 122));
					break;
				case Accent.Blue:
					brush = new SolidColorBrush(Color.FromRgb(0, 122, 204));
					break;
				case Accent.Orange:
					brush = new SolidColorBrush(Color.FromRgb(202, 81, 0));
					break;
			}

			app.Resources["AccentBrushKey"] = brush;
		}
	}
}
