using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NamingConventions
{
	public sealed class PascalCaseNamingConvention : INamingConvention
	{
		public string Apply(string value)
		{
			return value.ToPascalCase();
		}
	}
}
