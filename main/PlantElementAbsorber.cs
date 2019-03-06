public struct PlantElementAbsorber
{
	public struct ConsumeInfo
	{
		public Tag tag;

		public float massConsumptionRate;

		public ConsumeInfo(Tag tag, float mass_consumption_rate)
		{
			this.tag = tag;
			massConsumptionRate = mass_consumption_rate;
		}
	}

	public struct LocalInfo
	{
		public Tag tag;

		public float massConsumptionRate;
	}

	public Storage storage;

	public LocalInfo localInfo;

	public HandleVector<int>.Handle[] accumulators;

	public ConsumeInfo[] consumedElements;

	public void Clear()
	{
		storage = null;
		consumedElements = null;
	}
}
