using System.Collections.Generic;

public static class Hash
{
	public static int SDBMLower(string s)
	{
		if (s != null)
		{
			uint num = 0u;
			for (int i = 0; i < s.Length; i++)
			{
				char c = char.ToLower(s[i]);
				num = c + (num << 6) + (num << 16) - num;
			}
			return (int)num;
		}
		return 0;
	}

	public static int[] SDBMLower(IList<string> strings)
	{
		int[] array = new int[strings.Count];
		for (int i = 0; i < strings.Count; i++)
		{
			array[i] = SDBMLower(strings[i]);
		}
		return array;
	}
}
