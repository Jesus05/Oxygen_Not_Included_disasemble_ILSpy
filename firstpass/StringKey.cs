using System;

[Serializable]
public struct StringKey
{
	public string String;

	public int Hash;

	public StringKey(string str)
	{
		String = str;
		Hash = str.GetHashCode();
	}

	public override string ToString()
	{
		return "S: [" + String + "] H: [" + Hash + "] Value: [" + (string)Strings.Get(this) + "]";
	}

	public bool IsValid()
	{
		return Hash != 0;
	}
}
