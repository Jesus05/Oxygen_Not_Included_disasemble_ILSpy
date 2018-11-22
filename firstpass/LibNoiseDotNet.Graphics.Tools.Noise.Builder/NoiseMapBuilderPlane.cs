using LibNoiseDotNet.Graphics.Tools.Noise.Model;
using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMapBuilderPlane : NoiseMapBuilder
	{
		private bool _seamless = false;

		private float _lowerXBound;

		private float _lowerZBound;

		private float _upperXBound;

		private float _upperZBound;

		public bool Seamless
		{
			get
			{
				return _seamless;
			}
			set
			{
				_seamless = value;
			}
		}

		public float LowerXBound => _lowerXBound;

		public float LowerZBound => _lowerZBound;

		public float UpperXBound => _upperXBound;

		public float UpperZBound => _upperZBound;

		public NoiseMapBuilderPlane()
		{
			_seamless = false;
			_lowerXBound = (_lowerZBound = (_upperXBound = (_upperZBound = 0f)));
		}

		public NoiseMapBuilderPlane(float lowerXBound, float upperXBound, float lowerZBound, float upperZBound, bool seamless)
		{
			_seamless = seamless;
			SetBounds(lowerXBound, upperXBound, lowerZBound, upperZBound);
		}

		public void SetBounds(float lowerXBound, float upperXBound, float lowerZBound, float upperZBound)
		{
			if (lowerXBound >= upperXBound || lowerZBound >= upperZBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerXBound >= upperXBound or lowerZBound >= upperZBound");
			}
			_lowerXBound = lowerXBound;
			_upperXBound = upperXBound;
			_lowerZBound = lowerZBound;
			_upperZBound = upperZBound;
		}

		public override void Build()
		{
			if (_lowerXBound >= _upperXBound || _lowerZBound >= _upperZBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerXBound >= upperXBound or lowerZBound >= upperZBound");
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
			Plane plane = new Plane(_sourceModule);
			float num = _upperXBound - _lowerXBound;
			float num2 = _upperZBound - _lowerZBound;
			float num3 = num / (float)_width;
			float num4 = num2 / (float)_height;
			float lowerXBound = _lowerXBound;
			float num5 = _lowerZBound;
			for (int i = 0; i < _height; i++)
			{
				lowerXBound = _lowerXBound;
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
						if (_seamless)
						{
							float value = plane.GetValue(lowerXBound, num5);
							float value2 = plane.GetValue(lowerXBound + num, num5);
							float value3 = plane.GetValue(lowerXBound, num5 + num2);
							float value4 = plane.GetValue(lowerXBound + num, num5 + num2);
							float a = 1f - (lowerXBound - _lowerXBound) / num;
							float a2 = 1f - (num5 - _lowerZBound) / num2;
							float n = Libnoise.Lerp(value, value2, a);
							float n2 = Libnoise.Lerp(value3, value4, a);
							num6 = Libnoise.Lerp(n, n2, a2);
						}
						else
						{
							num6 = plane.GetValue(lowerXBound, num5);
						}
						if (filterLevel == FilterLevel.Filter)
						{
							num6 = _filter.FilterValue(j, i, num6);
						}
					}
					_noiseMap.SetValue(j, i, num6);
					lowerXBound += num3;
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
