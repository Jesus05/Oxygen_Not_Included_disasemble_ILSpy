public static class WorldGenLogger
{
	public static void LogException(string message, string stack)
	{
		Debug.LogError(message + "\n" + stack);
	}
}
