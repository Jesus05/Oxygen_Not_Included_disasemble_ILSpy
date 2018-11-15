using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class MaximumRecursionLevelReachedException : YamlException
	{
		public MaximumRecursionLevelReachedException()
		{
		}

		public MaximumRecursionLevelReachedException(string message)
			: base(message)
		{
		}

		public MaximumRecursionLevelReachedException(Mark start, Mark end, string message)
			: base(start, end, message)
		{
		}

		public MaximumRecursionLevelReachedException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
