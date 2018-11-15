using System;

namespace FMOD.Studio
{
	public struct SOUND_INFO
	{
		public IntPtr name_or_data;

		public MODE mode;

		public CREATESOUNDEXINFO exinfo;

		public int subsoundindex;

		public string name
		{
			get
			{
				using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
				{
					return ((mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != 0) ? string.Empty : threadSafeEncoding.stringFromNative(name_or_data);
				}
			}
		}
	}
}
