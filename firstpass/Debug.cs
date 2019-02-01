#define UNITY_ASSERTIONS
using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public static class Debug
{
	public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

	public static bool developerConsoleVisible
	{
		get
		{
			return UnityEngine.Debug.developerConsoleVisible;
		}
		set
		{
			UnityEngine.Debug.developerConsoleVisible = value;
		}
	}

	private static string TimeStamp()
	{
		return DateTime.UtcNow.ToString("[HH:mm:ss.fff] [") + Thread.CurrentThread.ManagedThreadId + "] ";
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	public static void LogException(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	public static void Log(object obj, UnityEngine.Object context = null)
	{
		Console.Out.Write(TimeStamp() + "[ INFO  ] " + obj + "\n");
	}

	public static void LogFormat(string format, params object[] args)
	{
		Console.Out.Write(TimeStamp() + "[ INFO  ] " + string.Format(format, args) + "\n");
	}

	public static void LogWarning(object obj, UnityEngine.Object context = null)
	{
		Console.Out.Write(TimeStamp() + "[WARNING] " + obj + "\n");
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		Console.Out.Write(TimeStamp() + "[WARNING] " + string.Format(format, args) + "\n");
	}

	public static void LogError(object obj, UnityEngine.Object context = null)
	{
		Console.Out.Write(TimeStamp() + "[ERROR] " + obj + "\n");
		if (context == (UnityEngine.Object)null)
		{
			UnityEngine.Debug.LogError(obj);
		}
		else
		{
			UnityEngine.Debug.LogError(obj, context);
		}
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		Console.Out.Write(TimeStamp() + "[ERROR] " + string.Format(format, args) + "\n");
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition)
	{
		UnityEngine.Debug.Assert(condition);
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message)
	{
		UnityEngine.Debug.Assert(condition, message);
	}

	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.Assert(condition, message, context);
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}
}
