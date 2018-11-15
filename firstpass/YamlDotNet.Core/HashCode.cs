namespace YamlDotNet.Core
{
	internal static class HashCode
	{
		public static int CombineHashCodes(int h1, int h2)
		{
			return ((h1 << 5) + h1) ^ h2;
		}
	}
}
