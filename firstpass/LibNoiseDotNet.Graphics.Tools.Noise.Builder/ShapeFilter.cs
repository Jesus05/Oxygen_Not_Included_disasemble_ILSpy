using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class ShapeFilter : IBuilderFilter
	{
		protected struct LevelCache
		{
			private int x;

			private int y;

			public byte level;

			public LevelCache(int x, int y, byte level)
			{
				this.x = x;
				this.y = y;
				this.level = level;
			}

			public bool IsCached(int x, int y)
			{
				return this.x == x && this.y == y;
			}

			public void Update(int x, int y, byte level)
			{
				this.x = x;
				this.y = y;
				this.level = level;
			}
		}

		public const float DEFAULT_VALUE = -0.5f;

		protected float _constant = -0.5f;

		protected IMap2D<IColor> _shape;

		protected LevelCache _cache = new LevelCache(-1, -1, 0);

		public IMap2D<IColor> Shape
		{
			get
			{
				return _shape;
			}
			set
			{
				_shape = value;
			}
		}

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

		public FilterLevel IsFiltered(int x, int y)
		{
			switch (GetGreyscaleLevel(x, y))
			{
			case 0:
				return FilterLevel.Constant;
			case byte.MaxValue:
				return FilterLevel.Source;
			default:
				return FilterLevel.Filter;
			}
		}

		public float FilterValue(int x, int y, float source)
		{
			byte greyscaleLevel = GetGreyscaleLevel(x, y);
			switch (greyscaleLevel)
			{
			case byte.MaxValue:
				return source;
			case 0:
				return _constant;
			default:
				return Libnoise.Lerp(_constant, source, (float)(int)greyscaleLevel / 255f);
			}
		}

		protected byte GetGreyscaleLevel(int x, int y)
		{
			if (!_cache.IsCached(x, y))
			{
				_cache.Update(x, y, _shape.GetValue(x, y).Red);
			}
			return _cache.level;
		}
	}
}
