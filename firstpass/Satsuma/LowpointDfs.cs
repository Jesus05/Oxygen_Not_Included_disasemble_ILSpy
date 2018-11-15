using System.Collections.Generic;

namespace Satsuma
{
	internal class LowpointDfs : Dfs
	{
		protected Dictionary<Node, int> level;

		protected Dictionary<Node, int> lowpoint;

		private void UpdateLowpoint(Node node, int newLowpoint)
		{
			if (lowpoint[node] > newLowpoint)
			{
				lowpoint[node] = newLowpoint;
			}
		}

		protected override void Start(out Direction direction)
		{
			direction = Direction.Undirected;
			level = new Dictionary<Node, int>();
			lowpoint = new Dictionary<Node, int>();
		}

		protected override bool NodeEnter(Node node, Arc arc)
		{
			level[node] = base.Level;
			lowpoint[node] = base.Level;
			return true;
		}

		protected override bool NodeExit(Node node, Arc arc)
		{
			if (arc != Arc.Invalid)
			{
				Node node2 = base.Graph.Other(arc, node);
				UpdateLowpoint(node2, lowpoint[node]);
			}
			return true;
		}

		protected override bool BackArc(Node node, Arc arc)
		{
			Node key = base.Graph.Other(arc, node);
			UpdateLowpoint(node, level[key]);
			return true;
		}

		protected override void StopSearch()
		{
			level = null;
			lowpoint = null;
		}
	}
}
