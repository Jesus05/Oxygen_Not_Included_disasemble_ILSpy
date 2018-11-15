using System.Collections.Generic;

public static class DebugHashes
{
	private static Dictionary<int, string> hashMap = new Dictionary<int, string>();

	public static void Add(string name)
	{
		int key = Hash.SDBMLower(name);
		hashMap[key] = name;
	}

	public static string GetName(int hash)
	{
		if (hashMap.ContainsKey(hash))
		{
			return hashMap[hash];
		}
		return "Unknown HASH [0x" + hash.ToString("X") + "]";
	}
}
