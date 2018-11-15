namespace YamlDotNet.RepresentationModel
{
	public interface IYamlVisitor
	{
		void Visit(YamlStream stream);

		void Visit(YamlDocument document);

		void Visit(YamlScalarNode scalar);

		void Visit(YamlSequenceNode sequence);

		void Visit(YamlMappingNode mapping);
	}
}
