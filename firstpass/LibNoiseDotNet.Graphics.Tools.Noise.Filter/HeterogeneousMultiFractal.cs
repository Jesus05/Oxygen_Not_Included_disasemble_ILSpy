namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class HeterogeneousMultiFractal : FilterModule, IModule3D, IModule2D, IModule
	{
		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			float num = _offset + _source3D.GetValue(x, y, z);
			x *= _lacunarity;
			y *= _lacunarity;
			z *= _lacunarity;
			int i;
			for (i = 1; (float)i < _octaveCount; i++)
			{
				float num2 = _offset + _source3D.GetValue(x, y, z);
				num2 *= _spectralWeights[i];
				num2 *= num;
				num += num2;
				x *= _lacunarity;
				y *= _lacunarity;
				z *= _lacunarity;
			}
			float num3 = _octaveCount - (float)(int)_octaveCount;
			if (num3 > 0f)
			{
				float num2 = _offset + _source3D.GetValue(x, y, z);
				num2 *= _spectralWeights[i];
				num2 *= num;
				num2 *= num3;
				num += num2;
			}
			return num;
		}

		public float GetValue(float x, float y)
		{
			x *= _frequency;
			y *= _frequency;
			float num = _offset + _source2D.GetValue(x, y);
			x *= _lacunarity;
			y *= _lacunarity;
			int i;
			for (i = 1; (float)i < _octaveCount; i++)
			{
				float num2 = _offset + _source2D.GetValue(x, y);
				num2 *= _spectralWeights[i];
				num2 *= num;
				num += num2;
				x *= _lacunarity;
				y *= _lacunarity;
			}
			float num3 = _octaveCount - (float)(int)_octaveCount;
			if (num3 > 0f)
			{
				float num2 = _offset + _source2D.GetValue(x, y);
				num2 *= _spectralWeights[i];
				num2 *= num;
				num2 *= num3;
				num += num2;
			}
			return num;
		}
	}
}
