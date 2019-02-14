using System.Diagnostics;
using System.Text;
using UnityEngine;

public static class DebugUtil
{
	private static StringBuilder errorMessageBuilder = new StringBuilder();

	private static StringBuilder fullNameBuilder = new StringBuilder();

	private static void Break(string message)
	{
		Debug.LogError(message, null);
		Debug.Break();
		Debugger.Break();
	}

	public static void Assert(bool test)
	{
		if (!test)
		{
			Break("Failed assertion");
		}
	}

	public static void Assert(bool test, string message)
	{
		if (!test)
		{
			Break(message);
		}
	}

	public static void Assert(bool test, string message0, string message1)
	{
		if (!test)
		{
			errorMessageBuilder.Length = 0;
			Break(errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.ToString());
		}
	}

	public static void Assert(bool test, string message0, string message1, string message2)
	{
		if (!test)
		{
			errorMessageBuilder.Length = 0;
			Break(errorMessageBuilder.Append(message0).Append(" ").Append(message1)
				.Append(" ")
				.Append(message2)
				.ToString());
		}
	}

	public static void Assert(bool test, params object[] objs)
	{
		if (!test)
		{
			Debug.LogError(Output.BuildString(objs), null);
			Debug.Break();
			Debugger.Break();
		}
	}

	public static void DevAssert(bool test, params object[] objs)
	{
		if (!test)
		{
			if (Application.isEditor)
			{
				Debug.LogError(Output.BuildString(objs), null);
				Debug.Break();
				Debugger.Break();
			}
			else
			{
				Debug.LogWarning(Output.BuildString(objs), null);
			}
		}
	}

	public static void DevAssertWithStack(bool test, params object[] objs)
	{
		if (!test)
		{
			if (Application.isEditor)
			{
				Debug.LogError(Output.BuildString(objs), null);
				Debug.Break();
				Debugger.Break();
			}
			else
			{
				StackTrace arg = new StackTrace(1, true);
				string obj = $"{Output.BuildString(objs)}\n{arg}";
				Debug.LogWarning(obj, null);
			}
		}
	}

	public static void DevLogErrorWithObj(GameObject gameObject, string msg)
	{
		if (Debug.isDebugBuild)
		{
			Output.LogErrorWithObj(gameObject, msg);
		}
		else
		{
			Output.LogWarningWithObj(gameObject, msg);
		}
	}

	public static void SoftAssert(bool test, params object[] objs)
	{
		if (!test)
		{
			Debug.LogWarning(Output.BuildString(objs), null);
		}
	}

	private static void RecursiveBuildFullName(GameObject obj)
	{
		if (!((Object)obj == (Object)null))
		{
			RecursiveBuildFullName(obj.transform.parent.gameObject);
			fullNameBuilder.Append("/").Append(obj.name);
		}
	}

	private static StringBuilder BuildFullName(GameObject obj)
	{
		fullNameBuilder.Length = 0;
		RecursiveBuildFullName(obj);
		return fullNameBuilder.Append(" (").Append(obj.GetInstanceID()).Append(")");
	}

	public static string FullName(GameObject obj)
	{
		return BuildFullName(obj).ToString();
	}

	public static string FullName(Component cmp)
	{
		return BuildFullName(cmp.gameObject).Append(" (").Append(cmp.GetType()).Append(" ")
			.Append(cmp.GetInstanceID().ToString())
			.Append(")")
			.ToString();
	}

	public static void LogIfSelected(GameObject obj, params object[] objs)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileBegin(string str)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileBegin(string str, Object target)
	{
	}

	[Conditional("ENABLE_DETAILED_PROFILING")]
	public static void ProfileEnd()
	{
	}
}
