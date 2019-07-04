using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class TerrainElementBandSettings
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
