using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public static class KProfiler
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Region : IDisposable
	{
		public Region(string region_name, UnityEngine.Object profiler_obj = null)
		{
		}

		public void Dispose()
		{
		}
	}

	public static int counter;

	public static Thread main_thread;

	public static void BeginThreadProfiling(string threadGroupName, string threadName)
	{
	}

	public static void EndThreadProfiling()
	{
	}

	public static int BeginSampleI(string region_name)
	{
		int result = counter;
		counter++;
		return result;
	}

	public static int BeginSampleI(string region_name, UnityEngine.Object profiler_obj)
	{
		int result = counter;
		counter++;
		return result;
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name)
	{
		BeginSampleI(region_name);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, int count)
	{
		BeginSampleI(region_name);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void BeginSample(string region_name, UnityEngine.Object profiler_obj)
	{
		BeginSampleI(region_name, profiler_obj);
	}

	[Conditional("ENABLE_KPROFILER")]
	public static void EndSample()
	{
		EndSampleI();
	}

	public static int EndSampleI()
	{
		counter--;
		return counter;
	}

	public static void AddEvent(string event_name)
	{
	}

	public static void AddCounter(string event_name, List<KeyValuePair<string, int>> series_name_counts)
	{
	}

	public static void AddCounter(string event_name, string series_name, int count)
	{
	}

	public static void AddCounter(string event_name, int count)
	{
		AddCounter(event_name, event_name, count);
	}
}
