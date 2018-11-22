namespace Klei.AI.DiseaseGrowthRules
{
	public class CompositeGrowthRule
	{
		public string name;

		public float underPopulationDeathRate;

		public float populationHalfLife;

		public float overPopulationHalfLife;

		public float diffusionScale;

		public float minCountPerKG;

		public float maxCountPerKG;

		public int minDiffusionCount;

		public byte minDiffusionInfestationTickCount;

		public string Name()
		{
			return name;
		}

		public void Overlay(GrowthRule rule)
		{
			float? nullable = rule.underPopulationDeathRate;
			if (nullable.HasValue)
			{
				float? nullable2 = rule.underPopulationDeathRate;
				underPopulationDeathRate = nullable2.Value;
			}
			float? nullable3 = rule.populationHalfLife;
			if (nullable3.HasValue)
			{
				float? nullable4 = rule.populationHalfLife;
				populationHalfLife = nullable4.Value;
			}
			float? nullable5 = rule.overPopulationHalfLife;
			if (nullable5.HasValue)
			{
				float? nullable6 = rule.overPopulationHalfLife;
				overPopulationHalfLife = nullable6.Value;
			}
			float? nullable7 = rule.diffusionScale;
			if (nullable7.HasValue)
			{
				float? nullable8 = rule.diffusionScale;
				diffusionScale = nullable8.Value;
			}
			float? nullable9 = rule.minCountPerKG;
			if (nullable9.HasValue)
			{
				float? nullable10 = rule.minCountPerKG;
				minCountPerKG = nullable10.Value;
			}
			float? nullable11 = rule.maxCountPerKG;
			if (nullable11.HasValue)
			{
				float? nullable12 = rule.maxCountPerKG;
				maxCountPerKG = nullable12.Value;
			}
			int? nullable13 = rule.minDiffusionCount;
			if (nullable13.HasValue)
			{
				int? nullable14 = rule.minDiffusionCount;
				minDiffusionCount = nullable14.Value;
			}
			byte? nullable15 = rule.minDiffusionInfestationTickCount;
			if (nullable15.HasValue)
			{
				byte? nullable16 = rule.minDiffusionInfestationTickCount;
				minDiffusionInfestationTickCount = nullable16.Value;
			}
			name = rule.Name();
		}

		public float GetHalfLifeForCount(int count, float kg)
		{
			int num = (int)(minCountPerKG * kg);
			int num2 = (int)(maxCountPerKG * kg);
			if (count >= num)
			{
				if (count >= num2)
				{
					return overPopulationHalfLife;
				}
				return populationHalfLife;
			}
			return populationHalfLife;
		}
	}
}
