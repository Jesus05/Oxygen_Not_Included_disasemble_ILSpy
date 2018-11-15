using System.Collections.Generic;

namespace Satsuma
{
	internal class BridgeDfs : LowpointDfs
	{
		public int ComponentCount;

		public HashSet<Arc> Bridges;

		protected override void Start(out Direction direction)
		{
			base.Start(out direction);
			ComponentCount = 0;
			Bridges = new HashSet<Arc>();
		}

		protected override bool NodeExit(Node node, Arc arc)
		{
			if (arc == Arc.Invalid)
			{
				ComponentCount++;
			}
			else if (lowpoint[node] == base.Level)
			{
				Bridges.Add(arc);
				ComponentCount++;
			}
			return base.NodeExit(node, arc);
		}
	}
}
