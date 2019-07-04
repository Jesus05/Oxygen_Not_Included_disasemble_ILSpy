using System.Collections.Generic;

namespace ProcGen
{
	public class BiomeSettings : IMerge<BiomeSettings>
	{
		public ComposableDictionary<string, ElementBandConfiguration> TerrainBiomeLookupTable
		{
			get;
			private set;
		}

		public BiomeSettings()
		{
			TerrainBiomeLookupTable = new ComposableDictionary<string, ElementBandConfiguration>();
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

		public void Merge(BiomeSettings other)
		{
			TerrainBiomeLookupTable.Merge(other.TerrainBiomeLookupTable);
		}
	}
}
