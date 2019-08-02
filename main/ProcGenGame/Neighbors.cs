using KSerialization;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public struct Neighbors
	{
		public TerrainCell n0;

		public TerrainCell n1;

		public Neighbors(TerrainCell a, TerrainCell b)
		{
			Debug.Assert(a != null && b != null, "NULL Neighbor");
			n0 = a;
			n1 = b;
		}
	}
}
