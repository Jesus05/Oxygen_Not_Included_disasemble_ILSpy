namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class ImprovedPerlin : PrimitiveModule, IModule3D, IModule2D, IModule1D, IModule
	{
		protected const int RANDOM_SIZE = 256;

		protected static int[] _source = new int[256]
		{
			151,
			160,
			137,
			91,
			90,
			15,
			131,
			13,
			201,
			95,
			96,
			53,
			194,
			233,
			7,
			225,
			140,
			36,
			103,
			30,
			69,
			142,
			8,
			99,
			37,
			240,
			21,
			10,
			23,
			190,
			6,
			148,
			247,
			120,
			234,
			75,
			0,
			26,
			197,
			62,
			94,
			252,
			219,
			203,
			117,
			35,
			11,
			32,
			57,
			177,
			33,
			88,
			237,
			149,
			56,
			87,
			174,
			20,
			125,
			136,
			171,
			168,
			68,
			175,
			74,
			165,
			71,
			134,
			139,
			48,
			27,
			166,
			77,
			146,
			158,
			231,
			83,
			111,
			229,
			122,
			60,
			211,
			133,
			230,
			220,
			105,
			92,
			41,
			55,
			46,
			245,
			40,
			244,
			102,
			143,
			54,
			65,
			25,
			63,
			161,
			1,
			216,
			80,
			73,
			209,
			76,
			132,
			187,
			208,
			89,
			18,
			169,
			200,
			196,
			135,
			130,
			116,
			188,
			159,
			86,
			164,
			100,
			109,
			198,
			173,
			186,
			3,
			64,
			52,
			217,
			226,
			250,
			124,
			123,
			5,
			202,
			38,
			147,
			118,
			126,
			255,
			82,
			85,
			212,
			207,
			206,
			59,
			227,
			47,
			16,
			58,
			17,
			182,
			189,
			28,
			42,
			223,
			183,
			170,
			213,
			119,
			248,
			152,
			2,
			44,
			154,
			163,
			70,
			221,
			153,
			101,
			155,
			167,
			43,
			172,
			9,
			129,
			22,
			39,
			253,
			19,
			98,
			108,
			110,
			79,
			113,
			224,
			232,
			178,
			185,
			112,
			104,
			218,
			246,
			97,
			228,
			251,
			34,
			242,
			193,
			238,
			210,
			144,
			12,
			191,
			179,
			162,
			241,
			81,
			51,
			145,
			235,
			249,
			14,
			239,
			107,
			49,
			192,
			214,
			31,
			181,
			199,
			106,
			157,
			184,
			84,
			204,
			176,
			115,
			121,
			50,
			45,
			127,
			4,
			150,
			254,
			138,
			236,
			205,
			93,
			222,
			114,
			67,
			29,
			24,
			72,
			243,
			141,
			128,
			195,
			78,
			66,
			215,
			61,
			156,
			180
		};

		protected int[] _random;

		public override int Seed
		{
			get
			{
				return _seed;
			}
			set
			{
				if (_seed != value)
				{
					_seed = value;
					Randomize(_seed);
				}
			}
		}

		public ImprovedPerlin()
			: this(0, NoiseQuality.Standard)
		{
		}

		public ImprovedPerlin(int seed, NoiseQuality quality)
		{
			_seed = seed;
			_quality = quality;
			Randomize(_seed);
		}

		protected void Randomize(int seed)
		{
			_random = new int[512];
			if (seed != 0)
			{
				byte[] buffer = new byte[4];
				Libnoise.UnpackLittleUint32(seed, ref buffer);
				for (int i = 0; i < _source.Length; i++)
				{
					_random[i] = (_source[i] ^ buffer[0]);
					_random[i] ^= buffer[1];
					_random[i] ^= buffer[2];
					_random[i] ^= buffer[3];
					_random[i + 256] = _random[i];
				}
			}
			else
			{
				for (int j = 0; j < 256; j++)
				{
					_random[j + 256] = (_random[j] = _source[j]);
				}
			}
		}

		public float GetValue(float x, float y, float z)
		{
			int num = (!((double)x > 0.0)) ? ((int)x - 1) : ((int)x);
			int num2 = (!((double)y > 0.0)) ? ((int)y - 1) : ((int)y);
			int num3 = (!((double)z > 0.0)) ? ((int)z - 1) : ((int)z);
			int num4 = num & 0xFF;
			int num5 = num2 & 0xFF;
			int num6 = num3 & 0xFF;
			x -= (float)num;
			y -= (float)num2;
			z -= (float)num3;
			float a = 0f;
			float a2 = 0f;
			float a3 = 0f;
			switch (_quality)
			{
			case NoiseQuality.Fast:
				a = x;
				a2 = y;
				a3 = z;
				break;
			case NoiseQuality.Standard:
				a = Libnoise.SCurve3(x);
				a2 = Libnoise.SCurve3(y);
				a3 = Libnoise.SCurve3(z);
				break;
			case NoiseQuality.Best:
				a = Libnoise.SCurve5(x);
				a2 = Libnoise.SCurve5(y);
				a3 = Libnoise.SCurve5(z);
				break;
			}
			int num7 = _random[num4] + num5;
			int num8 = _random[num7] + num6;
			int num9 = _random[num7 + 1] + num6;
			int num10 = _random[num4 + 1] + num5;
			int num11 = _random[num10] + num6;
			int num12 = _random[num10 + 1] + num6;
			return Libnoise.Lerp(Libnoise.Lerp(Libnoise.Lerp(Grad(_random[num8], x, y, z), Grad(_random[num11], x - 1f, y, z), a), Libnoise.Lerp(Grad(_random[num9], x, y - 1f, z), Grad(_random[num12], x - 1f, y - 1f, z), a), a2), Libnoise.Lerp(Libnoise.Lerp(Grad(_random[num8 + 1], x, y, z - 1f), Grad(_random[num11 + 1], x - 1f, y, z - 1f), a), Libnoise.Lerp(Grad(_random[num9 + 1], x, y - 1f, z - 1f), Grad(_random[num12 + 1], x - 1f, y - 1f, z - 1f), a), a2), a3);
		}

		protected float Grad(int hash, float x, float y, float z)
		{
			int num = hash & 0xF;
			float num2 = (num >= 8) ? y : x;
			float num3 = (num < 4) ? y : ((num != 12 && num != 14) ? z : x);
			return (((num & 1) != 0) ? (0f - num2) : num2) + (((num & 2) != 0) ? (0f - num3) : num3);
		}

		public float GetValue(float x, float y)
		{
			int num = (!((double)x > 0.0)) ? ((int)x - 1) : ((int)x);
			int num2 = (!((double)y > 0.0)) ? ((int)y - 1) : ((int)y);
			int num3 = num & 0xFF;
			int num4 = num2 & 0xFF;
			x -= (float)num;
			x -= (float)num2;
			float a = 0f;
			float a2 = 0f;
			switch (_quality)
			{
			case NoiseQuality.Fast:
				a = x;
				a2 = y;
				break;
			case NoiseQuality.Standard:
				a = Libnoise.SCurve3(x);
				a2 = Libnoise.SCurve3(y);
				break;
			case NoiseQuality.Best:
				a = Libnoise.SCurve5(x);
				a2 = Libnoise.SCurve5(y);
				break;
			}
			int num5 = _random[num3] + num4;
			int num6 = _random[num3 + 1] + num4;
			return Libnoise.Lerp(Libnoise.Lerp(Grad(_random[num5], x, y), Grad(_random[num6], x - 1f, y), a), Libnoise.Lerp(Grad(_random[num5 + 1], x, y - 1f), Grad(_random[num6 + 1], x - 1f, y - 1f), a), a2);
		}

		protected float Grad(int hash, float x, float y)
		{
			int num = hash & 3;
			float num2 = ((num & 2) != 0) ? (0f - x) : x;
			float num3 = ((num & 1) != 0) ? (0f - y) : y;
			return num2 + num3;
		}

		public float GetValue(float x)
		{
			int num = (!((double)x > 0.0)) ? ((int)x - 1) : ((int)x);
			int num2 = num & 0xFF;
			x -= (float)num;
			float a = 0f;
			switch (_quality)
			{
			case NoiseQuality.Fast:
				a = x;
				break;
			case NoiseQuality.Standard:
				a = Libnoise.SCurve3(x);
				break;
			case NoiseQuality.Best:
				a = Libnoise.SCurve5(x);
				break;
			}
			return Libnoise.Lerp(Grad(_random[num2], x), Grad(_random[num2 + 1], x - 1f), a);
		}

		protected float Grad(int hash, float x)
		{
			return ((hash & 1) != 0) ? (0f - x) : x;
		}
	}
}
