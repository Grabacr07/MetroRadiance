using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MetroRadiance.Controls
{
	public class ProgressRing : Control
	{
		static ProgressRing()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(ProgressRing),
				new FrameworkPropertyMetadata(typeof(ProgressRing)));
		}

		#region IsActive dependency property

		public bool IsActive
		{
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}

		public static readonly DependencyProperty IsActiveProperty =
			DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), new PropertyMetadata(true, IsActiveChangedCallback));

		private static void IsActiveChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
		{
			((ProgressRing)d).SetActivate((bool)args.NewValue);
		}

		#endregion

		#region EllipseDiameter dependency property

		public int EllipseDiameter
		{
			get { return (int)GetValue(EllipseDiameterProperty); }
			set { SetValue(EllipseDiameterProperty, value); }
		}

		public static readonly DependencyProperty EllipseDiameterProperty =
			DependencyProperty.Register("EllipseDiameter", typeof(int), typeof(ProgressRing), new PropertyMetadata(3));

		#endregion

		#region EllipseOffset dependency property

		public Thickness EllipseOffset
		{
			get { return (Thickness)GetValue(EllipseOffsetProperty); }
			set { SetValue(EllipseOffsetProperty, value); }
		}

		public static readonly DependencyProperty EllipseOffsetProperty =
			DependencyProperty.Register("EllipseOffset", typeof(Thickness), typeof(ProgressRing), new PropertyMetadata(new Thickness(0, 7, 0, 0)));

		#endregion

		#region MaxSideLength dependency property

		public int MaxSideLength
		{
			get { return (int)GetValue(MaxSideLengthProperty); }
			set { SetValue(MaxSideLengthProperty, value); }
		}

		public static readonly DependencyProperty MaxSideLengthProperty =
			DependencyProperty.Register("MaxSideLength", typeof(int), typeof(ProgressRing), new PropertyMetadata(20));

		#endregion

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.SetActivate(true);
			this.SetSize(true);
		}

		private void SetActivate(bool active)
		{
			VisualStateManager.GoToState(this, active ? "Active" : "Inactive", true);
		}

		private void SetSize(bool large)
		{
			VisualStateManager.GoToState(this, large ? "Large" : "Small", true);
		}
	}
}
