namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class ScalePoint : TransformerModule, IModule3D, IModule
	{
		public const float DEFAULT_POINT_X = 1f;

		public const float DEFAULT_POINT_Y = 1f;

		public const float DEFAULT_POINT_Z = 1f;

		protected IModule _sourceModule;

		protected float _xScale = 1f;

		protected float _yScale = 1f;

		protected float _zScale = 1f;

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

		public float XScale
		{
			get
			{
				return _xScale;
			}
			set
			{
				_xScale = value;
			}
		}

		public float YScale
		{
			get
			{
				return _yScale;
			}
			set
			{
				_yScale = value;
			}
		}

		public float ZScale
		{
			get
			{
				return _zScale;
			}
			set
			{
				_zScale = value;
			}
		}

		public ScalePoint()
		{
		}

		public ScalePoint(IModule source)
		{
			_sourceModule = source;
		}

		public ScalePoint(IModule source, float x, float y, float z)
			: this(source)
		{
			_xScale = x;
			_yScale = y;
			_zScale = z;
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)_sourceModule).GetValue(x * _xScale, y * _yScale, z * _zScale);
		}
	}
}
