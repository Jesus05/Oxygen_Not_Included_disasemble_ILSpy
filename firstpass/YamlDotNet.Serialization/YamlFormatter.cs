using System;
using System.Globalization;

namespace YamlDotNet.Serialization
{
	internal static class YamlFormatter
	{
		public static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo
		{
			CurrencyDecimalSeparator = ".",
			CurrencyGroupSeparator = "_",
			CurrencyGroupSizes = new int[1]
			{
				3
			},
			CurrencySymbol = string.Empty,
			CurrencyDecimalDigits = 99,
			NumberDecimalSeparator = ".",
			NumberGroupSeparator = "_",
			NumberGroupSizes = new int[1]
			{
				3
			},
			NumberDecimalDigits = 99,
			NaNSymbol = ".nan",
			PositiveInfinitySymbol = ".inf",
			NegativeInfinitySymbol = "-.inf"
		};

		public static string FormatNumber(object number)
		{
			return Convert.ToString(number, NumberFormat);
		}

		public static string FormatNumber(double number)
		{
			return number.ToString("G17", NumberFormat);
		}

		public static string FormatNumber(float number)
		{
			return number.ToString("G17", NumberFormat);
		}

		public static string FormatBoolean(object boolean)
		{
			return (!boolean.Equals(true)) ? "false" : "true";
		}

		public static string FormatDateTime(object dateTime)
		{
			return ((DateTime)dateTime).ToString("o", CultureInfo.InvariantCulture);
		}

		public static string FormatTimeSpan(object timeSpan)
		{
			return ((TimeSpan)timeSpan).ToString();
		}
	}
}
