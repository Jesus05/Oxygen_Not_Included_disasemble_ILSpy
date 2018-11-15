using System.Diagnostics;

namespace KSerialization
{
	internal static class DebugLog
	{
		public enum Level
		{
			Error,
			Warning,
			Info
		}

		private const Level OutputLevel = Level.Error;

		[Conditional("DEBUG_LOG")]
		public static void Output(Level msg_level, string msg)
		{
			switch (msg_level)
			{
			case Level.Error:
				Debug.LogError(msg, null);
				break;
			}
			return;
			IL_0021:
			Debug.Log(msg, null);
			return;
			IL_002d:
			Debug.LogWarning(msg, null);
		}
	}
}
