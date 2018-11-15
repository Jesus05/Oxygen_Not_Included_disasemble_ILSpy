namespace Satsuma
{
	internal abstract class IdAllocator
	{
		private long randomSeed;

		private long lastAllocated;

		public IdAllocator()
		{
			randomSeed = 205891132094649L;
			Rewind();
		}

		private long Random()
		{
			return randomSeed *= 3L;
		}

		protected abstract bool IsAllocated(long id);

		public void Rewind()
		{
			lastAllocated = 0L;
		}

		public long Allocate()
		{
			long num = lastAllocated + 1;
			int num2 = 0;
			while (true)
			{
				if (num == 0)
				{
					num = 1L;
				}
				if (!IsAllocated(num))
				{
					break;
				}
				num++;
				num2++;
				if (num2 >= 100)
				{
					num = Random();
					num2 = 0;
				}
			}
			lastAllocated = num;
			return num;
		}
	}
}
