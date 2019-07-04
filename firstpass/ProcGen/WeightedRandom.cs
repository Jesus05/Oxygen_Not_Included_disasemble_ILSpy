using System.Collections.Generic;

namespace ProcGen
{
	public static class WeightedRandom
	{
		public static T Choose<T>(List<T> list, SeededRandom rand) where T : IWeighted
		{
			T result;
			if (list.Count != 0)
			{
				float num = 0f;
				for (int i = 0; i < list.Count; i++)
				{
					float num2 = num;
					result = list[i];
					num = num2 + result.weight;
				}
				float num3 = rand.RandomValue() * num;
				float num4 = 0f;
				for (int j = 0; j < list.Count; j++)
				{
					num4 += list[j].weight;
					if (num4 > num3)
					{
						return list[j];
					}
				}
				return list[list.Count - 1];
			}
			result = default(T);
			return result;
		}
	}
}
