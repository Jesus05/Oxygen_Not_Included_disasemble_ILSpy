namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class SumFractal : FilterModule, IModule3D, IModule2D, IModule
	{
		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			float num = 0f;
			int i;
			for (i = 0; (float)i < _octaveCount; i++)
			{
				float num2 = _source3D.GetValue(x, y, z) * _spectralWeights[i];
				num += num2;
				x *= _lacunarity;
				y *= _lacunarity;
				z *= _lacunarity;
			}
			float num3 = _octaveCount - (float)(int)_octaveCount;
			if (num3 > 0f)
			{
				num += num3 * _source3D.GetValue(x, y, z) * _spectralWeights[i];
			}
			return num;
		}

		public float GetValue(float x, float y)
		{
			x *= _frequency;
			y *= _frequency;
			float num = 0f;
			int i;
			for (i = 0; (float)i < _octaveCount; i++)
			{
				float num2 = _source2D.GetValue(x, y) * _spectralWeights[i];
				num += num2;
				x *= _lacunarity;
				y *= _lacunarity;
			}
			float num3 = _octaveCount - (float)(int)_octaveCount;
			if (num3 > 0f)
			{
				num += num3 * _source2D.GetValue(x, y) * _spectralWeights[i];
			}
			return num;
		}
	}
}
