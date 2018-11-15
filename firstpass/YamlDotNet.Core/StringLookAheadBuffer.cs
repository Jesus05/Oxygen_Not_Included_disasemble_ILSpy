using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class StringLookAheadBuffer : ILookAheadBuffer
	{
		private readonly string value;

		public int Position
		{
			get;
			private set;
		}

		public int Length => value.Length;

		public bool EndOfInput => IsOutside(Position);

		public StringLookAheadBuffer(string value)
		{
			this.value = value;
		}

		public char Peek(int offset)
		{
			int index = Position + offset;
			return (!IsOutside(index)) ? value[index] : '\0';
		}

		private bool IsOutside(int index)
		{
			return index >= value.Length;
		}

		public void Skip(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "The length must be positive.");
			}
			Position += length;
		}
	}
}
