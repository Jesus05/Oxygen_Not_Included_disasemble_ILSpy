using System.Collections.Generic;

public static class Strings
{
	private static StringTable RootTable = new StringTable();

	private static HashSet<string> invalidKeys = new HashSet<string>();

	private static StringEntry GetInvalidString(params StringKey[] keys)
	{
		string text = "MISSING";
		for (int i = 0; i < keys.Length; i++)
		{
			StringKey stringKey = keys[i];
			if (text != string.Empty)
			{
				text += ".";
			}
			text += stringKey.String;
		}
		invalidKeys.Add(text);
		return new StringEntry(text);
	}

	public static StringEntry Get(StringKey key0)
	{
		StringEntry stringEntry = RootTable.Get(key0);
		if (stringEntry == null)
		{
			stringEntry = GetInvalidString(key0);
		}
		return stringEntry;
	}

	public static StringEntry Get(string key)
	{
		StringKey stringKey = new StringKey(key);
		StringEntry stringEntry = RootTable.Get(stringKey);
		if (stringEntry == null)
		{
			stringEntry = GetInvalidString(stringKey);
		}
		return stringEntry;
	}

	public static bool TryGet(StringKey key, out StringEntry result)
	{
		result = RootTable.Get(key);
		return result != null;
	}

	public static bool TryGet(string key, out StringEntry result)
	{
		return TryGet(new StringKey(key), out result);
	}

	public static void Add(params string[] value)
	{
		RootTable.Add(0, value);
	}

	public static void PrintTable()
	{
		RootTable.Print(string.Empty);
	}
}
