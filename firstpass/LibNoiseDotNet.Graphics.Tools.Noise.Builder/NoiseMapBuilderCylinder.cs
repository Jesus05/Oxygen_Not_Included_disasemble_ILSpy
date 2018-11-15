using LibNoiseDotNet.Graphics.Tools.Noise.Model;
using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class NoiseMapBuilderCylinder : NoiseMapBuilder
	{
		private float _lowerAngleBound;

		private float _lowerHeightBound;

		private float _upperAngleBound;

		private float _upperHeightBound;

		public float LowerHeightBound => _lowerHeightBound;

		public float LowerAngleBound => _lowerAngleBound;

		public float UpperAngleBound => _upperAngleBound;

		public float UpperHeightBound => _upperHeightBound;

		public NoiseMapBuilderCylinder()
		{
			SetBounds(-180f, 180f, -10f, 10f);
		}

		public void SetBounds(float lowerAngleBound, float upperAngleBound, float lowerHeightBound, float upperHeightBound)
		{
			if (lowerAngleBound >= upperAngleBound || lowerHeightBound >= upperHeightBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerAngleBound >= upperAngleBound or lowerZBound >= upperHeightBound");
			}
			_lowerAngleBound = lowerAngleBound;
			_upperAngleBound = upperAngleBound;
			_lowerHeightBound = lowerHeightBound;
			_upperHeightBound = upperHeightBound;
		}

		public override void Build()
		{
			if (_lowerAngleBound >= _upperAngleBound || _lowerHeightBound >= _upperHeightBound)
			{
				throw new ArgumentException("Incoherent bounds : lowerAngleBound >= upperAngleBound or lowerZBound >= upperHeightBound");
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
			Cylinder cylinder = new Cylinder((IModule3D)_sourceModule);
			float num = _upperAngleBound - _lowerAngleBound;
			float num2 = _upperHeightBound - _lowerHeightBound;
			float num3 = num / (float)_width;
			float num4 = num2 / (float)_height;
			float lowerAngleBound = _lowerAngleBound;
			float num5 = _lowerHeightBound;
			for (int i = 0; i < _height; i++)
			{
				lowerAngleBound = _lowerAngleBound;
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
						num6 = cylinder.GetValue(lowerAngleBound, num5);
						if (filterLevel == FilterLevel.Filter)
						{
							num6 = _filter.FilterValue(j, i, num6);
						}
					}
					_noiseMap.SetValue(j, i, num6);
					lowerAngleBound += num3;
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
