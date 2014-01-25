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
		Original,
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
		private ResourceDictionary appTheme;
		private ResourceDictionary appAccent;
		private bool initialized;

		#region Theme 変更通知プロパティ

		private Theme _Theme;

		public Theme Theme
		{
			get { return this._Theme; }
			private set
			{
				if (this._Theme != value)
				{
					this._Theme = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Accent 変更通知プロパティ

		private Accent _Accent;

		public Accent Accent
		{
			get { return this._Accent; }
			private set
			{
				if (this._Accent != value)
				{
					this._Accent = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		private ThemeService() { }

		public void Initialize(Application app, Theme theme, Accent accent)
		{
			this.dispatcher = app.Dispatcher;

			var accentUri = CreateAccentResourceUri(accent);
			if (accentUri != null)
			{
				var accentResource = app.Resources.MergedDictionaries.FirstOrDefault(x => Uri.Compare(
					x.Source,
					accentUri,
					UriComponents.AbsoluteUri,
					UriFormat.Unescaped,
					StringComparison.InvariantCultureIgnoreCase) == 0);

				if (accentResource == null)
				{
					accentResource = new ResourceDictionary { Source = accentUri };
					app.Resources.MergedDictionaries.Add(accentResource);
				}

				this.Initialize(app, theme, accentResource);
			}
		}

		public void Initialize(Application app, Theme theme, ResourceDictionary accent)
		{
			this.dispatcher = app.Dispatcher;

			var themeUri = CreateThemeResourceUri(theme);
			if (themeUri != null)
			{
				this.appTheme = app.Resources.MergedDictionaries.FirstOrDefault(x => Uri.Compare(
					x.Source,
					themeUri,
					UriComponents.AbsoluteUri,
					UriFormat.Unescaped,
					StringComparison.InvariantCultureIgnoreCase) == 0);

				if (this.appTheme == null)
				{
					this.appTheme = new ResourceDictionary { Source = themeUri };
					app.Resources.MergedDictionaries.Add(this.appTheme);
				}
			}

			this.appAccent = accent;

			this.initialized = (this.appTheme != null && this.appAccent != null);
		}


		public void ChangeTheme(Theme theme)
		{
			if (this.initialized && this.Theme != theme)
			{
				this.dispatcher.Invoke(() =>
				{
					var uri = CreateThemeResourceUri(theme);
					var dic = new ResourceDictionary { Source = uri };

					dic.Keys.OfType<string>()
						.Where(key => this.appTheme.Contains(key))
						.ForEach(key => this.appTheme[key] = dic[key]);
				});

				this.Theme = theme;
			}
		}

		public void ChangeAccent(Accent accent)
		{
			var uri = CreateAccentResourceUri(accent);
			if (uri != null)
			{
				this.dispatcher.Invoke(() =>
				{
					var resource = new ResourceDictionary { Source = uri };
					this.ChangeAccentCore(resource);
				});
				this.Accent = accent;
			}
		}

		public void ChangeAccent(ResourceDictionary resource)
		{
			this.dispatcher.Invoke(() => this.ChangeAccentCore(resource));
			this.Accent = Accent.Original;
		}

		private void ChangeAccentCore(ResourceDictionary resource)
		{
			resource.Keys.OfType<string>()
				.Where(key => this.appAccent.Contains(key))
				.ForEach(key => this.appAccent[key] = resource[key]);
		}



		private static Uri CreateThemeResourceUri(Theme theme)
		{
			var uri = string.Format(@"pack://application:,,,/MetroRadiance;component/Themes/{0}.xaml", theme);
			Uri result;
			return Uri.TryCreate(uri, UriKind.Absolute, out result) ? result : null;
		}

		private static Uri CreateAccentResourceUri(Accent accent)
		{
			var uri = string.Format(@"pack://application:,,,/MetroRadiance;component/Themes/Accents/{0}.xaml", accent);
			Uri result;
			return Uri.TryCreate(uri, UriKind.Absolute, out result) ? result : null;
		}

	}
}
