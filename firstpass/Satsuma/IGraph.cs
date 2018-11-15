using System.Collections.Generic;

namespace Satsuma
{
	public interface IGraph : IArcLookup
	{
		IEnumerable<Node> Nodes();

		IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All);

		IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All);

		IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All);

		int NodeCount();

		int ArcCount(ArcFilter filter = ArcFilter.All);

		int ArcCount(Node u, ArcFilter filter = ArcFilter.All);

		int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All);

		bool HasNode(Node node);

		bool HasArc(Arc arc);
	}
}
