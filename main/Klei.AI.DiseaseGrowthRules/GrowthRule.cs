using System.Collections.Generic;

namespace Klei.AI.DiseaseGrowthRules
{
	public class GrowthRule
	{
		public float? underPopulationDeathRate;

		public float? populationHalfLife;

		public float? overPopulationHalfLife;

		public float? diffusionScale;

		public float? minCountPerKG;

		public float? maxCountPerKG;

		public int? minDiffusionCount;

		public byte? minDiffusionInfestationTickCount;

		public void Apply(ElemGrowthInfo[] infoList)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				Element element = elements[i];
				if (element.id != SimHashes.Vacuum && Test(element))
				{
					ElemGrowthInfo elemGrowthInfo = infoList[i];
					float? nullable = underPopulationDeathRate;
					if (nullable.HasValue)
					{
						float? nullable2 = underPopulationDeathRate;
						elemGrowthInfo.underPopulationDeathRate = nullable2.Value;
					}
					float? nullable3 = populationHalfLife;
					if (nullable3.HasValue)
					{
						float? nullable4 = populationHalfLife;
						elemGrowthInfo.populationHalfLife = nullable4.Value;
					}
					float? nullable5 = overPopulationHalfLife;
					if (nullable5.HasValue)
					{
						float? nullable6 = overPopulationHalfLife;
						elemGrowthInfo.overPopulationHalfLife = nullable6.Value;
					}
					float? nullable7 = diffusionScale;
					if (nullable7.HasValue)
					{
						float? nullable8 = diffusionScale;
						elemGrowthInfo.diffusionScale = nullable8.Value;
					}
					float? nullable9 = minCountPerKG;
					if (nullable9.HasValue)
					{
						float? nullable10 = minCountPerKG;
						elemGrowthInfo.minCountPerKG = nullable10.Value;
					}
					float? nullable11 = maxCountPerKG;
					if (nullable11.HasValue)
					{
						float? nullable12 = maxCountPerKG;
						elemGrowthInfo.maxCountPerKG = nullable12.Value;
					}
					int? nullable13 = minDiffusionCount;
					if (nullable13.HasValue)
					{
						int? nullable14 = minDiffusionCount;
						elemGrowthInfo.minDiffusionCount = nullable14.Value;
					}
					byte? nullable15 = minDiffusionInfestationTickCount;
					if (nullable15.HasValue)
					{
						byte? nullable16 = minDiffusionInfestationTickCount;
						elemGrowthInfo.minDiffusionInfestationTickCount = nullable16.Value;
					}
					infoList[i] = elemGrowthInfo;
				}
			}
		}

		public virtual bool Test(Element e)
		{
			return true;
		}

		public virtual string Name()
		{
			return null;
		}
	}
}
