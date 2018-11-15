using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Tag : Token
	{
		private readonly string handle;

		private readonly string suffix;

		public string Handle => handle;

		public string Suffix => suffix;

		public Tag(string handle, string suffix)
			: this(handle, suffix, Mark.Empty, Mark.Empty)
		{
		}

		public Tag(string handle, string suffix, Mark start, Mark end)
			: base(start, end)
		{
			this.handle = handle;
			this.suffix = suffix;
		}
	}
}
