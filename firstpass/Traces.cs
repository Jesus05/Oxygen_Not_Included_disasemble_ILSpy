using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Traces : KMonoBehaviour
{
	[Serializable]
	public class Entry
	{
		public string Name;

		public StackTrace StackTrace;

		public bool Foldout;
	}

	public List<Entry> DestroyTraces = new List<Entry>();

	public static Traces Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Instance = null;
	}

	public void TraceDestroy(GameObject go, StackTrace stack_trace)
	{
		if (DestroyTraces.Count > 99)
		{
			DestroyTraces.RemoveAt(0);
		}
		Entry entry = new Entry();
		entry.Name = Time.frameCount + " " + go.name + " [" + go.GetInstanceID() + "]";
		entry.StackTrace = stack_trace;
		Entry item = entry;
		DestroyTraces.Add(item);
	}
}
