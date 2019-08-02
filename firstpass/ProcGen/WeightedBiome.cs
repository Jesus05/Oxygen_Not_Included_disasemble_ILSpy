using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class WeightedBiome : IWeighted
	{
		public string name
		{
			get;
			private set;
		}

		public float weight
		{
			get;
			set;
		}

		public List<string> tags
		{
			get;
			private set;
		}

		public WeightedBiome()
		{
			tags = new List<string>();
		}

		public WeightedBiome(string name, float weight)
			: this()
		{
			this.name = name;
			this.weight = weight;
		}
	}
}
