using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class Spheres : PrimitiveModule, IModule3D, IModule
	{
		public const float DEFAULT_FREQUENCY = 1f;

		protected float _frequency = 1f;

		public float Frequency
		{
			get
			{
				return _frequency;
			}
			set
			{
				_frequency = value;
			}
		}

		public Spheres()
			: this(1f)
		{
		}

		public Spheres(float frequency)
		{
			_frequency = frequency;
		}

		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			float num = (float)Math.Sqrt((double)(x * x + y * y + z * z));
			float num2 = num - (float)Math.Floor((double)num);
			float val = 1f - num2;
			float num3 = Math.Min(num2, val);
			return 1f - num3 * 4f;
		}
	}
}
