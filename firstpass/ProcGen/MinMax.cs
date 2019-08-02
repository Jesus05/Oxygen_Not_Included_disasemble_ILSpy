using System;

namespace ProcGen
{
	[Serializable]
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

		public MinMax(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float GetRandomValueWithinRange(SeededRandom rnd)
		{
			return rnd.RandomRange(min, max);
		}

		public float GetAverage()
		{
			return (min + max) / 2f;
		}

		public void Mod(MinMax mod)
		{
			min += mod.min;
			max += mod.max;
		}

		public override string ToString()
		{
			return $"min:{min} max:{max}";
		}
	}
}
