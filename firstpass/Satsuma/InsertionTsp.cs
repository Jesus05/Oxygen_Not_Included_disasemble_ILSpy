using System;
using System.Collections.Generic;
using System.Linq;

namespace Satsuma
{
	public sealed class InsertionTsp<TNode> : ITsp<TNode>
	{
		private LinkedList<TNode> tour;

		private Dictionary<TNode, LinkedListNode<TNode>> tourNodes;

		private HashSet<TNode> insertableNodes;

		private PriorityQueue<TNode, double> insertableNodeQueue;

		public IEnumerable<TNode> Nodes
		{
			get;
			private set;
		}

		public Func<TNode, TNode, double> Cost
		{
			get;
			private set;
		}

		public TspSelectionRule SelectionRule
		{
			get;
			private set;
		}

		public IEnumerable<TNode> Tour => tour;

		public double TourCost
		{
			get;
			private set;
		}

		public InsertionTsp(IEnumerable<TNode> nodes, Func<TNode, TNode, double> cost, TspSelectionRule selectionRule = TspSelectionRule.Farthest)
		{
			Nodes = nodes;
			Cost = cost;
			SelectionRule = selectionRule;
			tour = new LinkedList<TNode>();
			tourNodes = new Dictionary<TNode, LinkedListNode<TNode>>();
			insertableNodes = new HashSet<TNode>();
			insertableNodeQueue = new PriorityQueue<TNode, double>();
			Clear();
		}

		private double PriorityFromCost(double c)
		{
			TspSelectionRule selectionRule = SelectionRule;
			if (selectionRule == TspSelectionRule.Farthest)
			{
				return 0.0 - c;
			}
			return c;
		}

		public void Clear()
		{
			tour.Clear();
			TourCost = 0.0;
			tourNodes.Clear();
			insertableNodes.Clear();
			insertableNodeQueue.Clear();
			if (Enumerable.Any<TNode>(Nodes))
			{
				TNode val = Enumerable.First<TNode>(Nodes);
				tour.AddFirst(val);
				tourNodes[val] = tour.AddFirst(val);
				foreach (TNode node in Nodes)
				{
					if (!node.Equals(val))
					{
						insertableNodes.Add(node);
						insertableNodeQueue[node] = PriorityFromCost(Cost(val, node));
					}
				}
			}
		}

		public bool Insert(TNode node)
		{
			if (!insertableNodes.Contains(node))
			{
				return false;
			}
			insertableNodes.Remove(node);
			insertableNodeQueue.Remove(node);
			LinkedListNode<TNode> node2 = null;
			double num = double.PositiveInfinity;
			for (LinkedListNode<TNode> linkedListNode = tour.First; linkedListNode != tour.Last; linkedListNode = linkedListNode.Next)
			{
				LinkedListNode<TNode> next = linkedListNode.Next;
				double num2 = Cost(linkedListNode.Value, node) + Cost(node, next.Value);
				if (linkedListNode != next)
				{
					num2 -= Cost(linkedListNode.Value, next.Value);
				}
				if (num2 < num)
				{
					num = num2;
					node2 = linkedListNode;
				}
			}
			tourNodes[node] = tour.AddAfter(node2, node);
			TourCost += num;
			foreach (TNode insertableNode in insertableNodes)
			{
				double num3 = PriorityFromCost(Cost(node, insertableNode));
				if (num3 < insertableNodeQueue[insertableNode])
				{
					insertableNodeQueue[insertableNode] = num3;
				}
			}
			return true;
		}

		public bool Insert()
		{
			if (insertableNodes.Count == 0)
			{
				return false;
			}
			Insert(insertableNodeQueue.Peek());
			return true;
		}

		public void Run()
		{
			while (Insert())
			{
			}
		}
	}
}
