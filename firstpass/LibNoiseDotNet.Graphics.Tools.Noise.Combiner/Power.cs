using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Combiner
{
	public class Power : CombinerModule, IModule3D, IModule
	{
		public Power()
		{
		}

		public Power(IModule left, IModule right)
			: base(left, right)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			return (float)Math.Pow((double)((IModule3D)_leftModule).GetValue(x, y, z), (double)((IModule3D)_rightModule).GetValue(x, y, z));
		}
	}
}
