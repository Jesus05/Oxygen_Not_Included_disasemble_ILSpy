using System;

public class SeededRandom
{
	private Random rnd = null;

	public int seed
	{
		get;
		private set;
	}

	public SeededRandom(int seed)
	{
		if (seed == -2147483648)
		{
			seed = 0;
		}
		this.seed = seed;
		rnd = new Random(seed);
	}

	public Random RandomSource()
	{
		return rnd;
	}

	public float RandomValue()
	{
		return (float)rnd.NextDouble();
	}

	public double NextDouble()
	{
		return rnd.NextDouble();
	}

	public float RandomRange(float rangeLow, float rangeHigh)
	{
		float num = rangeHigh - rangeLow;
		return rangeLow + (float)(rnd.NextDouble() * (double)num);
	}

	public int RandomRange(int rangeLow, int rangeHigh)
	{
		int num = rangeHigh - rangeLow;
		return rangeLow + (int)(rnd.NextDouble() * (double)num);
	}
}
