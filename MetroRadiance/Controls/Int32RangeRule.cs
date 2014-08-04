using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MetroRadiance.Controls
{
	public class Int32RangeRule : ValidationRule
	{
		public int Max { get; set; }
		public int Min { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			int num;
			try
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				num = int.Parse((string)value);
			}
			catch (Exception)
			{
				return new ValidationResult(false, "数値を入力してください。");
			}

			if (num < this.Min || this.Max < num)
			{
				return new ValidationResult(false, string.Format("{0} ～ {1} の範囲で入力してください。", this.Min, this.Max));
			}

			return new ValidationResult(true, null);
		}
	}
}
