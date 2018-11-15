using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSP_PARAMETER_3DATTRIBUTES_MULTI
	{
		public int numlisteners;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public ATTRIBUTES_3D[] relative;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public float[] weight;

		public ATTRIBUTES_3D absolute;
	}
}
