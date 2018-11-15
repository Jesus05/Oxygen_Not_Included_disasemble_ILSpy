using System.Diagnostics;
using UnityEngine;

public static class DebugUtil
{
	public static void Assert(bool test, string message = "Assert!", string message1 = "", string message2 = "")
	{
		if (!test)
		{
			Debug.LogError(message + " " + message1 + " " + message2, null);
			Debug.Break();
		}
	}

	public static void DevAssert(bool test, string message0 = "Assert!", string message1 = "", string message2 = "")
	{
		if (!test)
		{
			if (Application.isEditor)
			{
				Debug.LogError(message0 + message1 + message2, null);
				Debug.Break();
			}
			else
			{
				Debug.LogWarning(message0 + message1 + message2, null);
			}
		}
	}

	public static void SoftAssert(bool test, string message = "Assert!")
	{
		if (!test)
		{
			Debug.LogWarning(message, null);
		}
	}

	public static string FullName(Component cmp)
	{
		return FullName(cmp.gameObject) + " (" + cmp.GetType().ToString() + " " + cmp.GetInstanceID() + ")";
	}

	public static string FullName(GameObject obj)
	{
		GameObject gameObject = obj;
		string text = "/" + obj.name;
		while ((Object)obj.transform.parent != (Object)null)
		{
			obj = obj.transform.parent.gameObject;
			text = "/" + obj.name + text;
		}
		return text + " (" + gameObject.GetInstanceID() + ")";
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
