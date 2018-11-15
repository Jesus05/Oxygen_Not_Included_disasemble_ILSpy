using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class TerrainElementBandSettings : YamlIO<TerrainElementBandSettings>
	{
		public Dictionary<string, ElementBandConfiguration> BiomeBackgroundElementBandConfigurations
		{
			get;
			private set;
		}

		public TerrainElementBandSettings()
		{
			BiomeBackgroundElementBandConfigurations = new Dictionary<string, ElementBandConfiguration>();
		}

		public string[] GetNames()
		{
			string[] array = new string[BiomeBackgroundElementBandConfigurations.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, ElementBandConfiguration> biomeBackgroundElementBandConfiguration in BiomeBackgroundElementBandConfigurations)
			{
				array[num++] = biomeBackgroundElementBandConfiguration.Key;
			}
			return array;
		}
	}
}
