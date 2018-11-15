using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class ForwardAnchorNotSupportedException : YamlException
	{
		public ForwardAnchorNotSupportedException()
		{
		}

		public ForwardAnchorNotSupportedException(string message)
			: base(message)
		{
		}

		public ForwardAnchorNotSupportedException(Mark start, Mark end, string message)
			: base(start, end, message)
		{
		}

		public ForwardAnchorNotSupportedException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
