using System.Diagnostics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct LoggerFSFSF
{
	public LoggerFSFSF(string name, int max_entries = 35)
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
	public void Log(string name1, float val1, string name2, float val2)
	{
	}
}
