using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineDebuggerSettings : ScriptableObject
{
	[Serializable]
	public class Entry
	{
		public Type type;

		public string typeName;

		public bool breakOnGoTo;

		public bool enableConsoleLogging;

		public bool saveHistory;

		public Entry(Type type)
		{
			typeName = type.FullName;
			this.type = type;
		}

		public static void ShowHeader()
		{
		}

		public void ShowEditor()
		{
		}
	}

	public List<Entry> entries = new List<Entry>();

	private static StateMachineDebuggerSettings _Instance;

	public IEnumerator<Entry> GetEnumerator()
	{
		return entries.GetEnumerator();
	}

	public static StateMachineDebuggerSettings Get()
	{
		if ((UnityEngine.Object)_Instance == (UnityEngine.Object)null)
		{
			_Instance = Resources.Load<StateMachineDebuggerSettings>("StateMachineDebuggerSettings");
			_Instance.Initialize();
		}
		return _Instance;
	}

	private void Initialize()
	{
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			if (typeof(StateMachine).IsAssignableFrom(currentDomainType))
			{
				CreateEntry(currentDomainType);
			}
		}
		entries.RemoveAll((Entry x) => x.type == null);
	}

	public Entry CreateEntry(Type type)
	{
		foreach (Entry entry2 in entries)
		{
			if (type.FullName == entry2.typeName)
			{
				entry2.type = type;
				return entry2;
			}
		}
		Entry entry = new Entry(type);
		entries.Add(entry);
		return entry;
	}

	public void Clear()
	{
		entries.Clear();
		Initialize();
	}
}
