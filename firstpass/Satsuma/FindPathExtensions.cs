using System;
using System.Collections.Generic;

namespace Satsuma
{
	public static class FindPathExtensions
	{
		private class PathDfs : Dfs
		{
			public Direction PathDirection;

			public Func<Node, bool> IsTarget;

			public Node StartNode;

			public List<Arc> Path;

			public Node EndNode;

			protected override void Start(out Direction direction)
			{
				direction = PathDirection;
				StartNode = Node.Invalid;
				Path = new List<Arc>();
				EndNode = Node.Invalid;
			}

			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					StartNode = node;
				}
				else
				{
					Path.Add(arc);
				}
				if (IsTarget(node))
				{
					EndNode = node;
					return false;
				}
				return true;
			}

			protected override bool NodeExit(Node node, Arc arc)
			{
				if (arc != Arc.Invalid && EndNode == Node.Invalid)
				{
					Path.RemoveAt(Path.Count - 1);
				}
				return true;
			}
		}

		public static IPath FindPath(this IGraph graph, IEnumerable<Node> source, Func<Node, bool> target, Dfs.Direction direction)
		{
			PathDfs pathDfs = new PathDfs();
			pathDfs.PathDirection = direction;
			pathDfs.IsTarget = target;
			PathDfs pathDfs2 = pathDfs;
			pathDfs2.Run(graph, source);
			if (pathDfs2.EndNode == Node.Invalid)
			{
				return null;
			}
			Path path = new Path(graph);
			path.Begin(pathDfs2.StartNode);
			foreach (Arc item in pathDfs2.Path)
			{
				path.AddLast(item);
			}
			return path;
		}

		public static IPath FindPath(this IGraph graph, Node source, Node target, Dfs.Direction direction)
		{
			return graph.FindPath(new Node[1]
			{
				source
			}, (Node x) => x == target, direction);
		}
	}
}
