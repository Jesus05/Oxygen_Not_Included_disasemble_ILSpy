using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Combiner
{
	public class Min : CombinerModule, IModule3D, IModule
	{
		public Min()
		{
		}

		public Min(IModule left, IModule right)
			: base(left, right)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			return Math.Min(((IModule3D)_leftModule).GetValue(x, y, z), ((IModule3D)_rightModule).GetValue(x, y, z));
		}
	}
}
