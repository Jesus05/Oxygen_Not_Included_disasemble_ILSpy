public class StringEntry
{
	public string String;

	public StringEntry(string str)
	{
		String = str;
	}

	public override string ToString()
	{
		return String;
	}

	public static implicit operator string(StringEntry entry)
	{
		return entry.String;
	}
}
