using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MetroRadiance.Core;
using MetroRadiance.Internal;

namespace MetroRadiance
{
	public enum Theme
	{
		Dark, 
		Light,
	}

	public enum Accent
	{
		Purple,
		Blue,
		Orange,
	}

	public class ThemeService : Notificator
	{
		#region singleton members

		private static readonly ThemeService current = new ThemeService();

		public static ThemeService Current
		{
			get { return current; }
		}

		#endregion

		private Dispatcher dispatcher;
		private ResourceDictionary theme;
		private ResourceDictionary accent;
		private bool initialized;

		#region Theme 変更通知プロパティ

		private Theme _Theme = Theme.Dark;

		public Theme Theme
		{
			get { return this._Theme; }
			set
			{
				if (this.initialized && this._Theme != value)
				{
					this.dispatcher.Invoke(() =>
					{
						var uri = new Uri(string.Format(@"pack://application:,,,/MetroRadiance;component/Themes/{0}.xaml", value));
						var dic = new ResourceDictionary { Source = uri };

						dic.Keys.OfType<string>()
							.Where(key => this.theme.Contains(key))
							.ForEach(key => this.theme[key] = dic[key]);
					});

					this._Theme = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Accent 変更通知プロパティ

		private Accent _Accent = (Accent)(-1);

		public Accent Accent
		{
			get { return this._Accent; }
			set
			{
				if (this.initialized && this._Accent != value)
				{
					this.dispatcher.Invoke(() =>
					{
						var uri = new Uri(string.Format(@"pack://application:,,,/MetroRadiance;component/Themes/Accents/{0}.xaml", value));
						var dic = new ResourceDictionary { Source = uri };

						dic.Keys.OfType<string>()
							.Where(key => this.accent.Contains(key))
							.ForEach(key => this.accent[key] = dic[key]);
					});

					this._Accent = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private ThemeService() { }

		public void Initialize(Application app)
		{
			this.dispatcher = app.Dispatcher;

			this.theme = app.Resources.MergedDictionaries.FirstOrDefault(x => Uri.Compare(
				x.Source, new Uri(@"pack://application:,,,/MetroRadiance;component/Themes/Dark.xaml", UriKind.Absolute),
				UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) == 0);

			this.accent = app.Resources.MergedDictionaries.FirstOrDefault(x => Uri.Compare(
				x.Source, new Uri(@"pack://application:,,,/MetroRadiance;component/Themes/Accents/Blue.xaml", UriKind.Absolute),
				UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) == 0);

			this.initialized = (this.theme != null && this.accent != null);
		}
	}
}
