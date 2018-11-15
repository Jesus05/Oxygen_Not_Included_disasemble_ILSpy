using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NamingConventions
{
	public sealed class CamelCaseNamingConvention : INamingConvention
	{
		public string Apply(string value)
		{
			return value.ToCamelCase();
		}
	}
}
