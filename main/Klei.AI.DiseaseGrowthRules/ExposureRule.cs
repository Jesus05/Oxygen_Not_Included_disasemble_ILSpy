using System.Collections.Generic;

namespace Klei.AI.DiseaseGrowthRules
{
	public class ExposureRule
	{
		public float? populationHalfLife;

		public void Apply(ElemExposureInfo[] infoList)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				if (Test(elements[i]))
				{
					ElemExposureInfo elemExposureInfo = infoList[i];
					float? nullable = populationHalfLife;
					if (nullable.HasValue)
					{
						float? nullable2 = populationHalfLife;
						elemExposureInfo.populationHalfLife = nullable2.Value;
					}
					infoList[i] = elemExposureInfo;
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
