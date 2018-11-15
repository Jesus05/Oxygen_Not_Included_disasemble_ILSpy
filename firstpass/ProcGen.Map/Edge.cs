using KSerialization;
using Satsuma;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Edge : Arc
	{
		public Corner corner0;

		public Corner corner1;

		public Cell site0;

		public Cell site1;

		public Edge()
			: base(WorldGenTags.Edge.Name)
		{
			tags = new TagSet();
		}

		public Edge(Corner c0, Corner c1)
			: base(WorldGenTags.Edge.Name)
		{
			corner0 = c0;
			corner1 = c1;
			c0.Add(this);
			c1.Add(this);
			tags = new TagSet();
		}

		public Edge(Satsuma.Arc arc, Corner c0, Corner c1)
			: base(arc, WorldGenTags.Edge.Name)
		{
			corner0 = c0;
			corner1 = c1;
			c0.Add(this);
			c1.Add(this);
			tags = new TagSet();
		}

		public Edge(Satsuma.Arc arc, Corner c0, Corner c1, Cell s0, Cell s1)
			: base(arc, WorldGenTags.Edge.Name)
		{
			corner0 = c0;
			corner1 = c1;
			site0 = s0;
			site1 = s1;
			c0.Add(this);
			c1.Add(this);
			s0.Add(this);
			s1.Add(this);
			tags = new TagSet();
		}

		public void SetSite0(Cell s0)
		{
			site0 = s0;
			s0.Add(this);
		}

		public void SetSite1(Cell s1)
		{
			site1 = s1;
			s1.Add(this);
		}

		public Vector2 MidPoint()
		{
			return (corner1.position - corner0.position) * 0.5f + corner0.position;
		}
	}
}
