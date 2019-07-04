using System.Diagnostics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct LoggerFIO
{
	public LoggerFIO(string name, int max_entries = 35)
	{
	}

	public string GetName()
	{
		return "";
	}

	public void SetName(string name)
	{
	}

	[Conditional("ENABLE_LOGGER")]
	public void Log(int evt, object obj)
	{
	}
}
