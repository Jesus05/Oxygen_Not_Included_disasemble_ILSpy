using System.Collections.Generic;

namespace Delaunay
{
	public class Node
	{
		public static Stack<Node> pool = new Stack<Node>();

		public Node parent;

		public int treeSize;
	}
}
