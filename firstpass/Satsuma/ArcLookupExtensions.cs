namespace Satsuma
{
	public static class ArcLookupExtensions
	{
		public static string ArcToString(this IArcLookup graph, Arc arc)
		{
			if (arc == Arc.Invalid)
			{
				return "Arc.Invalid";
			}
			return graph.U(arc) + ((!graph.IsEdge(arc)) ? "--->" : "<-->") + graph.V(arc);
		}

		public static Node Other(this IArcLookup graph, Arc arc, Node node)
		{
			Node node2 = graph.U(arc);
			if (node2 != node)
			{
				return node2;
			}
			return graph.V(arc);
		}

		public static Node[] Nodes(this IArcLookup graph, Arc arc, bool allowDuplicates = true)
		{
			Node node = graph.U(arc);
			Node node2 = graph.V(arc);
			if (!allowDuplicates && node == node2)
			{
				return new Node[1]
				{
					node
				};
			}
			return new Node[2]
			{
				node,
				node2
			};
		}
	}
}
