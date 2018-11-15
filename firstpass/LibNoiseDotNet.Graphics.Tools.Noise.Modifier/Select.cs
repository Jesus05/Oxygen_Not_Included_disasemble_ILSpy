namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Select : SelectorModule, IModule3D, IModule
	{
		public const float DEFAULT_FALL_OFF = -1f;

		public const float DEFAULT_LOWER_BOUND = -1f;

		public const float DEFAULT_UPPER_BOUND = 1f;

		protected float _lowerBound = -1f;

		protected float _upperBound = 1f;

		protected float _edgeFalloff = -1f;

		protected IModule _controlModule;

		protected IModule _rightModule;

		protected IModule _leftModule;

		public float LowerBound => _lowerBound;

		public float UpperBound => _upperBound;

		public float EdgeFalloff
		{
			get
			{
				return _edgeFalloff;
			}
			set
			{
				float num = _upperBound - _lowerBound;
				_edgeFalloff = ((!(value > num / 2f)) ? value : (num / 2f));
			}
		}

		public IModule LeftModule
		{
			get
			{
				return _leftModule;
			}
			set
			{
				_leftModule = value;
			}
		}

		public IModule RightModule
		{
			get
			{
				return _rightModule;
			}
			set
			{
				_rightModule = value;
			}
		}

		public IModule ControlModule
		{
			get
			{
				return _controlModule;
			}
			set
			{
				_controlModule = value;
			}
		}

		public Select()
		{
		}

		public Select(IModule controlModule, IModule rightModule, IModule leftModule, float lower, float upper, float edge)
		{
			_controlModule = controlModule;
			_leftModule = leftModule;
			_rightModule = rightModule;
			SetBounds(lower, upper);
			EdgeFalloff = edge;
		}

		public void SetBounds(float lower, float upper)
		{
			_lowerBound = lower;
			_upperBound = upper;
			EdgeFalloff = _edgeFalloff;
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)_controlModule).GetValue(x, y, z);
			if ((double)_edgeFalloff > 0.0)
			{
				if (value < _lowerBound - _edgeFalloff)
				{
					return ((IModule3D)_leftModule).GetValue(x, y, z);
				}
				if (value < _lowerBound + _edgeFalloff)
				{
					float num = _lowerBound - _edgeFalloff;
					float num2 = _lowerBound + _edgeFalloff;
					float a = Libnoise.SCurve3((value - num) / (num2 - num));
					return Libnoise.Lerp(((IModule3D)_leftModule).GetValue(x, y, z), ((IModule3D)_leftModule).GetValue(x, y, z), a);
				}
				if (value < _upperBound - _edgeFalloff)
				{
					return ((IModule3D)_leftModule).GetValue(x, y, z);
				}
				if (value < _upperBound + _edgeFalloff)
				{
					float num3 = _upperBound - _edgeFalloff;
					float num4 = _upperBound + _edgeFalloff;
					float a = Libnoise.SCurve3((value - num3) / (num4 - num3));
					return Libnoise.Lerp(((IModule3D)_leftModule).GetValue(x, y, z), ((IModule3D)_leftModule).GetValue(x, y, z), a);
				}
				return ((IModule3D)_leftModule).GetValue(x, y, z);
			}
			if (value < _lowerBound || value > _upperBound)
			{
				return ((IModule3D)_leftModule).GetValue(x, y, z);
			}
			return ((IModule3D)_leftModule).GetValue(x, y, z);
		}
	}
}
