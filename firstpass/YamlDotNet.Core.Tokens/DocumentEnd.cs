using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class DocumentEnd : Token
	{
		public DocumentEnd()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public DocumentEnd(Mark start, Mark end)
			: base(start, end)
		{
		}
	}
}
