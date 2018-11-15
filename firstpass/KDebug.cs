#define CHECK_ASSERTS
using System.Diagnostics;
using UnityEngine;

public class KDebug
{
	[Conditional("CHECK_ASSERTS")]
	public static void CheckValidFloat(float f)
	{
		if (float.IsNaN(f) || float.IsPositiveInfinity(f) || float.IsNegativeInfinity(f))
		{
			DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertLess(float f, float max)
	{
		if (f >= max)
		{
			DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertGreater(float f, float min)
	{
		if (f <= min)
		{
			DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertLessEqual(float f, float max)
	{
		if (f > max)
		{
			DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void AssertEqual(float f, float expected)
	{
		if (f != expected)
		{
			DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void Assert(bool condition)
	{
		if (!condition)
		{
			DebugBreak();
		}
	}

	[Conditional("CHECK_ASSERTS")]
	public static void DebugBreak()
	{
		StackTrace stackTrace = new StackTrace(true);
		Output.LogError("Assert failed at:\n", stackTrace.ToString());
		UnityEngine.Debug.Break();
	}
}
