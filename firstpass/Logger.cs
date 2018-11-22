using System;
using System.Collections.Generic;
using System.Diagnostics;

public abstract class Logger
{
	public static uint NextIdx;

	protected string name;

	public bool enableConsoleLogging
	{
		get;
		set;
	}

	public bool breakOnLog
	{
		get;
		set;
	}

	public abstract int Count
	{
		get;
	}

	public Logger(string name)
	{
		this.name = name;
	}

	public string GetName()
	{
		return name;
	}

	public void SetName(string name)
	{
		this.name = name;
	}

	public virtual void DebugDisplayLog()
	{
	}
}
public class Logger<EntryType> : Logger
{
	private List<EntryType> entries;

	public Action<EntryType> OnLog;

	public override int Count
	{
		get
		{
			if (entries != null)
			{
				return entries.Count;
			}
			return 0;
		}
	}

	public Logger(string name, int new_max = 35)
		: base(name)
	{
		SetMaxEntries(new_max);
	}

	public IEnumerator<EntryType> GetEnumerator()
	{
		if (entries == null)
		{
			entries = new List<EntryType>();
		}
		return entries.GetEnumerator();
	}

	public void SetMaxEntries(int new_max)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(EntryType entry)
	{
	}
}
