using Delaunay.Geo;
using KSerialization;
using Satsuma;
using Satsuma.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Graph
	{
		[Serialize]
		public List<Node> nodeList;

		[Serialize]
		public List<Arc> arcList;

		private SeededRandom myRandom;

		public List<Node> nodes => nodeList;

		public List<Arc> arcs => arcList;

		public CustomGraph baseGraph
		{
			get;
			private set;
		}

		public Graph(int seed)
		{
			SetSeed(seed);
			nodeList = new List<Node>();
			arcList = new List<Arc>();
			baseGraph = new CustomGraph();
		}

		public void SetSeed(int seed)
		{
			myRandom = new SeededRandom(seed);
		}

		public Node AddNode(string type)
		{
			Node node = new Node(baseGraph.AddNode(), type);
			nodeList.Add(node);
			return node;
		}

		public void Remove(Node n)
		{
			baseGraph.DeleteNode(n.node);
			nodes.Remove(n);
		}

		public Arc AddArc(Node nodeA, Node nodeB, string type)
		{
			Satsuma.Arc arc = baseGraph.AddArc(nodeA.node, nodeB.node, Directedness.Undirected);
			Arc arc2 = new Arc(arc, type);
			arcList.Add(arc2);
			return arc2;
		}

		public Node FindNodeByID(uint id)
		{
			return nodeList.Find((Node node) => node.node.Id == id);
		}

		public Arc FindArcByID(uint id)
		{
			return arcList.Find((Arc arc) => arc.arc.Id == id);
		}

		public Node FindNode(Predicate<Node> pred)
		{
			return nodeList.Find(pred);
		}

		public Arc FindArc(Predicate<Arc> pred)
		{
			return arcList.Find(pred);
		}

		public int GetDistanceToTagSetFromNode(Node node, TagSet tagset)
		{
			List<Node> nodesWithAtLeastOneTag = GetNodesWithAtLeastOneTag(tagset);
			if (nodesWithAtLeastOneTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(baseGraph, (Satsuma.Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithAtLeastOneTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithAtLeastOneTag[i].node);
				}
				dijkstra.RunUntilFixed(node.node);
				return (int)dijkstra.GetDistance(node.node);
			}
			return -1;
		}

		public int GetDistanceToTagFromNode(Node node, Tag tag)
		{
			List<Node> nodesWithTag = GetNodesWithTag(tag);
			if (nodesWithTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(baseGraph, (Satsuma.Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithTag[i].node);
				}
				dijkstra.RunUntilFixed(node.node);
				return (int)dijkstra.GetDistance(node.node);
			}
			return -1;
		}

		public Dictionary<uint, int> GetDistanceToTag(Tag tag)
		{
			List<Node> nodesWithTag = GetNodesWithTag(tag);
			if (nodesWithTag.Count > 0)
			{
				Dijkstra dijkstra = new Dijkstra(baseGraph, (Satsuma.Arc arc) => 1.0, DijkstraMode.Sum);
				for (int i = 0; i < nodesWithTag.Count; i++)
				{
					dijkstra.AddSource(nodesWithTag[i].node);
				}
				Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
				for (int j = 0; j < nodes.Count; j++)
				{
					dijkstra.RunUntilFixed(nodes[j].node);
					dictionary[(uint)nodes[j].node.Id] = (int)dijkstra.GetDistance(nodes[j].node);
				}
				return dictionary;
			}
			return null;
		}

		public List<Node> GetNodesWithAtLeastOneTag(TagSet tagset)
		{
			return nodeList.FindAll((Node node) => node.tags.ContainsOne(tagset));
		}

		public List<Node> GetNodesWithTag(Tag tag)
		{
			return nodeList.FindAll((Node node) => node.tags.Contains(tag));
		}

		public List<Arc> GetArcsWithTag(Tag tag)
		{
			return arcList.FindAll((Arc arc) => arc.tags.Contains(tag));
		}

		[OnDeserialized]
		internal void OnDeserializedMethod()
		{
			try
			{
				for (int i = 0; i < nodeList.Count; i++)
				{
					Node node = new Node(baseGraph.AddNode(), nodeList[i].type);
					node.SetPosition(nodeList[i].position);
					nodeList[i] = node;
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				Debug.Log("Error deserialising " + message + "\n" + stackTrace);
			}
		}

		public static PointD GetForceForBoundry(PointD particle, Polygon bounds)
		{
			Vector2 point = new Vector2((float)particle.X, (float)particle.Y);
			List<KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>>> edgesWithinDistance = bounds.GetEdgesWithinDistance(point, 3.40282347E+38f);
			double num = 0.0;
			double num2 = 0.0;
			for (int i = 0; i < edgesWithinDistance.Count; i++)
			{
				KeyValuePair<MathUtil.Pair<float, float>, MathUtil.Pair<Vector2, Vector2>> keyValuePair = edgesWithinDistance[i];
				MathUtil.Pair<Vector2, Vector2> value = keyValuePair.Value;
				float second = keyValuePair.Key.Second;
				double num3 = (double)keyValuePair.Key.First;
				Vector2 vector = value.First + (value.Second - value.First) * second;
				PointD pointD = new PointD((double)vector.x, (double)vector.y);
				double num4 = 1.0 / (num3 * num3);
				num += (particle.X - pointD.X) / num3 * num4;
				num2 += (particle.Y - pointD.Y) / num3 * num4;
			}
			if (bounds.Contains(point))
			{
				return new PointD(num, num2);
			}
			return new PointD(0.0 - num, 0.0 - num2);
		}

		public PointD GetPositionForNode(Satsuma.Node node)
		{
			Node node2 = nodeList.Find((Node n) => n.node == node);
			Vector2 position = node2.position;
			double x = (double)position.x;
			Vector2 position2 = node2.position;
			return new PointD(x, (double)position2.y);
		}

		public void SetInitialNodePositions(Polygon bounds)
		{
			List<Vector2> randomPoints = PointGenerator.GetRandomPoints(bounds, 50f, 0f, null, PointGenerator.SampleBehaviour.PoissonDisk, true, myRandom, true, true);
			int num = 0;
			for (int i = 0; i < nodeList.Count; i++)
			{
				if (num == randomPoints.Count - 1)
				{
					randomPoints = PointGenerator.GetRandomPoints(bounds, 10f, 20f, randomPoints, PointGenerator.SampleBehaviour.PoissonDisk, true, myRandom, true, true);
					num = 0;
				}
				nodeList[i].SetPosition(randomPoints[num++]);
			}
		}

		public bool Layout(Polygon bounds = null)
		{
			bool flag = false;
			int num = 0;
			Vector2 vector = default(Vector2);
			while (!flag && num < 100)
			{
				flag = true;
				Func<Satsuma.Node, PointD> initialPositions = (Satsuma.Node n) => GetPositionForNode(n);
				CustomGraph baseGraph = this.baseGraph;
				int seed = num;
				ForceDirectedLayout forceDirectedLayout = new ForceDirectedLayout(baseGraph, initialPositions, seed);
				forceDirectedLayout.ExternalForce = ((PointD point) => GetForceForBoundry(point, bounds));
				forceDirectedLayout.Run(0.01);
				IEnumerator<Satsuma.Node> enumerator = this.baseGraph.Nodes().GetEnumerator();
				int num2 = 0;
				while (enumerator.MoveNext())
				{
					Satsuma.Node node = enumerator.Current;
					Node node2 = nodeList.Find((Node n) => n.node == node);
					if (node2 != null)
					{
						vector.x = (float)forceDirectedLayout.NodePositions[node].X;
						vector.y = (float)forceDirectedLayout.NodePositions[node].Y;
						if (!bounds.Contains(vector))
						{
							flag = false;
							Debug.LogWarning("Re-doing layout - cell was off map");
							break;
						}
						node2.SetPosition(vector);
					}
					if (!flag)
					{
						break;
					}
					num2++;
				}
				num++;
			}
			if (num >= 10)
			{
				Debug.LogWarning("Re-ran layout " + num + " times");
			}
			return flag;
		}
	}
}
