using KSerialization;
using Satsuma;
using System.Collections.Generic;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Corner : Node
	{
		public List<Edge> edges;

		public List<Cell> cells;

		public Corner()
			: base(WorldGenTags.Corner.Name)
		{
			Init();
		}

		public Corner(Satsuma.Node node)
			: base(node, WorldGenTags.Corner.Name)
		{
			Init();
		}

		private void Init()
		{
			edges = new List<Edge>();
			cells = new List<Cell>();
			tags = new TagSet();
		}

		public void Add(Edge e)
		{
			if (edges.Find((Edge edge) => (e.corner0 == edge.corner0 && e.corner1 == edge.corner1) || (e.corner1 == edge.corner0 && e.corner0 == edge.corner1)) == null)
			{
				edges.Add(e);
				if (e.site0 != null && e.site0.position == base.position && cells.Find((Cell site) => e.site0 == site) == null)
				{
					cells.Add(e.site0);
					e.site0.Add(this);
				}
				if (e.site1 != null && e.site1.position == base.position && cells.Find((Cell site) => e.site1 == site) == null)
				{
					cells.Add(e.site1);
					e.site1.Add(this);
				}
			}
		}
	}
}
