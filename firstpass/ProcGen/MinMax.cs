namespace ProcGen
{
	public struct MinMax
	{
		public float min
		{
			get;
			private set;
		}

		public float max
		{
			get;
			private set;
		}

		public float GetRandomValueWithinRange(SeededRandom rnd)
		{
			return rnd.RandomRange(min, max);
		}
	}
}
