namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class PrimitiveModule : IModule
	{
		public const int DEFAULT_SEED = 0;

		public const NoiseQuality DEFAULT_QUALITY = NoiseQuality.Standard;

		protected int _seed = 0;

		protected NoiseQuality _quality = NoiseQuality.Standard;

		public virtual int Seed
		{
			get
			{
				return _seed;
			}
			set
			{
				_seed = value;
			}
		}

		public virtual NoiseQuality Quality
		{
			get
			{
				return _quality;
			}
			set
			{
				_quality = value;
			}
		}

		public PrimitiveModule()
			: this(0, NoiseQuality.Standard)
		{
		}

		public PrimitiveModule(int seed)
			: this(seed, NoiseQuality.Standard)
		{
		}

		public PrimitiveModule(int seed, NoiseQuality quality)
		{
			_seed = seed;
			_quality = quality;
		}
	}
}
