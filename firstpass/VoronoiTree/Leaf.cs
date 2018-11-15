using Delaunay.Geo;
using System.Collections.Generic;

namespace VoronoiTree
{
	public class Leaf : Node
	{
		public Leaf()
			: base(NodeType.Leaf)
		{
		}

		public Leaf(Diagram.Site site, Tree parent)
			: base(site, NodeType.Leaf, parent)
		{
		}

		public override Tree Split(SplitCommand cmd)
		{
			Tree tree = base.parent.ReplaceLeafWithTree(this);
			tree.Split(cmd);
			return tree;
		}

		public void GetIntersectingSites(LineSegment edge, List<Diagram.Site> intersectingSites)
		{
			if (site != null && site.poly != null)
			{
				LineSegment intersectingSegment = new LineSegment(null, null);
				if (site.poly.ClipSegment(edge, ref intersectingSegment))
				{
					intersectingSites.Add(site);
				}
			}
		}
	}
}
