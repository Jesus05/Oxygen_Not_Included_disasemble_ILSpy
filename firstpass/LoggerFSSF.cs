using System.Diagnostics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct LoggerFSSF
{
	public LoggerFSSF(string name)
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
	public void Log(string evt, string param, float value)
	{
	}
}
