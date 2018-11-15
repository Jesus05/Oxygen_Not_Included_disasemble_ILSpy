using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class BiomeSettings : YamlIO<BiomeSettings>
	{
		public Dictionary<string, ElementBandConfiguration> TerrainBiomeLookupTable
		{
			get;
			private set;
		}

		public BiomeSettings()
		{
			TerrainBiomeLookupTable = new Dictionary<string, ElementBandConfiguration>();
		}

		public string[] GetNames()
		{
			string[] array = new string[TerrainBiomeLookupTable.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, ElementBandConfiguration> item in TerrainBiomeLookupTable)
			{
				array[num++] = item.Key;
			}
			return array;
		}
	}
}
