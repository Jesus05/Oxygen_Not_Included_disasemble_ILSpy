using System.Collections.Generic;

public static class StringFormatter
{
	private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> cachedReplacements = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

	private static Dictionary<string, Dictionary<string, string>> cachedCombines = new Dictionary<string, Dictionary<string, string>>();

	private static Dictionary<HashedString, string> cachedToUppers = new Dictionary<HashedString, string>();

	public static string Replace(string format, string token, string replacement)
	{
		Dictionary<string, Dictionary<string, string>> value = null;
		if (!cachedReplacements.TryGetValue(format, out value))
		{
			value = new Dictionary<string, Dictionary<string, string>>();
			cachedReplacements[format] = value;
		}
		Dictionary<string, string> value2 = null;
		if (!value.TryGetValue(token, out value2))
		{
			value2 = (value[token] = new Dictionary<string, string>());
		}
		string value3 = null;
		if (!value2.TryGetValue(replacement, out value3))
		{
			value3 = (value2[replacement] = format.Replace(token, replacement));
		}
		return value3;
	}

	public static string Combine(string a, string b, string c)
	{
		return Combine(Combine(a, b), c);
	}

	public static string Combine(string a, string b, string c, string d)
	{
		return Combine(Combine(Combine(a, b), c), d);
	}

	public static string Combine(string a, string b)
	{
		Dictionary<string, string> value = null;
		if (!cachedCombines.TryGetValue(a, out value))
		{
			value = new Dictionary<string, string>();
			cachedCombines[a] = value;
		}
		string value2 = null;
		if (!value.TryGetValue(b, out value2))
		{
			value2 = (value[b] = a + b);
		}
		return value2;
	}

	public static string ToUpper(string a)
	{
		HashedString key = a;
		string value = null;
		if (!cachedToUppers.TryGetValue(key, out value))
		{
			value = a.ToUpper();
			cachedToUppers[key] = value;
		}
		return value;
	}
}
