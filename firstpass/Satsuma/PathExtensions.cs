using System.Collections.Generic;

namespace Satsuma
{
	public static class PathExtensions
	{
		public static bool IsCycle(this IPath path)
		{
			return path.FirstNode == path.LastNode && path.ArcCount(ArcFilter.All) > 0;
		}

		public static Node NextNode(this IPath path, Node node)
		{
			Arc arc = path.NextArc(node);
			if (!(arc == Arc.Invalid))
			{
				return path.Other(arc, node);
			}
			return Node.Invalid;
		}

		public static Node PrevNode(this IPath path, Node node)
		{
			Arc arc = path.PrevArc(node);
			if (!(arc == Arc.Invalid))
			{
				return path.Other(arc, node);
			}
			return Node.Invalid;
		}

		internal static IEnumerable<Arc> ArcsHelper(this IPath path, Node u, ArcFilter filter)
		{
			Arc arc3 = path.PrevArc(u);
			Arc arc2 = path.NextArc(u);
			if (arc3 == arc2)
			{
				arc2 = Arc.Invalid;
			}
			for (int i = 0; i < 2; i++)
			{
				Arc arc = (i != 0) ? arc2 : arc3;
				if (!(arc == Arc.Invalid))
				{
					switch (filter)
					{
					case ArcFilter.All:
						yield return arc;
						/*Error: Unable to find new state assignment for yield return*/;
					case ArcFilter.Edge:
						if (path.IsEdge(arc))
						{
							yield return arc;
							/*Error: Unable to find new state assignment for yield return*/;
						}
						break;
					case ArcFilter.Forward:
						if (path.IsEdge(arc) || path.U(arc) == u)
						{
							yield return arc;
							/*Error: Unable to find new state assignment for yield return*/;
						}
						break;
					case ArcFilter.Backward:
						if (path.IsEdge(arc) || path.V(arc) == u)
						{
							yield return arc;
							/*Error: Unable to find new state assignment for yield return*/;
						}
						break;
					}
				}
			}
		}
	}
}
