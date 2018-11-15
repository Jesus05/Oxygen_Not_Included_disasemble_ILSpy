namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class TranslatePoint : TransformerModule, IModule3D, IModule
	{
		public const float DEFAULT_TRANSLATE_X = 1f;

		public const float DEFAULT_TRANSLATE_Y = 1f;

		public const float DEFAULT_TRANSLATE_Z = 1f;

		protected IModule _sourceModule;

		protected float _xTranslate = 1f;

		protected float _yTranslate = 1f;

		protected float _zTranslate = 1f;

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

		public float XTranslate
		{
			get
			{
				return _xTranslate;
			}
			set
			{
				_xTranslate = value;
			}
		}

		public float YTranslate
		{
			get
			{
				return _yTranslate;
			}
			set
			{
				_yTranslate = value;
			}
		}

		public float ZTranslate
		{
			get
			{
				return _zTranslate;
			}
			set
			{
				_zTranslate = value;
			}
		}

		public TranslatePoint()
		{
		}

		public TranslatePoint(IModule source)
		{
			_sourceModule = source;
		}

		public TranslatePoint(IModule source, float x, float y, float z)
			: this(source)
		{
			_xTranslate = x;
			_yTranslate = y;
			_zTranslate = z;
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)_sourceModule).GetValue(x + _xTranslate, y + _yTranslate, z + _zTranslate);
		}
	}
}
