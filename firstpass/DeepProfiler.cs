using System.Diagnostics;

public class DeepProfiler
{
	private bool enableProfiling;

	public DeepProfiler(bool enable_profiling)
	{
		enableProfiling = enable_profiling;
	}

	[Conditional("DEEP_PROFILE")]
	public void BeginSample(string message)
	{
		if (!enableProfiling)
		{
			return;
		}
	}

	[Conditional("DEEP_PROFILE")]
	public void EndSample()
	{
		if (!enableProfiling)
		{
			return;
		}
	}
}
