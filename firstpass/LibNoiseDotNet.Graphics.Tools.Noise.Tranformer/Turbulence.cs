namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class Turbulence : TransformerModule, IModule3D, IModule
	{
		public const float DEFAULT_POWER = 1f;

		protected float _power = 1f;

		protected IModule _sourceModule;

		protected IModule _xDistortModule;

		protected IModule _yDistortModule;

		protected IModule _zDistortModule;

		public IModule SourceModule
		{
			get
			{
				return _sourceModule;
			}
			set
			{
				_sourceModule = value;
			}
		}

		public IModule XDistortModule
		{
			get
			{
				return _xDistortModule;
			}
			set
			{
				_xDistortModule = value;
			}
		}

		public IModule YDistortModule
		{
			get
			{
				return _yDistortModule;
			}
			set
			{
				_yDistortModule = value;
			}
		}

		public IModule ZDistortModule
		{
			get
			{
				return _zDistortModule;
			}
			set
			{
				_zDistortModule = value;
			}
		}

		public float Power
		{
			get
			{
				return _power;
			}
			set
			{
				_power = value;
			}
		}

		public Turbulence()
		{
			_power = 1f;
		}

		public Turbulence(IModule source)
			: this()
		{
			_sourceModule = source;
		}

		public Turbulence(IModule source, IModule xDistortModule, IModule yDistortModule, IModule zDistortModule, float power)
		{
			_sourceModule = source;
			_xDistortModule = xDistortModule;
			_yDistortModule = yDistortModule;
			_zDistortModule = zDistortModule;
			_power = power;
		}

		public float GetValue(float x, float y, float z)
		{
			float x2 = x + 0.1894226f;
			float y2 = y + 0.9937134f;
			float z2 = z + 0.478164673f;
			float x3 = x + 0.404647827f;
			float y3 = y + 0.276611328f;
			float z3 = z + 0.9230499f;
			float x4 = x + 0.821228f;
			float y4 = y + 0.1710968f;
			float z4 = z + 0.6842804f;
			float x5 = x + ((IModule3D)_xDistortModule).GetValue(x2, y2, z2) * _power;
			float y5 = y + ((IModule3D)_yDistortModule).GetValue(x3, y3, z3) * _power;
			float z5 = z + ((IModule3D)_zDistortModule).GetValue(x4, y4, z4) * _power;
			return ((IModule3D)_sourceModule).GetValue(x5, y5, z5);
		}
	}
}
