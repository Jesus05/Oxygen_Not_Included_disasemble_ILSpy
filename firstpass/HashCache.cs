using System.Collections.Generic;

public class HashCache
{
	private Dictionary<int, string> hashes = new Dictionary<int, string>();

	private static HashCache instance;

	public static HashCache Get()
	{
		if (instance == null)
		{
			instance = new HashCache();
		}
		return instance;
	}

	public string Get(int hash)
	{
		string value = string.Empty;
		hashes.TryGetValue(hash, out value);
		return value;
	}

	public string Get(HashedString hash)
	{
		return Get(hash.HashValue);
	}

	public string Get(KAnimHashedString hash)
	{
		return Get(hash.HashValue);
	}

	public HashedString Add(string text)
	{
		HashedString result = new HashedString(text);
		Add(result.HashValue, text);
		return result;
	}

	public void Add(int hash, string text)
	{
		string value = null;
		if (!hashes.TryGetValue(hash, out value))
		{
			hashes[hash] = text.ToLower();
		}
	}
}
