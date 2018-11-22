using System;
using UnityEngine;

public class Output
{
	public static string BuildString(object[] objs)
	{
		string text = "";
		if (objs.Length > 0)
		{
			text = ((objs[0] == null) ? "null" : objs[0].ToString());
			for (int i = 1; i < objs.Length; i++)
			{
				object obj = objs[i];
				text = text + " " + ((obj == null) ? "null" : obj.ToString());
			}
		}
		return text;
	}

	public static void Log(params object[] objs)
	{
		string str = BuildString(objs);
		Print(str);
	}

	public static void LogWithObj(UnityEngine.Object obj, params object[] objs)
	{
		string str = BuildString(objs);
		PrintWithObj(obj, str);
	}

	public static void LogError(params object[] objs)
	{
		string str = BuildString(objs);
		LogError("ERROR: " + str);
	}

	public static void LogErrorWithObj(UnityEngine.Object obj, params object[] objs)
	{
		string str = BuildString(objs);
		LogErrorWithObj(obj, str);
	}

	public static void LogCriticalWarning(params object[] objs)
	{
		LogWarning(objs);
	}

	public static void LogWarning(params object[] objs)
	{
		string obj = BuildString(objs);
		Debug.LogWarning(obj, null);
	}

	public static void LogWarningWithObj(UnityEngine.Object obj, params object[] objs)
	{
		string obj2 = BuildString(objs);
		Debug.LogWarning(obj2, obj);
	}

	public static void Print(string str)
	{
		Console.Out.WriteLine(str);
	}

	private static void PrintWithObj(UnityEngine.Object obj, string str)
	{
		Console.Out.WriteLine(str + " : " + ((!(obj != (UnityEngine.Object)null)) ? "<null>" : obj.name));
	}

	private static void Warn(string str)
	{
		Console.Out.WriteLine("WARNING: " + str);
	}

	private static void LogWarningWithObj(UnityEngine.Object obj, string str)
	{
		Console.Out.WriteLine("WARNING: " + str + " : " + ((!(obj != (UnityEngine.Object)null)) ? "<null>" : obj.name));
	}

	public static void LogError(string str)
	{
		Debug.LogError(str, null);
	}

	private static void LogErrorWithObj(UnityEngine.Object obj, string str)
	{
		Debug.LogError(str, obj);
	}
}
