namespace Klei.AI.DiseaseGrowthRules
{
	public class CompositeExposureRule
	{
		public string name;

		public float populationHalfLife;

		public string Name()
		{
			return name;
		}

		public void Overlay(ExposureRule rule)
		{
			float? nullable = rule.populationHalfLife;
			if (nullable.HasValue)
			{
				float? nullable2 = rule.populationHalfLife;
				populationHalfLife = nullable2.Value;
			}
			name = rule.Name();
		}

		public float GetHalfLifeForCount(int count)
		{
			return populationHalfLife;
		}
	}
}
