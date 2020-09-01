using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using MetroRadiance.Properties;

namespace MetroRadiance.UI.Controls
{
	internal static class NumberRuleResults
	{
		internal static readonly ValidationResult SuccessValidationResult = new ValidationResult(true, null);

		internal static readonly ValidationResult FailedEmptyValidationResult = new ValidationResult(false, Resources.NumberRule_ErrorMessage_Empty);

		internal static readonly ValidationResult FailedNonNumberValidationResult = new ValidationResult(false, Resources.NumberRule_ErrorMessage_NonNumber);
	}

	public abstract class NumberRule<T> : ValidationRule where T : struct, IComparable<T>
	{
		private ValidationResult _failedMinValidationResult;
		private ValidationResult _failedMaxValidationResult;

		private T? _Min;
		private T? _Max;

		/// <summary>
		/// 入力に空文字を許可するかどうかを示す値を取得または設定します。
		/// </summary>
		public bool AllowsEmpty { get; set; }

		/// <summary>
		/// 入力可能な最小値を取得または設定します。
		/// </summary>
		/// <value>
		/// 入力可能な最小値。最小値がない場合は null。
		/// </value>
		public T? Min
		{
			get { return this._Min; }
			set
			{
				if (!ReferenceEquals(this._Min, value))
				{
					this._Min = value;
					this._failedMinValidationResult = null;
				}
			}
		}

		/// <summary>
		/// 入力可能な最大値を取得または設定します。
		/// </summary>
		/// <value>
		/// 入力可能な最大値。最大値がない場合は null。
		/// </value>
		public T? Max
		{
			get { return this._Max; }
			set
			{
				if (!ReferenceEquals(this._Max, value))
				{
					this._Max = value;
					this._failedMaxValidationResult = null;
				}
			}
		}

		protected abstract bool TryParse(string s, IFormatProvider provider, out T value);

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var numberAsString = value as string;
			if (string.IsNullOrEmpty(numberAsString))
			{
				return this.AllowsEmpty
					? NumberRuleResults.SuccessValidationResult
					: NumberRuleResults.FailedEmptyValidationResult;
			}

			T number;
			if (!this.TryParse(numberAsString, cultureInfo, out number))
			{
				return NumberRuleResults.FailedNonNumberValidationResult;
			}

			if (this.Min.HasValue && number.CompareTo(this.Min.Value) < 0)
			{
				return this._failedMinValidationResult
					?? (this._failedMinValidationResult = new ValidationResult(false, string.Format(Resources.NumberRule_ErrorMessage_Min, this.Min)));
			}

			if (this.Max.HasValue && number.CompareTo(this.Max.Value) > 0)
			{
				return this._failedMaxValidationResult
					?? (this._failedMaxValidationResult = new ValidationResult(false, string.Format(Resources.NumberRule_ErrorMessage_Max, this.Max)));
			}

			return NumberRuleResults.SuccessValidationResult;
		}
	}

	/// <summary>
	/// 入力された値が有効な <see cref="short"/> 値かどうかを検証します。
	/// </summary>
	public sealed class Int16Rule : NumberRule<short>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out short value)
			=> short.TryParse(s, NumberStyles.Integer, provider, out value);
	}

	/// <summary>
	/// 入力された値が有効な <see cref="ushort"/> 値かどうかを検証します。
	/// </summary>
	public sealed class UInt16Rule : NumberRule<ushort>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out ushort value)
			=> ushort.TryParse(s, NumberStyles.Integer, provider, out value);
	}

	/// <summary>
	/// 入力された値が有効な <see cref="int"/> 値かどうかを検証します。
	/// </summary>
	public sealed class Int32Rule : NumberRule<int>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out int value)
			=> int.TryParse(s, NumberStyles.Integer, provider, out value);
	}

	/// <summary>
	/// 入力された値が有効な <see cref="uint"/> 値かどうかを検証します。
	/// </summary>
	public sealed class UInt32Rule : NumberRule<uint>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out uint value)
			=> uint.TryParse(s, NumberStyles.Integer, provider, out value);
	}

	/// <summary>
	/// 入力された値が有効な <see cref="long"/> 値かどうかを検証します。
	/// </summary>
	public sealed class Int64Rule : NumberRule<long>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out long value)
			=> long.TryParse(s, NumberStyles.Integer, provider, out value);
	}

	/// <summary>
	/// 入力された値が有効な <see cref="ulong"/> 値かどうかを検証します。
	/// </summary>
	public sealed class UInt64Rule : NumberRule<ulong>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out ulong value)
			=> ulong.TryParse(s, NumberStyles.Integer, provider, out value);
	}

	/// <summary>
	/// 入力された値が有効な <see cref="float"/> 値かどうかを検証します。
	/// </summary>
	public sealed class SingleRule : NumberRule<float>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out float value)
			=> float.TryParse(s, NumberStyles.Float, provider, out value);
	}

	/// <summary>
	/// 入力された値が有効な <see cref="double"/> 値かどうかを検証します。
	/// </summary>
	public sealed class DoubleRule : NumberRule<double>
	{
		protected override bool TryParse(string s, IFormatProvider provider, out double value)
			=> double.TryParse(s, NumberStyles.Float, provider, out value);
	}
}
