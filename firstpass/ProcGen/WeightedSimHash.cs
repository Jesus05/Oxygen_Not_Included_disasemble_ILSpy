using System;

namespace ProcGen
{
	[Serializable]
	public class WeightedSimHash : IWeighted
	{
		public string element
		{
			get;
			private set;
		}

		public float weight
		{
			get;
			set;
		}

		public SampleDescriber.Override overrides
		{
			get;
			private set;
		}

		public WeightedSimHash()
		{
		}

		public WeightedSimHash(string elementHash, float weight, SampleDescriber.Override overrides = null)
		{
			element = elementHash;
			this.weight = weight;
			this.overrides = overrides;
		}
	}
}
