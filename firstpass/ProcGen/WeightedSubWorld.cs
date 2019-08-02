namespace ProcGen
{
	public class WeightedSubWorld : IWeighted
	{
		public SubWorld subWorld
		{
			get;
			set;
		}

		public float weight
		{
			get;
			set;
		}

		public WeightedSubWorld(float weight, SubWorld subWorld)
		{
			this.weight = weight;
			this.subWorld = subWorld;
		}

		public override int GetHashCode()
		{
			return subWorld.GetHashCode();
		}
	}
}
