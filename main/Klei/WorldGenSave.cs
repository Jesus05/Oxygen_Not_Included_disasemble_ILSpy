using System.Collections.Generic;

namespace Klei
{
	public class WorldGenSave
	{
		public Vector2I version;

		public Dictionary<string, object> stats;

		public Data data;

		public WorldGenSave()
		{
			data = new Data();
			stats = new Dictionary<string, object>();
		}
	}
}
