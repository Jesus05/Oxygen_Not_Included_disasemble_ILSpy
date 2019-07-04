using System.Diagnostics;
using System.IO;

public class FileLog
{
	private static FileLog instance = null;

	private StreamWriter writer;

	private FileLog(string filename)
	{
		writer = new StreamWriter(filename);
		writer.AutoFlush = true;
	}

	[Conditional("ENABLE_LOG")]
	public static void Initialize(string filename)
	{
		instance = new FileLog(filename);
	}

	[Conditional("ENABLE_LOG")]
	public static void Shutdown()
	{
		if (instance.writer != null)
		{
			instance.writer.Close();
		}
		instance = null;
	}

	[Conditional("ENABLE_LOG")]
	public static void Log(params object[] objs)
	{
		instance.LogObjs(objs);
	}

	private void LogObjs(object[] objs)
	{
		string value = BuildString(objs);
		writer.WriteLine(value);
	}

	private static string BuildString(object[] objs)
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
}
