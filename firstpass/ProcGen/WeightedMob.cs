namespace ProcGen
{
	public class WeightedMob : IWeighted
	{
		public float weight
		{
			get;
			set;
		}

		public string tag
		{
			get;
			private set;
		}

		public WeightedMob()
		{
		}

		public WeightedMob(string tag, float weight)
		{
			this.tag = tag;
			this.weight = weight;
		}
	}
}
