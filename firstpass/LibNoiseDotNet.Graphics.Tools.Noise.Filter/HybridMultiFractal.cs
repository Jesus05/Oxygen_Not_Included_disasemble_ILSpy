namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class HybridMultiFractal : FilterModule, IModule3D, IModule2D, IModule
	{
		public HybridMultiFractal()
		{
			_gain = 1f;
			_offset = 0.7f;
			_spectralExponent = 0.25f;
			ComputeSpectralWeights();
		}

		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			float num = _source3D.GetValue(x, y, z) + _offset;
			float num2 = _gain * num;
			x *= _lacunarity;
			y *= _lacunarity;
			z *= _lacunarity;
			int num3 = 1;
			while ((double)num2 > 0.001 && (float)num3 < _octaveCount)
			{
				if ((double)num2 > 1.0)
				{
					num2 = 1f;
				}
				float num4 = (_offset + _source3D.GetValue(x, y, z)) * _spectralWeights[num3];
				num4 *= num2;
				num += num4;
				num2 *= _gain * num4;
				x *= _lacunarity;
				y *= _lacunarity;
				z *= _lacunarity;
				num3++;
			}
			float num5 = _octaveCount - (float)(int)_octaveCount;
			if (num5 > 0f)
			{
				float num4 = _source3D.GetValue(x, y, z);
				num4 *= _spectralWeights[num3];
				num4 *= num5;
				num += num4;
			}
			return num;
		}

		public float GetValue(float x, float y)
		{
			x *= _frequency;
			y *= _frequency;
			float num = _source2D.GetValue(x, y) + _offset;
			float num2 = _gain * num;
			x *= _lacunarity;
			y *= _lacunarity;
			int num3 = 1;
			while ((double)num2 > 0.001 && (float)num3 < _octaveCount)
			{
				if ((double)num2 > 1.0)
				{
					num2 = 1f;
				}
				float num4 = (_offset + _source2D.GetValue(x, y)) * _spectralWeights[num3];
				num4 *= num2;
				num += num4;
				num2 *= _gain * num4;
				x *= _lacunarity;
				y *= _lacunarity;
				num3++;
			}
			float num5 = _octaveCount - (float)(int)_octaveCount;
			if (num5 > 0f)
			{
				float num4 = _source2D.GetValue(x, y);
				num4 *= _spectralWeights[num3];
				num4 *= num5;
				num += num4;
			}
			return num;
		}
	}
}
