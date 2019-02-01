namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class ScaleBias : ModifierModule, IModule3D, IModule
	{
		public const float DEFAULT_SCALE = 1f;

		public const float DEFAULT_BIAS = 0f;

		protected float _scale = 1f;

		protected float _bias = 0f;

		public float Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
			}
		}

		public float Bias
		{
			get
			{
				return _bias;
			}
			set
			{
				_bias = value;
			}
		}

		public ScaleBias()
		{
		}

		public ScaleBias(IModule source)
			: base(source)
		{
		}

		public ScaleBias(IModule source, float scale, float bias)
			: base(source)
		{
			_scale = scale;
			_bias = bias;
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)_sourceModule).GetValue(x, y, z) * _scale + _bias;
		}
	}
}
