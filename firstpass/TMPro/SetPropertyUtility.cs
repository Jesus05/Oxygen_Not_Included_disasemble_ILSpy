using System;
using UnityEngine;

namespace TMPro
{
	internal static class SetPropertyUtility
	{
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetEquatableStruct<T>(ref T currentValue, T newValue) where T : IEquatable<T>
		{
			if (!((IEquatable<T>)currentValue).Equals(newValue))
			{
				currentValue = newValue;
				return true;
			}
			return false;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (!currentValue.Equals(newValue))
			{
				currentValue = newValue;
				return true;
			}
			return false;
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue != null || newValue != null) && (currentValue == null || !currentValue.Equals(newValue)))
			{
				currentValue = newValue;
				return true;
			}
			return false;
		}
	}
}
