using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class SinFractal : FilterModule, IModule3D, IModule2D, IModule
	{
		public float GetValue(float x, float y, float z)
		{
			float num = x;
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			float num2 = 0f;
			int i;
			for (i = 0; (float)i < _octaveCount; i++)
			{
				float num3 = _source3D.GetValue(x, y, z) * _spectralWeights[i];
				if ((double)num3 < 0.0)
				{
					num3 = 0f - num3;
				}
				num2 += num3;
				x *= _lacunarity;
				y *= _lacunarity;
				z *= _lacunarity;
			}
			float num4 = _octaveCount - (float)(int)_octaveCount;
			if (num4 > 0f)
			{
				num2 += num4 * _source3D.GetValue(x, y, z) * _spectralWeights[i];
			}
			return (float)Math.Sin((double)(num + num2));
		}

		public float GetValue(float x, float y)
		{
			float num = x;
			x *= _frequency;
			y *= _frequency;
			float num2 = 0f;
			int i;
			for (i = 0; (float)i < _octaveCount; i++)
			{
				float num3 = _source2D.GetValue(x, y) * _spectralWeights[i];
				if ((double)num3 < 0.0)
				{
					num3 = 0f - num3;
				}
				num2 += num3;
				x *= _lacunarity;
				y *= _lacunarity;
			}
			float num4 = _octaveCount - (float)(int)_octaveCount;
			if (num4 > 0f)
			{
				num2 += num4 * _source2D.GetValue(x, y) * _spectralWeights[i];
			}
			return (float)Math.Sin((double)(num + num2));
		}
	}
}
