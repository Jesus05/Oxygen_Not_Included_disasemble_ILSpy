using System;

namespace ClipperLib
{
	internal class ClipperException : Exception
	{
		public ClipperException(string description)
			: base(description)
		{
		}
	}
}
