using System.Collections.Generic;

public class UIStringFormatter
{
	private struct Entry
	{
		public string format;

		public string key;

		public string value;

		public string result;
	}

	private int activeStringCount;

	private List<Entry> entries = new List<Entry>();

	public string Format(string format, string s0)
	{
		return Replace(format, "{0}", s0);
	}

	public string Format(string format, string s0, string s1)
	{
		return Replace(Replace(format, "{0}", s0), "{1}", s1);
	}

	private string Replace(string format, string key, string value)
	{
		Entry entry = default(Entry);
		if (activeStringCount >= entries.Count)
		{
			entry.format = format;
			entry.key = key;
			entry.value = value;
			entry.result = entry.format.Replace(key, value);
			entries.Add(entry);
		}
		else
		{
			entry = entries[activeStringCount];
			if (entry.format != format || entry.key != key || entry.value != value)
			{
				entry.format = format;
				entry.key = key;
				entry.value = value;
				entry.result = entry.format.Replace(key, value);
				entries[activeStringCount] = entry;
			}
		}
		activeStringCount++;
		return entry.result;
	}

	public void BeginDrawing()
	{
		activeStringCount = 0;
	}

	public void EndDrawing()
	{
	}
}
