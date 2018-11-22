using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

	public static int counter = 0;

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
}
