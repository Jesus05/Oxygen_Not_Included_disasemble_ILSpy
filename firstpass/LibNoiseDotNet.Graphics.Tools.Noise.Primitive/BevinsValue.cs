namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class BevinsValue : PrimitiveModule, IModule3D, IModule2D, IModule1D, IModule
	{
		public const int X_NOISE_GEN = 1619;

		public const int Y_NOISE_GEN = 31337;

		public const int Z_NOISE_GEN = 6971;

		public const int SEED_NOISE_GEN = 1013;

		public const int SHIFT_NOISE_GEN = 8;

		public BevinsValue()
			: this(0, NoiseQuality.Standard)
		{
		}

		public BevinsValue(int seed, NoiseQuality quality)
		{
			_seed = seed;
			_quality = quality;
		}

		public float GetValue(float x, float y, float z)
		{
			return ValueCoherentNoise3D(x, y, z, _seed, _quality);
		}

		public static float ValueCoherentNoise3D(float x, float y, float z, long seed, NoiseQuality quality)
		{
			int num = (!((double)x > 0.0)) ? ((int)x - 1) : ((int)x);
			int x2 = num + 1;
			int num2 = (!((double)y > 0.0)) ? ((int)y - 1) : ((int)y);
			int y2 = num2 + 1;
			int num3 = (!((double)z > 0.0)) ? ((int)z - 1) : ((int)z);
			int z2 = num3 + 1;
			float a = 0f;
			float a2 = 0f;
			float a3 = 0f;
			switch (quality)
			{
			case NoiseQuality.Fast:
				a = x - (float)num;
				a2 = y - (float)num2;
				a3 = z - (float)num3;
				break;
			case NoiseQuality.Standard:
				a = Libnoise.SCurve3(x - (float)num);
				a2 = Libnoise.SCurve3(y - (float)num2);
				a3 = Libnoise.SCurve3(z - (float)num3);
				break;
			case NoiseQuality.Best:
				a = Libnoise.SCurve5(x - (float)num);
				a2 = Libnoise.SCurve5(y - (float)num2);
				a3 = Libnoise.SCurve5(z - (float)num3);
				break;
			}
			float n = ValueNoise3D(num, num2, num3, seed);
			float n2 = ValueNoise3D(x2, num2, num3, seed);
			float n3 = Libnoise.Lerp(n, n2, a);
			n = ValueNoise3D(num, y2, num3, seed);
			n2 = ValueNoise3D(x2, y2, num3, seed);
			float n4 = Libnoise.Lerp(n, n2, a);
			float n5 = Libnoise.Lerp(n3, n4, a2);
			n = ValueNoise3D(num, num2, z2, seed);
			n2 = ValueNoise3D(x2, num2, z2, seed);
			n3 = Libnoise.Lerp(n, n2, a);
			n = ValueNoise3D(num, y2, z2, seed);
			n2 = ValueNoise3D(x2, y2, z2, seed);
			n4 = Libnoise.Lerp(n, n2, a);
			float n6 = Libnoise.Lerp(n3, n4, a2);
			return Libnoise.Lerp(n5, n6, a3);
		}

		public static float ValueNoise3D(int x, int y, int z, long seed)
		{
			return 1f - (float)IntValueNoise3D(x, y, z, seed) / 1.07374182E+09f;
		}

		protected static int IntValueNoise3D(int x, int y, int z, long seed)
		{
			long num = (1619 * x + 31337 * y + 6971 * z + 1013 * seed) & 0x7FFFFFFF;
			num = ((num >> 13) ^ num);
			return (int)(num * (num * num * 60493 + 19990303) + 1376312589) & 0x7FFFFFFF;
		}

		public float GetValue(float x, float y)
		{
			return ValueCoherentNoise2D(x, y, _seed, _quality);
		}

		public float ValueCoherentNoise2D(float x, float y, long seed, NoiseQuality quality)
		{
			int num = (!((double)x > 0.0)) ? ((int)x - 1) : ((int)x);
			int x2 = num + 1;
			int num2 = (!((double)y > 0.0)) ? ((int)y - 1) : ((int)y);
			int y2 = num2 + 1;
			float a = 0f;
			float a2 = 0f;
			switch (quality)
			{
			case NoiseQuality.Fast:
				a = x - (float)num;
				a2 = y - (float)num2;
				break;
			case NoiseQuality.Standard:
				a = Libnoise.SCurve3(x - (float)num);
				a2 = Libnoise.SCurve3(y - (float)num2);
				break;
			case NoiseQuality.Best:
				a = Libnoise.SCurve5(x - (float)num);
				a2 = Libnoise.SCurve5(y - (float)num2);
				break;
			}
			float n = ValueNoise2D(num, num2, seed);
			float n2 = ValueNoise2D(x2, num2, seed);
			float n3 = Libnoise.Lerp(n, n2, a);
			n = ValueNoise2D(num, y2, seed);
			n2 = ValueNoise2D(x2, y2, seed);
			float n4 = Libnoise.Lerp(n, n2, a);
			return Libnoise.Lerp(n3, n4, a2);
		}

		public float ValueNoise2D(int x, int y, long seed)
		{
			return 1f - (float)IntValueNoise2D(x, y, seed) / 1.07374182E+09f;
		}

		public float ValueNoise2D(int x, int y)
		{
			return ValueNoise2D(x, y, _seed);
		}

		protected int IntValueNoise2D(int x, int y, long seed)
		{
			long num = (1619 * x + 31337 * y + 1013 * seed) & 0x7FFFFFFF;
			num = ((num >> 13) ^ num);
			return (int)(num * (num * num * 60493 + 19990303) + 1376312589) & 0x7FFFFFFF;
		}

		public float GetValue(float x)
		{
			return ValueCoherentNoise1D(x, _seed, _quality);
		}

		public static float ValueCoherentNoise1D(float x, long seed, NoiseQuality quality)
		{
			int num = (!((double)x > 0.0)) ? ((int)x - 1) : ((int)x);
			int x2 = num + 1;
			float a = 0f;
			switch (quality)
			{
			case NoiseQuality.Fast:
				a = x - (float)num;
				break;
			case NoiseQuality.Standard:
				a = Libnoise.SCurve3(x - (float)num);
				break;
			case NoiseQuality.Best:
				a = Libnoise.SCurve5(x - (float)num);
				break;
			}
			float n = ValueNoise1D(num, seed);
			float n2 = ValueNoise1D(x2, seed);
			return Libnoise.Lerp(n, n2, a);
		}

		public static float ValueNoise1D(int x, long seed)
		{
			return 1f - (float)IntValueNoise1D(x, seed) / 1.07374182E+09f;
		}

		public static float ValueNoise1D(int x)
		{
			return ValueNoise1D(x, 0L);
		}

		protected static int IntValueNoise1D(int x, long seed)
		{
			long num = (1619 * x + 1013 * seed) & 0x7FFFFFFF;
			num = ((num >> 13) ^ num);
			return (int)(num * (num * num * 60493 + 19990303) + 1376312589) & 0x7FFFFFFF;
		}
	}
}
