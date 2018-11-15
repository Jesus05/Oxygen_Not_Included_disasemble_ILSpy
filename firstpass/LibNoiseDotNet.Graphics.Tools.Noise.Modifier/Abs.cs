using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Abs : ModifierModule, IModule3D, IModule
	{
		public Abs()
		{
		}

		public Abs(IModule source)
			: base(source)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			return Math.Abs(((IModule3D)_sourceModule).GetValue(x, y, z));
		}
	}
}
