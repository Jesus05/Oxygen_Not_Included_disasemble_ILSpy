using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class BlockEntry : Token
	{
		public BlockEntry()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public BlockEntry(Mark start, Mark end)
			: base(start, end)
		{
		}
	}
}
