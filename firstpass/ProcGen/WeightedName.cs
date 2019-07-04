using System;

namespace ProcGen
{
	[Serializable]
	public class WeightedName : IWeighted
	{
		public string name
		{
			get;
			private set;
		}

		public string overrideName
		{
			get;
			private set;
		}

		public float weight
		{
			get;
			set;
		}

		public WeightedName()
		{
			weight = 1f;
		}

		public WeightedName(string name, float weight)
		{
			this.name = name;
			this.weight = weight;
		}
	}
}
