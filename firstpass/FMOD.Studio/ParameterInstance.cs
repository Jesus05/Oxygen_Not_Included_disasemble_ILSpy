using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct ParameterInstance
	{
		public IntPtr handle;

		public RESULT getDescription(out PARAMETER_DESCRIPTION description)
		{
			return FMOD_Studio_ParameterInstance_GetDescription(handle, out description);
		}

		public RESULT getValue(out float value)
		{
			return FMOD_Studio_ParameterInstance_GetValue(handle, out value);
		}

		public RESULT setValue(float value)
		{
			return FMOD_Studio_ParameterInstance_SetValue(handle, value);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_ParameterInstance_IsValid(IntPtr parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetDescription(IntPtr parameter, out PARAMETER_DESCRIPTION description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_GetValue(IntPtr parameter, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParameterInstance_SetValue(IntPtr parameter, float value);

		public bool hasHandle()
		{
			return handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			handle = IntPtr.Zero;
		}

		public bool isValid()
		{
			return hasHandle() && FMOD_Studio_ParameterInstance_IsValid(handle);
		}
	}
}
