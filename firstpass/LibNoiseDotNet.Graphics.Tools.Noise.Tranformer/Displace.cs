namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class Displace : TransformerModule, IModule3D, IModule
	{
		protected IModule _sourceModule;

		protected IModule _xDisplaceModule;

		protected IModule _yDisplaceModule;

		protected IModule _zDisplaceModule;

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

		public IModule XDisplaceModule
		{
			get
			{
				return _xDisplaceModule;
			}
			set
			{
				_xDisplaceModule = value;
			}
		}

		public IModule YDisplaceModule
		{
			get
			{
				return _yDisplaceModule;
			}
			set
			{
				_yDisplaceModule = value;
			}
		}

		public IModule ZDisplaceModule
		{
			get
			{
				return _zDisplaceModule;
			}
			set
			{
				_zDisplaceModule = value;
			}
		}

		public Displace()
		{
		}

		public Displace(IModule source, IModule xDisplaceModule, IModule yDisplaceModule, IModule zDisplaceModule)
		{
			_sourceModule = source;
			_xDisplaceModule = xDisplaceModule;
			_yDisplaceModule = yDisplaceModule;
			_zDisplaceModule = zDisplaceModule;
		}

		public float GetValue(float x, float y, float z)
		{
			float x2 = x + ((IModule3D)_xDisplaceModule).GetValue(x, y, z);
			float y2 = y + ((IModule3D)_yDisplaceModule).GetValue(x, y, z);
			float z2 = z + ((IModule3D)_zDisplaceModule).GetValue(x, y, z);
			return ((IModule3D)_sourceModule).GetValue(x2, y2, z2);
		}
	}
}
