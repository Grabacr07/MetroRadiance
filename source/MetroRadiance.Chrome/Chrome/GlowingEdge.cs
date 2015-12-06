using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MetroRadiance.Chrome
{
	[TemplatePart(Name = PART_GradientBrush, Type = typeof(GradientBrush))]
	public class GlowingEdge : Control, IValueConverter
	{
		private const string PART_GradientBrush = "PART_GradientBrush";

		static GlowingEdge()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(GlowingEdge),
				new FrameworkPropertyMetadata(typeof(GlowingEdge)));
		}

		#region Position dependency property

		public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
			nameof(Position), typeof(Dock), typeof(GlowingEdge), new PropertyMetadata(default(Dock)));

		public Dock Position
		{
			get { return (Dock)this.GetValue(PositionProperty); }
			set { this.SetValue(PositionProperty, value); }
		}

		#endregion
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var num = 1;
			var brush = this.GetTemplateChild(PART_GradientBrush) as GradientBrush;
			if (brush != null)
			{
				this.SetGradientStops(brush);
				num++;
			}

			while ((brush = this.GetTemplateChild(PART_GradientBrush + num) as GradientBrush) != null)
			{
				this.SetGradientStops(brush);
				num++;
			}
		}

		private void SetGradientStops(GradientBrush brush)
		{
			var stops = new GradientStopCollection();
			var options = new[]
			{
				// Offset, Opacity
				Tuple.Create(1.0, 0.00),
				Tuple.Create(0.6, 0.02),
				Tuple.Create(0.4, 0.08),
				Tuple.Create(0.2, 0.16),
				Tuple.Create(0.1, 0.24),
				Tuple.Create(0.0, 0.32),
			};

			foreach (var tuple in options)
			{
				var stop = new GradientStop { Offset = tuple.Item1, };
				var binding = new Binding(nameof(this.Background))
				{
					Source = this,
					Converter = this,
					ConverterParameter = tuple.Item2,
				};
				BindingOperations.SetBinding(stop, GradientStop.ColorProperty, binding);
				stops.Add(stop);
			}

			brush.GradientStops = stops;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Color color;
			if (value is Color)
			{
				color = (Color)value;
			}
			else if (value is SolidColorBrush)
			{
				color = ((SolidColorBrush)value).Color;
			}
			else
			{
				return Colors.Transparent;
			}

			double opacity;
			if (!double.TryParse(parameter.ToString(), out opacity)) return color;

			color.A = (byte)(color.A * opacity);
			return color;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
