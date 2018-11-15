namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class Pipe : FilterModule, IModule4D, IModule3D, IModule2D, IModule1D, IModule
	{
		public float GetValue(float x, float y, float z, float t)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			t *= _frequency;
			return _source4D.GetValue(x, y, z, t);
		}

		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			return _source3D.GetValue(x, y, z);
		}

		public float GetValue(float x, float y)
		{
			x *= _frequency;
			y *= _frequency;
			return _source2D.GetValue(x, y);
		}

		public float GetValue(float x)
		{
			x *= _frequency;
			return _source1D.GetValue(x);
		}
	}
}
