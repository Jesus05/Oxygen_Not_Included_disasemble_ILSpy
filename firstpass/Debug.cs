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

	private static void WriteTimeStamped(params object[] objs)
	{
		string value = TimeStamp() + DebugUtil.BuildString(objs);
		Console.WriteLine(value);
	}

	public static void Break()
	{
	}

	public static void LogException(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	public static void Log(object obj)
	{
		WriteTimeStamped("[INFO]", obj);
	}

	public static void Log(object obj, UnityEngine.Object context)
	{
		WriteTimeStamped("[INFO]", (!(context != (UnityEngine.Object)null)) ? "null" : context.name, obj);
	}

	public static void LogFormat(string format, params object[] args)
	{
		WriteTimeStamped("[INFO]", string.Format(format, args));
	}

	public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
	{
		WriteTimeStamped("[INFO]", (!(context != (UnityEngine.Object)null)) ? "null" : context.name, string.Format(format, args));
	}

	public static void LogWarning(object obj)
	{
		WriteTimeStamped("[WARNING]", obj);
	}

	public static void LogWarning(object obj, UnityEngine.Object context)
	{
		WriteTimeStamped("[WARNING]", (!(context != (UnityEngine.Object)null)) ? "null" : context.name, obj);
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		WriteTimeStamped("[WARNING]", string.Format(format, args));
	}

	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
	{
		WriteTimeStamped("[WARNING]", (!(context != (UnityEngine.Object)null)) ? "null" : context.name, string.Format(format, args));
	}

	public static void LogError(object obj)
	{
		WriteTimeStamped("[ERROR]", obj);
		UnityEngine.Debug.LogError(obj);
	}

	public static void LogError(object obj, UnityEngine.Object context)
	{
		WriteTimeStamped("[ERROR]", (!(context != (UnityEngine.Object)null)) ? "null" : context.name, obj);
		UnityEngine.Debug.LogError(obj, context);
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		WriteTimeStamped("[ERROR]", string.Format(format, args));
		UnityEngine.Debug.LogErrorFormat(format, args);
	}

	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
	{
		WriteTimeStamped("[ERROR]", (!(context != (UnityEngine.Object)null)) ? "null" : context.name, string.Format(format, args));
		UnityEngine.Debug.LogErrorFormat(context, format, args);
	}

	public static void Assert(bool condition)
	{
		if (!condition)
		{
			LogError("Assert failed");
			Break();
		}
	}

	public static void Assert(bool condition, object message)
	{
		if (!condition)
		{
			LogError("Assert failed: " + message);
			Break();
		}
	}

	public static void Assert(bool condition, object message, UnityEngine.Object context)
	{
		if (!condition)
		{
			LogError("Assert failed: " + message, context);
			Break();
		}
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
