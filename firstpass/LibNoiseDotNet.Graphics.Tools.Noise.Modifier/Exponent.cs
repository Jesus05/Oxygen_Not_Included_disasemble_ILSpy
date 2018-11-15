using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Exponent : ModifierModule, IModule3D, IModule
	{
		public const float DEFAULT_EXPONENT = 1f;

		protected float _exponent = 1f;

		public float ExponentValue
		{
			get
			{
				return _exponent;
			}
			set
			{
				_exponent = value;
			}
		}

		public Exponent()
		{
		}

		public Exponent(IModule source)
			: base(source)
		{
		}

		public Exponent(IModule source, float exponent)
			: base(source)
		{
			_exponent = exponent;
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)_sourceModule).GetValue(x, y, z);
			value = (value + 1f) / 2f;
			return (float)Math.Pow((double)Libnoise.FastFloor(value), (double)_exponent) * 2f - 1f;
		}
	}
}
