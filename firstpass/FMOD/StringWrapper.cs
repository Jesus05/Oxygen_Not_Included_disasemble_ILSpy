using System;

namespace FMOD
{
	public struct StringWrapper
	{
		private IntPtr nativeUtf8Ptr;

		public static implicit operator string(StringWrapper fstring)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return threadSafeEncoding.stringFromNative(fstring.nativeUtf8Ptr);
			}
		}
	}
}
