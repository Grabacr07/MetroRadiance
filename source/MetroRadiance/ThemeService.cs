using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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

	public class ThemeService : INotifyPropertyChanged
	{
		#region singleton members

		public static ThemeService Current { get; } = new ThemeService();

		#endregion

		private static readonly UriTemplate themeTemplate = new UriTemplate(@"Themes/{theme}.xaml");
		private static readonly UriTemplate accentTemplate = new UriTemplate(@"Themes/Accents/{accent}.xaml");
		private static readonly Uri templateBaseUri = new Uri(@"pack://application:,,,/MetroRadiance;component");

		private static readonly IReadOnlyDictionary<Accent, ResourceDictionary> accentDictionaries = new Dictionary<Accent, ResourceDictionary>
		{
			{ Accent.Blue, new ResourceDictionary { Source = CreateAccentResourceUri(Accent.Blue), } },
			{ Accent.Purple, new ResourceDictionary { Source = CreateAccentResourceUri(Accent.Purple), } },
			{ Accent.Orange, new ResourceDictionary { Source = CreateAccentResourceUri(Accent.Orange), } },
		};

		private bool initialized;
		private Dispatcher dispatcher;
		private ResourceDictionary currentAccentDictionary;

		private readonly List<ResourceDictionary> themeResources = new List<ResourceDictionary>();
		private readonly List<ResourceDictionary> accentResources = new List<ResourceDictionary>();

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
			this.InitializeCore(app, theme, accentDictionaries[accent]);

			this.Theme = theme;
			this.Accent = accent;
		}

		public void Initialize(Application app, Theme theme, ResourceDictionary accent)
		{
			this.InitializeCore(app, theme, this.currentAccentDictionary);

			this.Theme = theme;
			this.Accent = Accent.Original;
		}

		private void InitializeCore(Application app, Theme theme, ResourceDictionary accent)
		{
			this.dispatcher = app.Dispatcher;

			this.currentAccentDictionary = accent;
			this.Register(app.Resources, theme, this.currentAccentDictionary);

			this.initialized = true;
		}

		public IDisposable Register(ResourceDictionary rd)
		{
			return this.Register(rd, this.Theme, this.currentAccentDictionary);
		}

		internal IDisposable Register(ResourceDictionary rd, Theme theme, ResourceDictionary accentDic)
		{
			var allDictionaries = EnumerateDictionaries(rd).ToArray();

			var themeDic = new ResourceDictionary { Source = CreateThemeResourceUri(theme), };
			var targetThemeDic = allDictionaries.FirstOrDefault(x => CheckThemeResourceUri(x.Source));
			if (targetThemeDic == null)
			{
				targetThemeDic = themeDic;
				rd.MergedDictionaries.Add(targetThemeDic);
			}
			else
			{
				foreach (var key in themeDic.Keys.OfType<string>().Where(x => targetThemeDic.Contains(x)))
				{
					targetThemeDic[key] = themeDic[key];
				}
			}
			this.themeResources.Add(targetThemeDic);

			var targetAccentDic = allDictionaries.FirstOrDefault(x => CheckAccentResourceUri(x.Source));
			if (targetAccentDic == null)
			{
				targetAccentDic = new ResourceDictionary { Source = accentDic.Source };
				rd.MergedDictionaries.Add(targetAccentDic);
			}
			else
			{
				foreach (var key in accentDic.Keys.OfType<string>().Where(x => targetAccentDic.Contains(x)))
				{
					targetAccentDic[key] = accentDic[key];
				}
			}
			this.accentResources.Add(targetAccentDic);

			// Unregister したいときは戻り値の IDisposable を Dispose() してほしい
			return Disposable.Create(() =>
			{
				this.themeResources.Remove(targetThemeDic);
				this.accentResources.Remove(targetAccentDic);
			});
		}

		public void ChangeTheme(Theme theme)
		{
			if (!this.initialized || this.Theme == theme) return;

			this.dispatcher.Invoke(() =>
			{
				var uri = CreateThemeResourceUri(theme);
				var dic = new ResourceDictionary { Source = uri, };

				foreach (var key in dic.Keys.OfType<string>())
				{
					foreach (var resource in this.themeResources.Where(x => x.Contains(key)))
					{
						resource[key] = dic[key];
					}
				}
			});

			this.Theme = theme;
		}

		public void ChangeAccent(Accent accent)
		{
			if (!this.initialized || this.Accent == accent) return;

			this.dispatcher.Invoke(() => this.ChangeAccentCore(accentDictionaries[accent]));
			this.Accent = accent;
		}

		public void ChangeAccent(ResourceDictionary accent)
		{
			if (!this.initialized) return;

			this.dispatcher.Invoke(() => this.ChangeAccentCore(accent));
			this.Accent = Accent.Original;
		}

		private void ChangeAccentCore(ResourceDictionary dic)
		{
			this.currentAccentDictionary = dic;
			foreach (var key in dic.Keys.OfType<string>())
			{
				foreach (var resource in this.accentResources.Where(x => x.Contains(key)))
				{
					resource[key] = dic[key];
				}
			}
		}

		private static Uri CreateThemeResourceUri(Theme theme)
		{
			var param = new Dictionary<string, string>
			{
				{ "theme", theme.ToString() },
			};
			return themeTemplate.BindByName(templateBaseUri, param);
		}

		private static Uri CreateAccentResourceUri(Accent accent)
		{
			var param = new Dictionary<string, string>
			{
				{ "accent", accent.ToString() },
			};
			return accentTemplate.BindByName(templateBaseUri, param);
		}

		/// <summary>
		/// 指定した <see cref="Uri"/> がテーマのリソースを指す URI かどうかをチェックします。
		/// </summary>
		/// <returns><paramref name="uri"/> がテーマのリソースを指す URI の場合は true、それ以外の場合は false。</returns>
		private static bool CheckThemeResourceUri(Uri uri)
		{
			return themeTemplate.Match(templateBaseUri, uri) != null;
		}

		/// <summary>
		/// 指定した <see cref="Uri"/> がアクセント カラーのリソースを指す URI かどうかをチェックします。
		/// </summary>
		/// <returns><paramref name="uri"/> がアクセント カラーのリソースを指す URI の場合は true、それ以外の場合は false。</returns>
		private static bool CheckAccentResourceUri(Uri uri)
		{
			return accentTemplate.Match(templateBaseUri, uri) != null;
		}

		private static IEnumerable<ResourceDictionary> EnumerateDictionaries(ResourceDictionary dictionary)
		{
			if (dictionary.MergedDictionaries.Count == 0)
			{
				yield break;
			}

			foreach (var mergedDictionary in dictionary.MergedDictionaries)
			{
				yield return mergedDictionary;

				foreach (var other in EnumerateDictionaries(mergedDictionary))
				{
					yield return other;
				}
			}
		}


		#region INotifyPropertyChanged 

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
