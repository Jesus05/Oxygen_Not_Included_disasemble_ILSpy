namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class RidgedMultiFractal : FilterModule, IModule3D, IModule2D, IModule
	{
		public RidgedMultiFractal()
		{
			_gain = 2f;
			_offset = 1f;
			_spectralExponent = 0.9f;
			ComputeSpectralWeights();
		}

		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			float num = _source3D.GetValue(x, y, z);
			if ((double)num < 0.0)
			{
				num = 0f - num;
			}
			num = _offset - num;
			num *= num;
			float num2 = num;
			float num3 = 1f;
			int num4 = 1;
			while ((double)num3 > 0.001 && (float)num4 < _octaveCount)
			{
				x *= _lacunarity;
				y *= _lacunarity;
				z *= _lacunarity;
				num3 = Libnoise.Clamp01(num * _gain);
				num = _source3D.GetValue(x, y, z);
				if (num < 0f)
				{
					num = 0f - num;
				}
				num = _offset - num;
				num *= num;
				num *= num3;
				num2 += num * _spectralWeights[num4];
				num4++;
			}
			return num2;
		}

		public float GetValue(float x, float y)
		{
			x *= _frequency;
			y *= _frequency;
			float num = _source2D.GetValue(x, y);
			if (num < 0f)
			{
				num = 0f - num;
			}
			num = _offset - num;
			num *= num;
			float num2 = num;
			float num3 = 1f;
			int num4 = 1;
			while ((double)num3 > 0.001 && (float)num4 < _octaveCount)
			{
				x *= _lacunarity;
				y *= _lacunarity;
				num3 = Libnoise.Clamp01(num * _gain);
				num = _source2D.GetValue(x, y);
				if ((double)num < 0.0)
				{
					num = 0f - num;
				}
				num = _offset - num;
				num *= num;
				num *= num3;
				num2 += num * _spectralWeights[num4];
				num4++;
			}
			return num2;
		}
	}
}
