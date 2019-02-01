namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class Billow : FilterModule, IModule3D, IModule2D, IModule
	{
		public const float DEFAULT_SCALE = 1f;

		public const float DEFAULT_BIAS = 0f;

		protected float _scale = 1f;

		protected float _bias = 0f;

		public float Scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
			}
		}

		public float Bias
		{
			get
			{
				return _bias;
			}
			set
			{
				_bias = value;
			}
		}

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
				if (num2 < 0f)
				{
					num2 = 0f - num2;
				}
				num += num2 * _scale + _bias;
				x *= _lacunarity;
				y *= _lacunarity;
				z *= _lacunarity;
			}
			float num3 = _octaveCount - (float)(int)_octaveCount;
			if (num3 > 0f)
			{
				num += _scale * num3 * _source3D.GetValue(x, y, z) * _spectralWeights[i] + _bias;
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
				if (num2 < 0f)
				{
					num2 = 0f - num2;
				}
				num += num2 * _scale + _bias;
				x *= _lacunarity;
				y *= _lacunarity;
			}
			float num3 = _octaveCount - (float)(int)_octaveCount;
			if (num3 > 0f)
			{
				num += _scale * num3 * _source2D.GetValue(x, y) * _spectralWeights[i] + _bias;
			}
			return num;
		}
	}
}
