using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VS2012LikeWindow2.Views.MetroChrome
{
	public class Glow : Control
	{
		static Glow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Glow), new FrameworkPropertyMetadata(typeof(Glow)));
		}


		#region GlowColor 依存関係プロパティ

		public Color GlowColor
		{
			get { return (Color)this.GetValue(Glow.GlowColorProperty); }
			set { this.SetValue(Glow.GlowColorProperty, value); }
		}
		public static readonly DependencyProperty GlowColorProperty =
			DependencyProperty.Register("GlowColor", typeof(Color), typeof(Glow), new UIPropertyMetadata(Colors.Transparent));

		#endregion

		#region IsGlow 依存関係プロパティ

		public bool IsGlow
		{
			get { return (bool)this.GetValue(Glow.IsGlowProperty); }
			set { this.SetValue(Glow.IsGlowProperty, value); }
		}
		public static readonly DependencyProperty IsGlowProperty =
			DependencyProperty.Register("IsGlow", typeof(bool), typeof(Glow), new UIPropertyMetadata(true));

		#endregion

		#region Orientation 依存関係プロパティ

		public Orientation Orientation
		{
			get { return (Orientation)this.GetValue(Glow.OrientationProperty); }
			set { this.SetValue(Glow.OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Glow), new UIPropertyMetadata(Orientation.Vertical));

		#endregion

	}
}
