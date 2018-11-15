namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class Constant : PrimitiveModule, IModule4D, IModule3D, IModule2D, IModule1D, IModule
	{
		public const float DEFAULT_VALUE = 0.5f;

		protected float _constant = 0.5f;

		public float ConstantValue
		{
			get
			{
				return _constant;
			}
			set
			{
				_constant = value;
			}
		}

		public Constant()
			: this(0.5f)
		{
		}

		public Constant(float value)
		{
			_constant = value;
		}

		public float GetValue(float x, float y, float z, float t)
		{
			return _constant;
		}

		public float GetValue(float x, float y, float z)
		{
			return _constant;
		}

		public float GetValue(float x, float y)
		{
			return _constant;
		}

		public float GetValue(float x)
		{
			return _constant;
		}
	}
}
