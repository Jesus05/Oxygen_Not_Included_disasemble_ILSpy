using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Filter;
using System;

namespace ProcGen.Noise
{
	public class Filter : NoiseBase
	{
		public NoiseFilter filter
		{
			get;
			set;
		}

		public float frequency
		{
			get;
			set;
		}

		public float lacunarity
		{
			get;
			set;
		}

		public int octaves
		{
			get;
			set;
		}

		public float offset
		{
			get;
			set;
		}

		public float gain
		{
			get;
			set;
		}

		public float exponent
		{
			get;
			set;
		}

		public Filter()
		{
			filter = NoiseFilter.RidgedMultiFractal;
			frequency = 10f;
			lacunarity = 3f;
			octaves = 10;
			offset = 1f;
			gain = 0f;
			exponent = 0f;
		}

		public Filter(Filter src)
		{
			filter = src.filter;
			frequency = src.frequency;
			lacunarity = src.lacunarity;
			octaves = src.octaves;
			offset = src.offset;
			gain = src.gain;
			exponent = src.exponent;
		}

		public override Type GetObjectType()
		{
			return typeof(Filter);
		}

		public IModule3D CreateModule()
		{
			FilterModule filterModule = null;
			switch (filter)
			{
			case NoiseFilter.Pipe:
				filterModule = new Pipe();
				break;
			case NoiseFilter.SumFractal:
				filterModule = new SumFractal();
				break;
			case NoiseFilter.SinFractal:
				filterModule = new SinFractal();
				break;
			case NoiseFilter.MultiFractal:
				filterModule = new MultiFractal();
				break;
			case NoiseFilter.Billow:
				filterModule = new Billow();
				break;
			case NoiseFilter.HeterogeneousMultiFractal:
				filterModule = new HeterogeneousMultiFractal();
				break;
			case NoiseFilter.HybridMultiFractal:
				filterModule = new HybridMultiFractal();
				break;
			case NoiseFilter.RidgedMultiFractal:
				filterModule = new RidgedMultiFractal();
				break;
			case NoiseFilter.Voronoi:
				filterModule = new Voronoi();
				break;
			}
			if (filterModule != null)
			{
				filterModule.Frequency = frequency;
				filterModule.Lacunarity = lacunarity;
				filterModule.OctaveCount = (float)octaves;
				filterModule.Offset = offset;
				filterModule.Gain = gain;
			}
			return (IModule3D)filterModule;
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule)
		{
			(target as FilterModule).Primitive3D = sourceModule;
		}
	}
}
