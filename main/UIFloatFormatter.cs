using System.Collections.Generic;

public class UIFloatFormatter
{
	private struct Entry
	{
		public string format;

		public string key;

		public float value;

		public string result;
	}

	private int activeStringCount;

	private List<Entry> entries = new List<Entry>();

	public string Format(string format, float value)
	{
		return Replace(format, "{0}", value);
	}

	private string Replace(string format, string key, float value)
	{
		Entry entry = default(Entry);
		if (activeStringCount >= entries.Count)
		{
			entry.format = format;
			entry.key = key;
			entry.value = value;
			entry.result = entry.format.Replace(key, value.ToString());
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
				entry.result = entry.format.Replace(key, value.ToString());
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
