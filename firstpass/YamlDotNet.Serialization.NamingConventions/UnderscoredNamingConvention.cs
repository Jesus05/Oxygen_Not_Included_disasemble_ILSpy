using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NamingConventions
{
	public sealed class UnderscoredNamingConvention : INamingConvention
	{
		public string Apply(string value)
		{
			return value.FromCamelCase("_");
		}
	}
}
