using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MetroRadiance.Controls
{
	public class Int32Rule : ValidationRule
	{
		public int Max { get; set; }
		public int Min { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			try
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				int.Parse((string)value);
			}
			catch (Exception)
			{
				return new ValidationResult(false, "数値を入力してください。");
			}

			return new ValidationResult(true, null);
		}
	}
}
