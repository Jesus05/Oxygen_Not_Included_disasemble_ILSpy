using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class Cylinder : AbstractModel
	{
		public Cylinder()
		{
		}

		public Cylinder(IModule3D module)
			: base(module)
		{
		}

		public float GetValue(float angle, float height)
		{
			float x = (float)Math.Cos((double)(angle * 0.0174532924f));
			float z = (float)Math.Sin((double)(angle * 0.0174532924f));
			return ((IModule3D)_sourceModule).GetValue(x, height, z);
		}
	}
}
