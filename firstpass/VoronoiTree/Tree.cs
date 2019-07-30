using Delaunay.Geo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronoiTree
{
	public class Tree : Node
	{
		public delegate bool LeafNodeTest(Node node);

		protected List<Node> children;

		public bool dontRelaxChildren;

		public SeededRandom myRandom
		{
			get;
			private set;
		}

		public Tree()
			: base(NodeType.Internal)
		{
			children = new List<Node>();
			SetSeed(0);
		}

		public Tree(int seed = 0)
			: base(NodeType.Internal)
		{
			children = new List<Node>();
			SetSeed(seed);
		}

		public Tree(Diagram.Site site, Tree parent, int seed = 0)
			: base(site, NodeType.Internal, parent)
		{
			children = new List<Node>();
			SetSeed(seed);
		}

		public Tree(Diagram.Site site, List<Node> children, Tree parent, int seed = 0)
			: base(site, NodeType.Internal, parent)
		{
			if (children == null)
			{
				children = new List<Node>();
			}
			this.children = children;
			SetSeed(seed);
		}

		public void SetSeed(int seed)
		{
			myRandom = new SeededRandom(seed);
		}

		public Node GetChildByID(uint id)
		{
			return children.Find((Node s) => s.site.id == id);
		}

		public int ChildCount()
		{
			if (children == null)
			{
				return 0;
			}
			return children.Count;
		}

		public Tree GetChildContainingLeaf(Leaf leaf)
		{
			Vector2 point = leaf.site.poly.Centroid();
			for (int i = 0; i < children.Count; i++)
			{
				Tree tree = children[i] as Tree;
				if (tree != null && tree.site.poly.Contains(point))
				{
					return tree;
				}
			}
			return null;
		}

		public Node GetChild(int childIndex)
		{
			if (childIndex < children.Count)
			{
				return children[childIndex];
			}
			return null;
		}

		public void AddChild(Node child)
		{
			if (child.site.id > Node.maxIndex)
			{
				Node.maxIndex = child.site.id;
			}
			children.Add(child);
			child.SetParent(this);
		}

		public Node AddSite(Diagram.Site site, NodeType type)
		{
			Node node = null;
			node = ((type != NodeType.Internal) ? ((Node)new Leaf(site, this)) : ((Node)new Tree(site, this, myRandom.seed + ChildCount())));
			AddChild(node);
			return node;
		}

		public bool ComputeChildrenRecursive(int depth, bool pd = false)
		{
			if (depth > Node.maxDepth || site.poly == null || children == null)
			{
				return false;
			}
			List<Diagram.Site> list = new List<Diagram.Site>();
			for (int i = 0; i < children.Count; i++)
			{
				list.Add(children[i].site);
			}
			PlaceSites(list, depth);
			if (pd)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (!site.poly.Contains(list[j].position))
					{
						Debug.LogErrorFormat("Cant feed points [{0}] to powerdiagram that are outside its area [{1}] ", list[j].id, list[j].position);
					}
				}
				if (ComputeNodePD(list, 500, 0.2f))
				{
					for (int k = 0; k < children.Count; k++)
					{
						if (children[k].type == NodeType.Internal)
						{
							Tree tree = children[k] as Tree;
							if (!tree.ComputeChildrenRecursive(depth + 1, pd))
							{
								return false;
							}
						}
					}
				}
			}
			else if (ComputeNode(list))
			{
				for (int l = 0; l < children.Count; l++)
				{
					if (children[l].type == NodeType.Internal)
					{
						Tree tree2 = children[l] as Tree;
						if (!tree2.ComputeChildrenRecursive(depth + 1, false))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public bool ComputeChildren(int seed, bool place = false, bool pd = false)
		{
			if (site.poly == null || children == null)
			{
				return false;
			}
			List<Diagram.Site> list = new List<Diagram.Site>();
			for (int i = 0; i < children.Count; i++)
			{
				if (place || !site.poly.Contains(children[i].site.position))
				{
					Debug.LogErrorFormat("Cant feed points [{0}] to powerdiagram that are outside its area [{1}] ", children[i].site.id, children[i].site.position);
				}
				list.Add(children[i].site);
			}
			if (place)
			{
				PlaceSites(list, seed);
			}
			if (pd)
			{
				ComputeNodePD(list, 500, 0.2f);
			}
			else
			{
				ComputeNode(list);
			}
			return true;
		}

		public int Count()
		{
			if (children == null || children.Count == 0)
			{
				return 0;
			}
			int num = children.Count;
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].type == NodeType.Internal)
				{
					Tree tree = children[i] as Tree;
					num += tree.Count();
				}
			}
			return num;
		}

		public void Reset()
		{
			if (children != null)
			{
				for (int i = 0; i < children.Count; i++)
				{
					if (children[i].type == NodeType.Internal)
					{
						Tree tree = children[i] as Tree;
						tree.Reset();
					}
				}
			}
			Reset(null);
		}

		public int MaxDepth(int depth = 0)
		{
			if (children == null || children.Count == 0)
			{
				return depth;
			}
			int num = depth + 1;
			int num2 = num;
			for (int i = 0; i < children.Count; i++)
			{
				int num3 = num2 + 1;
				if (children[i].type == NodeType.Internal)
				{
					Tree tree = children[i] as Tree;
					num3 = tree.MaxDepth(num2);
				}
				if (num3 > num)
				{
					num = num3;
				}
			}
			return num;
		}

		public void RelaxRecursive(int depth, int iterations = -1, float minEnergy = 1f, bool pd = false)
		{
			if (dontRelaxChildren || site.poly == null || children == null || children.Count == 0)
			{
				visited = VisitedType.MissingData;
			}
			else
			{
				List<Diagram.Site> list = new List<Diagram.Site>();
				for (int i = 0; i < children.Count; i++)
				{
					list.Add(children[i].site);
				}
				float num = 3.40282347E+38f;
				for (int j = 0; j < iterations; j++)
				{
					if (!(num > minEnergy))
					{
						break;
					}
					float num2 = 0f;
					for (int k = 0; k < children.Count; k++)
					{
						num2 += Vector2.Distance(children[k].site.position, list[k].poly.Centroid());
						children[k].site.position = list[k].poly.Centroid();
					}
					num = num2;
					PlaceSites(list, depth);
					if (pd)
					{
						if (!ComputeNodePD(list, 500, 0.2f))
						{
							visited = VisitedType.Error;
							return;
						}
					}
					else if (!ComputeNode(list))
					{
						visited = VisitedType.Error;
						return;
					}
				}
				for (int l = 0; l < children.Count; l++)
				{
					if (children[l].type == NodeType.Internal)
					{
						Tree tree = children[l] as Tree;
						if (tree.ComputeChildren(depth, false, false))
						{
							tree.RelaxRecursive(depth + 1, iterations, minEnergy, false);
						}
					}
				}
				visited = VisitedType.VisitedSuccess;
			}
		}

		public float Relax(int depth, int relaxDepth, bool pd = false)
		{
			if (dontRelaxChildren || depth > Node.maxDepth || depth > relaxDepth || site.poly == null || children == null || children.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			if (depth < relaxDepth)
			{
				for (int i = 0; i < children.Count; i++)
				{
					if (children[i].type == NodeType.Internal)
					{
						Tree tree = children[i] as Tree;
						num += tree.Relax(depth + 1, relaxDepth, false);
					}
				}
				return num;
			}
			if (depth == relaxDepth)
			{
				List<Diagram.Site> list = new List<Diagram.Site>();
				for (int j = 0; j < children.Count; j++)
				{
					list.Add(children[j].site);
				}
				if (pd)
				{
					if (!ComputeNodePD(list, 500, 0.2f))
					{
						return 0f;
					}
				}
				else
				{
					PlaceSites(list, depth);
					if (!ComputeNode(list))
					{
						return 0f;
					}
				}
				for (int k = 0; k < children.Count; k++)
				{
					num += Vector2.Distance(children[k].site.position, list[k].poly.Centroid());
					children[k].site.position = list[k].poly.Centroid();
					if (children[k].type == NodeType.Internal)
					{
						Tree tree2 = children[k] as Tree;
						if (!tree2.ComputeChildren(depth, false, false))
						{
							return 0f;
						}
					}
				}
			}
			return num;
		}

		public Node GetNodeForPoint(Vector2 point, bool stopAtFirstChild = false)
		{
			if (site.poly == null)
			{
				return null;
			}
			if (children == null || children.Count == 0)
			{
				return this;
			}
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].site.poly.Contains(point))
				{
					if (children[i].type == NodeType.Internal)
					{
						Tree tree = children[i] as Tree;
						if (stopAtFirstChild)
						{
							return children[i];
						}
						return tree.GetNodeForPoint(point, false);
					}
					return children[i];
				}
			}
			if (site.poly.Contains(point))
			{
				return this;
			}
			return null;
		}

		public Node GetNodeForSite(Diagram.Site target)
		{
			if (site == target)
			{
				return this;
			}
			if (site.poly == null || children == null || children.Count == 0)
			{
				return null;
			}
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].site == target)
				{
					return children[i];
				}
				if (children[i].site.poly.Contains(target.position))
				{
					if (children[i].type == NodeType.Internal)
					{
						Tree tree = children[i] as Tree;
						return tree.GetNodeForSite(target);
					}
					return children[i];
				}
			}
			return null;
		}

		public void GetIntersectingLeafSites(LineSegment edge, List<Diagram.Site> intersectingSites)
		{
			LineSegment intersectingSegment = new LineSegment(null, null);
			if ((site.poly.Contains(edge.p0.Value) | site.poly.Contains(edge.p1.Value)) || site.poly.ClipSegment(edge, ref intersectingSegment))
			{
				if (children.Count == 0)
				{
					intersectingSites.Add(site);
				}
				else
				{
					for (int i = 0; i < children.Count; i++)
					{
						if (children[i].type == NodeType.Internal)
						{
							((Tree)children[i]).GetIntersectingLeafSites(edge, intersectingSites);
						}
						else
						{
							((Leaf)children[i]).GetIntersectingSites(edge, intersectingSites);
						}
					}
				}
			}
		}

		public void GetIntersectingLeafNodes(LineSegment edge, List<Leaf> intersectingNodes)
		{
			LineSegment intersectingSegment = new LineSegment(null, null);
			for (int i = 0; i < children.Count; i++)
			{
				if ((children[i].site.poly.Contains(edge.p0.Value) | children[i].site.poly.Contains(edge.p1.Value)) || children[i].site.poly.ClipSegment(edge, ref intersectingSegment))
				{
					if (children[i].type == NodeType.Internal)
					{
						((Tree)children[i]).GetIntersectingLeafNodes(edge, intersectingNodes);
					}
					else
					{
						intersectingNodes.Add((Leaf)children[i]);
					}
				}
			}
		}

		public override Tree Split(SplitCommand cmd)
		{
			if (cmd.SplitFunction != null)
			{
				cmd.SplitFunction(this, cmd);
			}
			ComputeChildrenRecursive(0, false);
			RelaxRecursive(0, 3, 1f, false);
			return this;
		}

		public Tree ReplaceLeafWithTree(Leaf leaf)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] == leaf)
				{
					children[i] = new Tree(leaf.site, this, myRandom.seed + i);
					children[i].log = leaf.log;
					if (leaf.tags != null)
					{
						children[i].SetTags(leaf.tags);
					}
					return children[i] as Tree;
				}
			}
			return null;
		}

		public Leaf ReplaceTreeWithLeaf(Tree tree)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] == tree)
				{
					children[i] = new Leaf(tree.site, this);
					children[i].log = tree.log;
					if (tree.tags != null)
					{
						children[i].SetTags(tree.tags);
					}
					return children[i] as Leaf;
				}
			}
			return null;
		}

		public void ForceLowestToLeaf()
		{
			List<Node> list = new List<Node>();
			GetLeafNodes(list, null);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].type == NodeType.Internal)
				{
					list[i].parent.ReplaceTreeWithLeaf(list[i] as Tree);
				}
			}
		}

		public void Collapse()
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] != null && children[i].type == NodeType.Internal)
				{
					Tree tree = (Tree)children[i];
					if (tree.ChildCount() > 1)
					{
						tree.Collapse();
					}
					else
					{
						Node node = children[i];
						children[i] = new Leaf(tree.site, this);
						children[i].log = node.log;
						if (tree.tags != null)
						{
							children[i].SetTags(tree.tags);
						}
					}
				}
			}
		}

		public void VisitAll(Action<Node> action)
		{
			action(this);
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].type == NodeType.Internal)
				{
					(children[i] as Tree).VisitAll(action);
				}
				else
				{
					action(children[i]);
				}
			}
		}

		public void GetLeafNodes(List<Node> nodes, LeafNodeTest test = null)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].type == NodeType.Internal)
				{
					Tree tree = (Tree)children[i];
					if (tree.ChildCount() > 0)
					{
						tree.GetLeafNodes(nodes, test);
					}
					else if (test == null || test(children[i]))
					{
						nodes.Add(children[i]);
					}
				}
				else if (test == null || test(children[i]))
				{
					nodes.Add(children[i]);
				}
			}
		}

		public void GetInternalNodes(List<Tree> nodes)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].type == NodeType.Internal)
				{
					Tree tree = (Tree)children[i];
					nodes.Add(tree);
					if (tree.ChildCount() > 0)
					{
						tree.GetInternalNodes(nodes);
					}
				}
			}
		}

		public void ResetParentPointer()
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].type == NodeType.Internal)
				{
					Tree tree = (Tree)children[i];
					if (tree.ChildCount() > 0)
					{
						tree.ResetParentPointer();
					}
				}
				children[i].SetParent(this);
			}
		}

		public void AddTagToChildren(Tag tag)
		{
			for (int i = 0; i < children.Count; i++)
			{
				children[i].AddTag(tag);
			}
		}

		public void GetNodesWithTag(Tag tag, List<Node> nodes)
		{
			if (children.Count == 0 && tags.Contains(tag))
			{
				nodes.Add(this);
			}
			else
			{
				for (int i = 0; i < children.Count; i++)
				{
					if (children[i].type == NodeType.Internal)
					{
						((Tree)children[i]).GetNodesWithTag(tag, nodes);
					}
					else if (children[i].tags.Contains(tag))
					{
						nodes.Add(children[i]);
					}
				}
			}
		}

		public void GetNodesWithoutTag(Tag tag, List<Node> nodes)
		{
			if (children.Count == 0 && !tags.Contains(tag))
			{
				nodes.Add(this);
			}
			else
			{
				for (int i = 0; i < children.Count; i++)
				{
					if (children[i].type == NodeType.Internal)
					{
						((Tree)children[i]).GetNodesWithoutTag(tag, nodes);
					}
					else if (!children[i].tags.Contains(tag))
					{
						nodes.Add(children[i]);
					}
				}
			}
		}
	}
}
