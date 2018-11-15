using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public abstract class FilterModule : IModule
	{
		public const float DEFAULT_FREQUENCY = 1f;

		public const float DEFAULT_LACUNARITY = 2f;

		public const float DEFAULT_OCTAVE_COUNT = 6f;

		public const int MAX_OCTAVE = 30;

		public const float DEFAULT_OFFSET = 1f;

		public const float DEFAULT_GAIN = 2f;

		public const float DEFAULT_SPECTRAL_EXPONENT = 0.9f;

		protected float _frequency = 1f;

		protected float _lacunarity = 2f;

		protected float _octaveCount = 6f;

		protected float[] _spectralWeights = new float[30];

		protected float _offset = 1f;

		protected float _gain = 2f;

		protected float _spectralExponent = 0.9f;

		protected IModule4D _source4D;

		protected IModule3D _source3D;

		protected IModule2D _source2D;

		protected IModule1D _source1D;

		public float Frequency
		{
			get
			{
				return _frequency;
			}
			set
			{
				_frequency = value;
			}
		}

		public float Lacunarity
		{
			get
			{
				return _lacunarity;
			}
			set
			{
				_lacunarity = value;
				ComputeSpectralWeights();
			}
		}

		public float OctaveCount
		{
			get
			{
				return _octaveCount;
			}
			set
			{
				_octaveCount = Libnoise.Clamp(value, 1f, 30f);
			}
		}

		public float Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
			}
		}

		public float Gain
		{
			get
			{
				return _gain;
			}
			set
			{
				_gain = value;
			}
		}

		public float SpectralExponent
		{
			get
			{
				return _spectralExponent;
			}
			set
			{
				_spectralExponent = value;
				ComputeSpectralWeights();
			}
		}

		public IModule4D Primitive4D
		{
			get
			{
				return _source4D;
			}
			set
			{
				_source4D = value;
			}
		}

		public IModule3D Primitive3D
		{
			get
			{
				return _source3D;
			}
			set
			{
				_source3D = value;
			}
		}

		public IModule2D Primitive2D
		{
			get
			{
				return _source2D;
			}
			set
			{
				_source2D = value;
			}
		}

		public IModule1D Primitive1D
		{
			get
			{
				return _source1D;
			}
			set
			{
				_source1D = value;
			}
		}

		protected FilterModule()
			: this(1f, 2f, 0.9f, 6f)
		{
		}

		protected FilterModule(float frequency, float lacunarity, float exponent, float octaveCount)
		{
			_frequency = frequency;
			_lacunarity = lacunarity;
			_spectralExponent = exponent;
			_octaveCount = octaveCount;
			ComputeSpectralWeights();
		}

		protected void ComputeSpectralWeights()
		{
			for (int i = 0; i < 30; i++)
			{
				_spectralWeights[i] = (float)Math.Pow((double)_lacunarity, (double)((float)(-i) * _spectralExponent));
			}
		}
	}
}
