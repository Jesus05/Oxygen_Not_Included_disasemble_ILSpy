using System.Diagnostics;
using UnityEngine;

public class LoadProfiler : ProfilerBase
{
	private static LoadProfiler instance = null;

	public static LoadProfiler Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new LoadProfiler("load_stats_");
				if (!Stopwatch.IsHighResolution)
				{
					UnityEngine.Debug.LogWarning("Low resolution timer! [" + Stopwatch.Frequency + "] ticks per second");
				}
			}
			return instance;
		}
	}

	private LoadProfiler(string file_prefix)
		: base(file_prefix)
	{
	}

	private static void ProfilerSection(string region_name, string file = "unknown", uint line = 0u)
	{
		Instance.Push(region_name, file, line);
	}

	private static void EndProfilerSection()
	{
		Instance.Pop();
	}

	[Conditional("ENABLE_LOAD_STATS")]
	public static void AddEvent(string event_name, string file = "unknown", uint line = 0u)
	{
		if (Instance.IsRecording() && Instance.proFile != null)
		{
			Instance.WriteLine(event_name, "I", "},");
		}
	}

	[Conditional("ENABLE_LOAD_STATS")]
	public static void BeginSample(string region_name)
	{
		Instance.Push(region_name, "unknown", 0u);
	}

	[Conditional("ENABLE_LOAD_STATS")]
	public static void EndSample()
	{
		Instance.Pop();
	}
}
