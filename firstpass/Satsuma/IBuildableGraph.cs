namespace Satsuma
{
	public interface IBuildableGraph : IClearable
	{
		Node AddNode();

		Arc AddArc(Node u, Node v, Directedness directedness);
	}
}
