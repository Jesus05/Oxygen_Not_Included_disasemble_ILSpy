using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public abstract class Token
	{
		private readonly Mark start;

		private readonly Mark end;

		public Mark Start => start;

		public Mark End => end;

		protected Token(Mark start, Mark end)
		{
			this.start = start;
			this.end = end;
		}
	}
}
