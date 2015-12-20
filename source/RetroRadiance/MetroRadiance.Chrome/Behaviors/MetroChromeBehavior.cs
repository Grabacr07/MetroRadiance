using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using MetroRadiance.Chrome.Internal;

namespace MetroRadiance.Chrome.Behaviors
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class MetroChromeBehavior : Behavior<Window>
	{
		private GlowWindow left, right, top, bottom;

		private bool IsNotNull => this.left != null && this.right != null && this.top != null && this.bottom != null;

		internal Window Window => this.AssociatedObject;

		#region Mode 依存関係プロパティ

		public ChromeMode Mode
		{
			get { return (ChromeMode)this.GetValue(ModeProperty); }
			set { this.SetValue(ModeProperty, value); }
		}
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register("Mode", typeof(ChromeMode), typeof(MetroChromeBehavior), new UIPropertyMetadata(ChromeMode.VisualStudio2013, ModeChangedCallback));

		private static void ModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//var instance = (MetroChromeBehavior)d;
		}

		#endregion

		#region ActiveBrush 依存関係プロパティ

		public Brush ActiveBrush
		{
			get { return (Brush)this.GetValue(ActiveBrushProperty); }
			set { this.SetValue(ActiveBrushProperty, value); }
		}
		public static readonly DependencyProperty ActiveBrushProperty =
			DependencyProperty.Register("ActiveBrush", typeof(Brush), typeof(MetroChromeBehavior), new UIPropertyMetadata(null));

		#endregion

		#region InactiveBrush 依存関係プロパティ

		public Brush InactiveBrush
		{
			get { return (Brush)this.GetValue(InactiveBrushProperty); }
			set { this.SetValue(InactiveBrushProperty, value); }
		}
		public static readonly DependencyProperty InactiveBrushProperty =
			DependencyProperty.Register("InactiveBrush", typeof(Brush), typeof(MetroChromeBehavior), new UIPropertyMetadata(null));

		#endregion


		protected override void OnAttached()
		{
			base.OnAttached();

			this.left = new GlowWindow(this, new GlowWindowProcessorLeft());
			this.right = new GlowWindow(this, new GlowWindowProcessorRight());
			this.top = new GlowWindow(this, new GlowWindowProcessorTop());
			this.bottom = new GlowWindow(this, new GlowWindowProcessorBottom());
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
