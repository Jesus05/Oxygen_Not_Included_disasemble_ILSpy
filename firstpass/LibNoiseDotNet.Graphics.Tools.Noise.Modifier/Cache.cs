namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Cache : ModifierModule, IModule3D, IModule
	{
		protected float _cachedValue;

		protected bool _isCached;

		protected float _xCache;

		protected float _yCache;

		protected float _zCache;

		public new IModule SourceModule
		{
			get
			{
				return _sourceModule;
			}
			set
			{
				_isCached = false;
				_sourceModule = value;
			}
		}

		public Cache()
		{
		}

		public Cache(IModule source)
			: base(source)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			if (!_isCached || x != _xCache || y != _yCache || z != _zCache)
			{
				_cachedValue = ((IModule3D)_sourceModule).GetValue(x, y, z);
				_xCache = x;
				_yCache = y;
				_zCache = z;
				_isCached = true;
			}
			return _cachedValue;
		}
	}
}
