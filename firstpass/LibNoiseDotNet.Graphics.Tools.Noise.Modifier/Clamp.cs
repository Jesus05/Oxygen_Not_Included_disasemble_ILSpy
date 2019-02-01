namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Clamp : ModifierModule, IModule3D, IModule
	{
		public const float DEFAULT_LOWER_BOUND = -1f;

		public const float DEFAULT_UPPER_BOUND = 1f;

		protected float _lowerBound = -1f;

		protected float _upperBound = 1f;

		public float LowerBound
		{
			get
			{
				return _lowerBound;
			}
			set
			{
				_lowerBound = value;
			}
		}

		public float UpperBound
		{
			get
			{
				return _upperBound;
			}
			set
			{
				_upperBound = value;
			}
		}

		public Clamp()
		{
		}

		public Clamp(IModule source)
			: base(source)
		{
		}

		public Clamp(IModule source, float lower, float upper)
			: base(source)
		{
			_lowerBound = lower;
			_upperBound = upper;
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)_sourceModule).GetValue(x, y, z);
			if (!(value < _lowerBound))
			{
				if (!(value > _upperBound))
				{
					return value;
				}
				return _upperBound;
			}
			return _lowerBound;
		}
	}
}
