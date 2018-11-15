using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class Scalar : Token
	{
		private readonly string value;

		private readonly ScalarStyle style;

		public string Value => value;

		public ScalarStyle Style => style;

		public Scalar(string value)
			: this(value, ScalarStyle.Any)
		{
		}

		public Scalar(string value, ScalarStyle style)
			: this(value, style, Mark.Empty, Mark.Empty)
		{
		}

		public Scalar(string value, ScalarStyle style, Mark start, Mark end)
			: base(start, end)
		{
			this.value = value;
			this.style = style;
		}
	}
}
