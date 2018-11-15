using System;
using System.Collections.Generic;

public static class WorldGenUtil
{
	public static void ShuffleSeeded<T>(this IList<T> list, Random rng)
	{
		int num = ((ICollection<T>)list).Count;
		while (num > 1)
		{
			num--;
			int index = rng.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
