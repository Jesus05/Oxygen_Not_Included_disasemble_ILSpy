using LibNoiseDotNet.Graphics.Tools.Noise.Model;
using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMapBuilderSphere : NoiseMapBuilder
	{
		private float _eastLonBound;

		private float _northLatBound;

		private float _southLatBound;

		private float _westLonBound;

		public float EastLonBound => _eastLonBound;

		public float NorthLatBound => _northLatBound;

		public float SouthLatBound => _southLatBound;

		public float WestLonBound => _westLonBound;

		public NoiseMapBuilderSphere()
		{
			SetBounds(-90f, 90f, -180f, 180f);
		}

		public void SetBounds(float southLatBound, float northLatBound, float westLonBound, float eastLonBound)
		{
			if (southLatBound >= northLatBound || westLonBound >= eastLonBound)
			{
				throw new ArgumentException("Incoherent bounds : southLatBound >= northLatBound or westLonBound >= eastLonBound");
			}
			_southLatBound = southLatBound;
			_northLatBound = northLatBound;
			_westLonBound = westLonBound;
			_eastLonBound = eastLonBound;
		}

		public override void Build()
		{
			if (_southLatBound >= _northLatBound || _westLonBound >= _eastLonBound)
			{
				throw new ArgumentException("Incoherent bounds : southLatBound >= northLatBound or westLonBound >= eastLonBound");
			}
			if (_width < 0 || _height < 0)
			{
				throw new ArgumentException("Dimension must be greater or equal 0");
			}
			if (_sourceModule == null)
			{
				throw new ArgumentException("A source module must be provided");
			}
			if (_noiseMap == null)
			{
				throw new ArgumentException("A noise map must be provided");
			}
			_noiseMap.SetSize(_width, _height);
			Sphere sphere = new Sphere((IModule3D)_sourceModule);
			float num = _eastLonBound - _westLonBound;
			float num2 = _northLatBound - _southLatBound;
			float num3 = num / (float)_width;
			float num4 = num2 / (float)_height;
			float westLonBound = _westLonBound;
			float num5 = _southLatBound;
			for (int i = 0; i < _height; i++)
			{
				westLonBound = _westLonBound;
				for (int j = 0; j < _width; j++)
				{
					FilterLevel filterLevel = FilterLevel.Source;
					if (_filter != null)
					{
						filterLevel = _filter.IsFiltered(j, i);
					}
					float num6;
					if (filterLevel == FilterLevel.Constant)
					{
						num6 = _filter.ConstantValue;
					}
					else
					{
						num6 = sphere.GetValue(num5, westLonBound);
						if (filterLevel == FilterLevel.Filter)
						{
							num6 = _filter.FilterValue(j, i, num6);
						}
					}
					_noiseMap.SetValue(j, i, num6);
					westLonBound += num3;
				}
				num5 += num4;
				if (_callBack != null)
				{
					_callBack(i);
				}
			}
		}
	}
}
