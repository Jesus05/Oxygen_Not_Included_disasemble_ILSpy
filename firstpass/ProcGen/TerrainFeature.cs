using System.Collections.Generic;

namespace ProcGen
{
	public class TerrainFeature : SampleDescriber
	{
		public string type
		{
			get;
			private set;
		}

		public Feature defaultBiome
		{
			get;
			private set;
		}

		public List<string> tags
		{
			get;
			private set;
		}

		public List<string> excludesTags
		{
			get;
			private set;
		}

		public TerrainFeature()
		{
			tags = new List<string>();
			excludesTags = new List<string>();
			defaultBiome = new Feature();
		}
	}
}
