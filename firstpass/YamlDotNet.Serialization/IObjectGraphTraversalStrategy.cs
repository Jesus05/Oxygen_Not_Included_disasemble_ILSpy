namespace YamlDotNet.Serialization
{
	public interface IObjectGraphTraversalStrategy
	{
		void Traverse<TContext>(IObjectDescriptor graph, IObjectGraphVisitor<TContext> visitor, TContext context);
	}
}
