using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public static class CSVUtil
{
	private static char[] _listSeparators = new char[5]
	{
		',',
		';',
		'+',
		'|',
		'\n'
	};

	private static char[] _enumSeperators = new char[5]
	{
		',',
		';',
		'+',
		'|',
		' '
	};

	public static bool IsValidColumn(string[,] grid, int col)
	{
		return grid[col, 0] != null && grid[col, 0] != "";
	}

	public static void ParseData<T>(object def, string[,] grid, int row)
	{
		int length = grid.GetLength(0);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < length; i++)
		{
			if (IsValidColumn(grid, i))
			{
				try
				{
					string name = grid[i, 0];
					FieldInfo field = typeFromHandle.GetField(name);
					if (field != null)
					{
						string text = grid[i, row];
						if (text != null)
						{
							ParseValue(field, text, def, grid[0, row]);
						}
					}
				}
				catch
				{
				}
			}
		}
	}

	private static void ParseValue(FieldInfo field, string val, object target, string row_name)
	{
		if (field.FieldType.IsEnum)
		{
			object value = null;
			if (val != null && val != "" && EnumTryParse(field.FieldType, val, out value))
			{
				field.SetValue(target, value);
			}
		}
		else if (field.FieldType == typeof(string))
		{
			field.SetValue(target, val);
		}
		else if (field.FieldType == typeof(bool))
		{
			if (val.Contains("1"))
			{
				field.SetValue(target, true);
			}
			else
			{
				field.SetValue(target, val.ToLower() == "true");
			}
		}
		else if (field.FieldType == typeof(float))
		{
			field.SetValue(target, (!(val == "")) ? float.Parse(val) : 0f);
		}
		else if (field.FieldType == typeof(int))
		{
			field.SetValue(target, (!(val == "")) ? int.Parse(val) : 0);
		}
		else if (field.FieldType == typeof(byte))
		{
			field.SetValue(target, byte.Parse(val));
		}
		else if (field.FieldType == typeof(Tag))
		{
			field.SetValue(target, new Tag(val));
		}
		else if (field.FieldType == typeof(CellOffset))
		{
			if (val == null || val == "")
			{
				field.SetValue(target, default(CellOffset));
			}
			else
			{
				string[] array = val.Split(',');
				field.SetValue(target, new CellOffset(int.Parse(array[0]), int.Parse(array[1])));
			}
		}
		else if (field.FieldType == typeof(Vector3))
		{
			if (val == null || val == "")
			{
				field.SetValue(target, Vector3.zero);
			}
			else
			{
				string[] array2 = val.Split(',');
				field.SetValue(target, new Vector3(float.Parse(array2[0]), float.Parse(array2[1]), float.Parse(array2[2])));
			}
		}
		else if (typeof(Array).IsAssignableFrom(field.FieldType))
		{
			string[] array3 = val.Split(_listSeparators);
			Type elementType = field.FieldType.GetElementType();
			Array array4 = Array.CreateInstance(elementType, array3.Length);
			int num = 0;
			for (int i = 0; i < array3.Length; i++)
			{
				string a = array3[i].Trim();
				if (a != "")
				{
					num++;
				}
			}
			array4 = Array.CreateInstance(elementType, num);
			num = 0;
			for (int j = 0; j < array3.Length; j++)
			{
				string text = array3[j].Trim();
				if (text != "")
				{
					object value2 = Convert.ChangeType(text, elementType);
					array4.SetValue(value2, num);
					num++;
				}
			}
			field.SetValue(target, array4);
		}
	}

	public static bool EnumTryParse(Type type, string input, out object value)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (!type.IsEnum)
		{
			throw new ArgumentException(null, "type");
		}
		if (input != null)
		{
			input = input.Trim();
			if (input.Length != 0)
			{
				string[] names = Enum.GetNames(type);
				if (names.Length != 0)
				{
					Type underlyingType = Enum.GetUnderlyingType(type);
					Array values = Enum.GetValues(type);
					if (!type.IsDefined(typeof(FlagsAttribute), true) && input.IndexOfAny(_enumSeperators) < 0)
					{
						return EnumToObject(type, underlyingType, names, values, input, out value);
					}
					string[] array = input.Split(_enumSeperators, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length != 0)
					{
						ulong num = 0uL;
						string[] array2 = array;
						foreach (string text in array2)
						{
							string text2 = text.Trim();
							if (text2.Length != 0)
							{
								if (!EnumToObject(type, underlyingType, names, values, text2, out object value2))
								{
									value = Activator.CreateInstance(type);
									return false;
								}
								ulong num2;
								switch (Convert.GetTypeCode(value2))
								{
								case TypeCode.SByte:
								case TypeCode.Int16:
								case TypeCode.Int32:
								case TypeCode.Int64:
									num2 = (ulong)Convert.ToInt64(value2, CultureInfo.InvariantCulture);
									break;
								default:
									num2 = Convert.ToUInt64(value2, CultureInfo.InvariantCulture);
									break;
								}
								num |= num2;
							}
						}
						value = Enum.ToObject(type, num);
						return true;
					}
					value = Activator.CreateInstance(type);
					return false;
				}
				value = Activator.CreateInstance(type);
				return false;
			}
			value = Activator.CreateInstance(type);
			return false;
		}
		value = Activator.CreateInstance(type);
		return false;
	}

	private static object EnumToObject(Type underlyingType, string input)
	{
		if (underlyingType == typeof(int) && int.TryParse(input, out int result))
		{
			return result;
		}
		if (underlyingType == typeof(uint) && uint.TryParse(input, out uint result2))
		{
			return result2;
		}
		if (underlyingType == typeof(ulong) && ulong.TryParse(input, out ulong result3))
		{
			return result3;
		}
		if (underlyingType == typeof(long) && long.TryParse(input, out long result4))
		{
			return result4;
		}
		if (underlyingType == typeof(short) && short.TryParse(input, out short result5))
		{
			return result5;
		}
		if (underlyingType == typeof(ushort) && ushort.TryParse(input, out ushort result6))
		{
			return result6;
		}
		if (underlyingType == typeof(byte) && byte.TryParse(input, out byte result7))
		{
			return result7;
		}
		if (underlyingType == typeof(sbyte) && sbyte.TryParse(input, out sbyte result8))
		{
			return result8;
		}
		return null;
	}

	private static bool EnumToObject(Type type, Type underlyingType, string[] names, Array values, string input, out object value)
	{
		for (int i = 0; i < names.Length; i++)
		{
			if (string.Compare(names[i], input, StringComparison.OrdinalIgnoreCase) == 0)
			{
				value = values.GetValue(i);
				return true;
			}
		}
		if (!char.IsDigit(input[0]) && input[0] != '-' && input[0] != '+')
		{
			value = Activator.CreateInstance(type);
			return false;
		}
		object obj = EnumToObject(underlyingType, input);
		if (obj != null)
		{
			value = obj;
			return true;
		}
		value = Activator.CreateInstance(type);
		return false;
	}

	public static bool SetValue<T>(T src, ref T dest) where T : IComparable<T>
	{
		bool result = false;
		if (!src.Equals(dest))
		{
			dest = src;
			result = true;
		}
		return result;
	}

	public static bool SetValue<T>(T[] src, ref T[] dest) where T : IComparable<T>, new()
	{
		bool result = false;
		if (dest == null || src.Length != dest.Length)
		{
			result = true;
			dest = new T[src.Length];
			Array.Copy(src, dest, src.Length);
		}
		else
		{
			for (int i = 0; i < src.Length; i++)
			{
				if (((IComparable<T>)src[i]).CompareTo(dest[i]) != 0)
				{
					result = true;
					dest[i] = src[i];
				}
			}
		}
		return result;
	}
}
