using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class TerrainFeatureSettings : YamlIO<TerrainFeatureSettings>
	{
		public Dictionary<string, TerrainFeature> TerrainFeatures
		{
			get;
			private set;
		}

		public TerrainFeatureSettings()
		{
			TerrainFeatures = new Dictionary<string, TerrainFeature>();
		}

		public string[] GetNames()
		{
			string[] array = new string[TerrainFeatures.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, TerrainFeature> terrainFeature in TerrainFeatures)
			{
				array[num++] = terrainFeature.Key;
			}
			return array;
		}
	}
}
