namespace YamlDotNet.Serialization.NamingConventions
{
	public sealed class NullNamingConvention : INamingConvention
	{
		public string Apply(string value)
		{
			return value;
		}
	}
}
