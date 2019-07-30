using System.Collections.Generic;

namespace ClipperLib
{
	public class MyIntersectNodeSort : IComparer<IntersectNode>
	{
		public int Compare(IntersectNode node1, IntersectNode node2)
		{
			long num = node2.Pt.Y - node1.Pt.Y;
			if (num > 0)
			{
				return 1;
			}
			if (num < 0)
			{
				return -1;
			}
			return 0;
		}
	}
}
