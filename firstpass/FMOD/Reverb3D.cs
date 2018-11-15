using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct Reverb3D
	{
		public IntPtr handle;

		public RESULT release()
		{
			return FMOD5_Reverb3D_Release(handle);
		}

		public RESULT set3DAttributes(ref VECTOR position, float mindistance, float maxdistance)
		{
			return FMOD5_Reverb3D_Set3DAttributes(handle, ref position, mindistance, maxdistance);
		}

		public RESULT get3DAttributes(ref VECTOR position, ref float mindistance, ref float maxdistance)
		{
			return FMOD5_Reverb3D_Get3DAttributes(handle, ref position, ref mindistance, ref maxdistance);
		}

		public RESULT setProperties(ref REVERB_PROPERTIES properties)
		{
			return FMOD5_Reverb3D_SetProperties(handle, ref properties);
		}

		public RESULT getProperties(ref REVERB_PROPERTIES properties)
		{
			return FMOD5_Reverb3D_GetProperties(handle, ref properties);
		}

		public RESULT setActive(bool active)
		{
			return FMOD5_Reverb3D_SetActive(handle, active);
		}

		public RESULT getActive(out bool active)
		{
			return FMOD5_Reverb3D_GetActive(handle, out active);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD5_Reverb3D_SetUserData(handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD5_Reverb3D_GetUserData(handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_Release(IntPtr reverb);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_Set3DAttributes(IntPtr reverb, ref VECTOR position, float mindistance, float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_Get3DAttributes(IntPtr reverb, ref VECTOR position, ref float mindistance, ref float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_SetProperties(IntPtr reverb, ref REVERB_PROPERTIES properties);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_GetProperties(IntPtr reverb, ref REVERB_PROPERTIES properties);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_SetActive(IntPtr reverb, bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_GetActive(IntPtr reverb, out bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_SetUserData(IntPtr reverb, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Reverb3D_GetUserData(IntPtr reverb, out IntPtr userdata);

		public bool hasHandle()
		{
			return handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			handle = IntPtr.Zero;
		}
	}
}
