using KSerialization;
using Satsuma;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class MapGraph : Graph
	{
		[Serialize]
		public List<Cell> cellList;

		[Serialize]
		public List<Corner> cornerList;

		[Serialize]
		public List<Edge> edgeList;

		public List<Cell> cells => cellList;

		public List<Corner> corners => cornerList;

		public List<Edge> edges => edgeList;

		public MapGraph(int seed)
			: base(seed)
		{
			cellList = new List<Cell>();
			cornerList = new List<Corner>();
			edgeList = new List<Edge>();
		}

		public Edge GetEdge(Corner corner0, Corner corner1, bool createOK = true)
		{
			Edge edge = null;
			edge = edgeList.Find((Edge e) => (e.corner0 == corner0 || e.corner0 == corner1) && (e.corner1 == corner0 || e.corner1 == corner1));
			if (edge != null)
			{
				return edge;
			}
			if (!createOK)
			{
				Debug.LogWarning("Cant create Edge but no edge found");
				return null;
			}
			Satsuma.Arc arc = base.baseGraph.AddArc(corner0.node, corner1.node, Directedness.Undirected);
			edge = new Edge(arc, corner0, corner1);
			arcList.Add(edge);
			edgeList.Add(edge);
			return edge;
		}

		public Edge GetEdge(Corner corner0, Corner corner1, Cell site0, Cell site1, bool createOK = true)
		{
			Edge edge = null;
			edge = edgeList.Find((Edge e) => (e.corner0 == corner0 || e.corner0 == corner1) && (e.corner1 == corner0 || e.corner1 == corner1));
			if (edge != null)
			{
				return edge;
			}
			if (!createOK)
			{
				Debug.LogWarning("Cant create Edge but no edge found");
				return null;
			}
			Satsuma.Arc arc = base.baseGraph.AddArc(corner0.node, corner1.node, Directedness.Undirected);
			edge = new Edge(arc, corner0, corner1, site0, site1);
			arcList.Add(edge);
			edgeList.Add(edge);
			return edge;
		}

		public Corner GetCorner(Vector2 position, bool createOK = true)
		{
			position = new Vector2((float)(int)position.x, (float)(int)position.y);
			Corner corner = cornerList.Find((Corner c) => c.position == position);
			if (corner == null)
			{
				if (!createOK)
				{
					Debug.LogWarning("Cant create Corner but no corner found");
					return null;
				}
				corner = new Corner(base.baseGraph.AddNode());
				nodeList.Add(corner);
				corner.SetPosition(position);
				cornerList.Add(corner);
			}
			return corner;
		}

		public Cell GetCell(Satsuma.Node node)
		{
			return cellList.Find((Cell c) => c.node == node);
		}

		public Cell GetCell(Vector2 position)
		{
			position = new Vector2((float)(int)position.x, (float)(int)position.y);
			return cellList.Find((Cell c) => c.position == position);
		}

		public Cell GetCell(Vector2 position, Satsuma.Node node, bool createOK = true)
		{
			position = new Vector2((float)(int)position.x, (float)(int)position.y);
			Cell cell = cellList.Find((Cell c) => c.position == position);
			if (cell == null)
			{
				if (!createOK)
				{
					Debug.LogWarning("Cant create Cell but no cell found");
					return null;
				}
				cell = cellList.Find((Cell c) => c.node == node);
				if (cell == null)
				{
					cell = new Cell(node);
					cell.SetPosition(position);
					cellList.Add(cell);
				}
				else
				{
					Debug.LogWarning("GetCell Same node [" + node.Id + "] differnt position!");
				}
			}
			return cell;
		}

		public List<Edge> GetEdgesWithTag(Tag tag)
		{
			List<Edge> list = new List<Edge>();
			for (int i = 0; i < edgeList.Count; i++)
			{
				if (edgeList[i].tags.Contains(tag))
				{
					list.Add(edgeList[i]);
				}
			}
			return list;
		}

		public void Remove(Edge n)
		{
			n.site0.Remove(n);
			n.site1.Remove(n);
			edges.Remove(n);
		}

		public void Validate()
		{
			for (int i = 0; i < cellList.Count; i++)
			{
				for (int j = 0; j < cellList.Count; j++)
				{
					if (j != i)
					{
						if (cellList[i] == cellList[j])
						{
							Debug.LogError("Duplicate cell (class)");
							return;
						}
						if (cellList[i].position == cellList[j].position)
						{
							Debug.LogError("Duplicate cell (position)");
							return;
						}
						if (cellList[i].node == cellList[j].node)
						{
							Debug.LogError("Duplicate cell (node)");
							return;
						}
					}
				}
			}
			for (int k = 0; k < cornerList.Count; k++)
			{
				for (int l = 0; l < cornerList.Count; l++)
				{
					if (l != k)
					{
						if (cornerList[k] == cornerList[l])
						{
							Debug.LogError("Duplicate corner (class)");
							return;
						}
						if (cornerList[k].position == cornerList[l].position)
						{
							Debug.LogError("Duplicate corner (position)");
							return;
						}
						if (cornerList[k].node == cornerList[l].node)
						{
							Debug.LogError("Duplicate corner (node)");
							return;
						}
					}
				}
			}
			for (int m = 0; m < edgeList.Count; m++)
			{
				for (int n = 0; n < edgeList.Count; n++)
				{
					if (n != m)
					{
						Edge edge = edgeList[m];
						Edge edge2 = edgeList[n];
						if (edge == edge2)
						{
							Debug.LogError("Duplicate edge (class)");
							return;
						}
						if (edge.arc == edge2.arc)
						{
							Debug.LogError("Duplicate EDGE [" + edge.arc + "] & [" + edge2.arc + "] - (ARC) [" + edge.site0.node.Id + "] &  [" + edge.site1.node.Id + "]");
							return;
						}
						if (edge.corner0 == edge2.corner0 && edge.corner1 == edge2.corner1)
						{
							Debug.LogError("Duplicate edge (corner same order)");
							return;
						}
						if (edge.corner0 == edge2.corner1 && edge.corner1 == edge2.corner0)
						{
							Debug.LogError("Duplicate edge (corner different order)");
							return;
						}
						if (edge.site0 != edge.site1 && edge2.site0 != edge2.site1)
						{
							if (edge.site0 == edge2.site0 && edge.site1 == edge2.site1)
							{
								Debug.LogError("Duplicate edge (site same order)");
								return;
							}
							if (edge.site0 == edge2.site1 && edge.site1 == edge2.site0)
							{
								Debug.LogError("Duplicate Edge [" + edge.arc.Id + "] -> [" + edge.corner0.node.Id + "<-->" + edge.corner1.node.Id + "] sites: [" + edge.site0.node.Id + " -- " + edge.site1.node.Id + "] and [" + edge2.arc.Id + "] -> [" + edge2.corner0.node.Id + "<-->" + edge2.corner1.node.Id + "] sites: [" + edge2.site0.node.Id + " -- " + edge2.site1.node.Id + "] - (site differnt order)");
								Debug.Log("CE 0: " + edge.corner0.position + " 1: " + edge.corner1.position);
								Debug.Log("OE 0: " + edge2.corner0.position + " 1: " + edge2.corner1.position);
								Debug.Log("Sites C 0: " + edge.site0.position + " 1: " + edge.site1.position);
								DebugExtension.DebugCircle2d(edge.site0.position, Color.red, 1f, 15f, true, 4f);
								DebugExtension.DebugCircle2d(edge.site1.position, Color.magenta, 2f, 15f, true, 4f);
								Debug.Log("Sites O 0: " + edge2.site0.position + " 1: " + edge2.site1.position);
								DebugExtension.DebugCircle2d(edge2.site0.position, Color.green, 3f, 15f, true, 4f);
								DebugExtension.DebugCircle2d(edge2.site1.position, Color.cyan, 4f, 15f, true, 4f);
							}
							else
							{
								if (edge.site0.node == edge2.site0.node && edge.site1.node == edge2.site1.node)
								{
									Debug.LogError("Duplicate edge (site node same order)");
									return;
								}
								if (edge.site1.node == edge2.site0.node && edge.site0.node == edge2.site1.node)
								{
									Debug.LogError("Duplicate edge (site node differnt order)");
									return;
								}
							}
						}
					}
				}
			}
		}

		[OnDeserialized]
		internal new void OnDeserializedMethod()
		{
			try
			{
				base.OnDeserializedMethod();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				string stackTrace = ex.StackTrace;
				Debug.Log("Error deserialising " + message + "\n" + stackTrace);
			}
		}
	}
}
