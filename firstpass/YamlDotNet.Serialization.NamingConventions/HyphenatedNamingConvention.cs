using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NamingConventions
{
	public sealed class HyphenatedNamingConvention : INamingConvention
	{
		public string Apply(string value)
		{
			return value.FromCamelCase("-");
		}
	}
}
