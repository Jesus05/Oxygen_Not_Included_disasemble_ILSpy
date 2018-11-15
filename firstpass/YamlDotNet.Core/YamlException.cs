using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class YamlException : Exception
	{
		public Mark Start
		{
			get;
			private set;
		}

		public Mark End
		{
			get;
			private set;
		}

		public YamlException()
		{
		}

		public YamlException(string message)
			: base(message)
		{
		}

		public YamlException(Mark start, Mark end, string message)
			: this(start, end, message, null)
		{
		}

		public YamlException(Mark start, Mark end, string message, Exception innerException)
			: base($"({start}) - ({end}): {message}", innerException)
		{
			Start = start;
			End = end;
		}

		public YamlException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
