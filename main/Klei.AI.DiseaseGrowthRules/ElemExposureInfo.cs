using System;
using System.Collections.Generic;
using System.IO;

namespace Klei.AI.DiseaseGrowthRules
{
	public struct ElemExposureInfo
	{
		public float populationHalfLife;

		public void Write(BinaryWriter writer)
		{
			writer.Write(populationHalfLife);
		}

		public static void SetBulk(ElemExposureInfo[] info, Func<Element, bool> test, ElemExposureInfo settings)
		{
			List<Element> elements = ElementLoader.elements;
			for (int i = 0; i < elements.Count; i++)
			{
				if (test(elements[i]))
				{
					info[i] = settings;
				}
			}
		}

		public float CalculateExposureDiseaseCountDelta(int disease_count, float dt)
		{
			float num = Disease.HalfLifeToGrowthRate(populationHalfLife, dt);
			return (num - 1f) * (float)disease_count;
		}
	}
}
