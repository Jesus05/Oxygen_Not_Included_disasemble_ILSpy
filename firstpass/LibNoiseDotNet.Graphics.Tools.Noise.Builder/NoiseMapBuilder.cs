using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public abstract class NoiseMapBuilder
	{
		protected IModule _sourceModule;

		protected IMap2D<float> _noiseMap;

		protected NoiseMapBuilderCallback _callBack;

		protected int _width = 0;

		protected int _height = 0;

		protected IBuilderFilter _filter;

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

		public IMap2D<float> NoiseMap
		{
			get
			{
				return _noiseMap;
			}
			set
			{
				_noiseMap = value;
			}
		}

		public NoiseMapBuilderCallback CallBack
		{
			get
			{
				return _callBack;
			}
			set
			{
				_callBack = value;
			}
		}

		public int Width => _width;

		public int Height => _height;

		public IBuilderFilter Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				_filter = value;
			}
		}

		public NoiseMapBuilder()
		{
		}

		public abstract void Build();

		public void SetSize(int width, int height)
		{
			if (width < 0 || height < 0)
			{
				throw new ArgumentException("Dimension must be greater or equal 0");
			}
			_height = height;
			_width = width;
		}
	}
}
