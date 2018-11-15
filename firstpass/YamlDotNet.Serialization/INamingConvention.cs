namespace YamlDotNet.Serialization
{
	public interface INamingConvention
	{
		string Apply(string value);
	}
}
