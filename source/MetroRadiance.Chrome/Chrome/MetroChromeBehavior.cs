using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using MetroRadiance.Chrome.Internal;
using MetroRadiance.Chrome.Primitives;

namespace MetroRadiance.Chrome
{
	public class MetroChromeBehavior : Behavior<Window>, IChromeSettings
	{
		private GlowWindow left, right, top, bottom;

		private bool IsNotNull => this.left != null && this.right != null && this.top != null && this.bottom != null;

		#region Mode 依存関係プロパティ

		public ChromeMode Mode
		{
			get { return (ChromeMode)this.GetValue(ModeProperty); }
			set { this.SetValue(ModeProperty, value); }
		}
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register(nameof(Mode), typeof(ChromeMode), typeof(MetroChromeBehavior), new UIPropertyMetadata(ChromeMode.VisualStudio2013, ModeChangedCallback));

		private static void ModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//var instance = (MetroChromeBehavior)d;
		}

		ChromeMode IChromeSettings.ChromeMode => this.Mode;

		#endregion

		#region ActiveBrush 依存関係プロパティ

		public Brush ActiveBrush
		{
			get { return (Brush)this.GetValue(ActiveBrushProperty); }
			set { this.SetValue(ActiveBrushProperty, value); }
		}
		public static readonly DependencyProperty ActiveBrushProperty =
			DependencyProperty.Register(nameof(ActiveBrush), typeof(Brush), typeof(MetroChromeBehavior), new UIPropertyMetadata(null));

		#endregion

		#region InactiveBrush 依存関係プロパティ

		public Brush InactiveBrush
		{
			get { return (Brush)this.GetValue(InactiveBrushProperty); }
			set { this.SetValue(InactiveBrushProperty, value); }
		}
		public static readonly DependencyProperty InactiveBrushProperty =
			DependencyProperty.Register(nameof(InactiveBrush), typeof(Brush), typeof(MetroChromeBehavior), new UIPropertyMetadata(null));

		#endregion


		protected override void OnAttached()
		{
			base.OnAttached();

			var wrapper = WindowWrapper.Create(this.AssociatedObject);
			this.left = new GlowWindow(wrapper, this, new GlowWindowProcessorLeft());
			this.right = new GlowWindow(wrapper, this, new GlowWindowProcessorRight());
			this.top = new GlowWindow(wrapper, this, new GlowWindowProcessorTop());
			this.bottom = new GlowWindow(wrapper, this, new GlowWindowProcessorBottom());
		}

		public void Update()
		{
			if (this.IsNotNull)
			{
				this.left.Update();
				this.right.Update();
				this.top.Update();
				this.bottom.Update();
			}
		}
	}
}
