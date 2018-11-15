using KSerialization;
using Satsuma;
using System.Collections.Generic;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Cell : Node
	{
		public List<Cell> neighbors;

		public List<Edge> edges;

		public List<Corner> corners;

		public Cell()
		{
			SetType(WorldGenTags.Cell.Name);
			Init();
		}

		public Cell(Satsuma.Node node)
			: base(node, WorldGenTags.Cell.Name)
		{
			Init();
		}

		public Cell(Node node)
			: base(node.node, WorldGenTags.Cell.Name)
		{
			Init();
		}

		private void Init()
		{
			edges = new List<Edge>();
			corners = new List<Corner>();
			neighbors = new List<Cell>();
			tags = new TagSet();
		}

		public void Add(Edge e)
		{
			if (edges.Find((Edge edge) => (e.corner0 == edge.corner0 && e.corner1 == edge.corner1) || (e.corner1 == edge.corner0 && e.corner0 == edge.corner1)) == null)
			{
				edges.Add(e);
			}
		}

		public void Remove(Edge e)
		{
			edges.Remove(e);
			e.site0.neighbors.Remove(e.site1);
			e.site1.neighbors.Remove(e.site0);
		}

		public void Add(Corner c)
		{
			if (!corners.Contains(c))
			{
				corners.Add(c);
			}
		}

		public void Add(Cell c)
		{
			if (!neighbors.Contains(c))
			{
				neighbors.Add(c);
				c.Add(this);
			}
		}
	}
}
