public static class WorldGenLogger
{
	public static void LogException(string message, string stack)
	{
		Output.LogError(message + "\n" + stack);
	}
}
