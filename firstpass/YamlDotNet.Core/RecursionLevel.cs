using System;

namespace YamlDotNet.Core
{
	internal class RecursionLevel
	{
		private int current;

		public int Maximum
		{
			get;
			private set;
		}

		public RecursionLevel(int maximum)
		{
			Maximum = maximum;
		}

		public void Increment()
		{
			if (!TryIncrement())
			{
				throw new MaximumRecursionLevelReachedException();
			}
		}

		public bool TryIncrement()
		{
			if (current < Maximum)
			{
				current++;
				return true;
			}
			return false;
		}

		public void Decrement()
		{
			if (current == 0)
			{
				throw new InvalidOperationException("Attempted to decrement RecursionLevel to a negative value");
			}
			current--;
		}
	}
}
