using System.Collections.Generic;

public class StringTable
{
	private Dictionary<int, string> KeyNames = new Dictionary<int, string>();

	private Dictionary<int, StringTable> SubTables = new Dictionary<int, StringTable>();

	private Dictionary<int, StringEntry> Entries = new Dictionary<int, StringEntry>();

	public StringEntry Get(StringKey key0)
	{
		int hash = key0.Hash;
		StringEntry value = null;
		Entries.TryGetValue(hash, out value);
		return value;
	}

	public void Add(int idx, string[] value)
	{
		string text = value[idx];
		int hashCode = text.GetHashCode();
		KeyNames[hashCode] = text;
		if (idx == value.Length - 2)
		{
			StringEntry value2 = new StringEntry(value[idx + 1]);
			Entries[hashCode] = value2;
		}
		else
		{
			StringTable value3 = null;
			if (!SubTables.TryGetValue(hashCode, out value3))
			{
				value3 = new StringTable();
				SubTables[hashCode] = value3;
			}
			value3.Add(idx + 1, value);
		}
	}

	public void Print(string parent_path)
	{
		foreach (KeyValuePair<int, StringEntry> entry in Entries)
		{
			Debug.Log(parent_path + "." + KeyNames[entry.Key] + "." + entry.Value.String, null);
		}
		string text = parent_path;
		if (text != string.Empty)
		{
			text += ".";
		}
		foreach (KeyValuePair<int, StringTable> subTable in SubTables)
		{
			subTable.Value.Print(text + KeyNames[subTable.Key]);
		}
	}
}
