using System.Diagnostics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct LoggerFS
{
	public LoggerFS(string name, int max_entries = 35)
	{
	}

	public string GetName()
	{
		return string.Empty;
	}

	public void SetName(string name)
	{
	}

	[Conditional("ENABLE_LOGGER")]
	public void Log(string evt)
	{
	}
}
