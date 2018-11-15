using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Anchor : Token
	{
		private readonly string value;

		public string Value => value;

		public Anchor(string value)
			: this(value, Mark.Empty, Mark.Empty)
		{
		}

		public Anchor(string value, Mark start, Mark end)
			: base(start, end)
		{
			this.value = value;
		}
	}
}
