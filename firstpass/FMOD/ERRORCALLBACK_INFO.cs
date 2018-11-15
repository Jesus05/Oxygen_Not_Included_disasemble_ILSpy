using System;

namespace FMOD
{
	public struct ERRORCALLBACK_INFO
	{
		public RESULT result;

		public ERRORCALLBACK_INSTANCETYPE instancetype;

		public IntPtr instance;

		public StringWrapper functionname;

		public StringWrapper functionparams;
	}
}
