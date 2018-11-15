using System;

public static class PerlinSimplexNoise
{
	private static int[][] grad3;

	private static int[] p;

	private static readonly int[] perm;

	static PerlinSimplexNoise()
	{
		grad3 = new int[12][]
		{
			new int[3]
			{
				1,
				1,
				0
			},
			new int[3]
			{
				-1,
				1,
				0
			},
			new int[3]
			{
				1,
				-1,
				0
			},
			new int[3]
			{
				-1,
				-1,
				0
			},
			new int[3]
			{
				1,
				0,
				1
			},
			new int[3]
			{
				-1,
				0,
				1
			},
			new int[3]
			{
				1,
				0,
				-1
			},
			new int[3]
			{
				-1,
				0,
				-1
			},
			new int[3]
			{
				0,
				1,
				1
			},
			new int[3]
			{
				0,
				-1,
				1
			},
			new int[3]
			{
				0,
				1,
				-1
			},
			new int[3]
			{
				0,
				-1,
				-1
			}
		};
		p = new int[256]
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
		perm = new int[512];
		for (int i = 0; i < 512; i++)
		{
			perm[i] = p[i & 0xFF];
		}
	}

	private static int fastfloor(float x)
	{
		return (!(x > 0f)) ? ((int)x - 1) : ((int)x);
	}

	private static float dot(int[] g, float x, float y)
	{
		return (float)g[0] * x + (float)g[1] * y;
	}

	private static float dot(int[] g, float x, float y, float z)
	{
		return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z;
	}

	private static float dot(int[] g, float x, float y, float z, float w)
	{
		return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z + (float)g[3] * w;
	}

	public static float noise(float xin, float yin, float zin)
	{
		float num = 0.333333343f;
		float num2 = (xin + yin + zin) * num;
		int num3 = fastfloor(xin + num2);
		int num4 = fastfloor(yin + num2);
		int num5 = fastfloor(zin + num2);
		float num6 = 0.166666672f;
		float num7 = (float)(num3 + num4 + num5) * num6;
		float num8 = (float)num3 - num7;
		float num9 = (float)num4 - num7;
		float num10 = (float)num5 - num7;
		float num11 = xin - num8;
		float num12 = yin - num9;
		float num13 = zin - num10;
		int num14;
		int num15;
		int num16;
		int num17;
		int num18;
		int num19;
		if (num11 >= num12)
		{
			if (num12 >= num13)
			{
				num14 = 1;
				num15 = 0;
				num16 = 0;
				num17 = 1;
				num18 = 1;
				num19 = 0;
			}
			else if (num11 >= num13)
			{
				num14 = 1;
				num15 = 0;
				num16 = 0;
				num17 = 1;
				num18 = 0;
				num19 = 1;
			}
			else
			{
				num14 = 0;
				num15 = 0;
				num16 = 1;
				num17 = 1;
				num18 = 0;
				num19 = 1;
			}
		}
		else if (num12 < num13)
		{
			num14 = 0;
			num15 = 0;
			num16 = 1;
			num17 = 0;
			num18 = 1;
			num19 = 1;
		}
		else if (num11 < num13)
		{
			num14 = 0;
			num15 = 1;
			num16 = 0;
			num17 = 0;
			num18 = 1;
			num19 = 1;
		}
		else
		{
			num14 = 0;
			num15 = 1;
			num16 = 0;
			num17 = 1;
			num18 = 1;
			num19 = 0;
		}
		float num20 = num11 - (float)num14 + num6;
		float num21 = num12 - (float)num15 + num6;
		float num22 = num13 - (float)num16 + num6;
		float num23 = num11 - (float)num17 + 2f * num6;
		float num24 = num12 - (float)num18 + 2f * num6;
		float num25 = num13 - (float)num19 + 2f * num6;
		float num26 = num11 - 1f + 3f * num6;
		float num27 = num12 - 1f + 3f * num6;
		float num28 = num13 - 1f + 3f * num6;
		int num29 = num3 & 0xFF;
		int num30 = num4 & 0xFF;
		int num31 = num5 & 0xFF;
		int num32 = perm[num29 + perm[num30 + perm[num31]]] % 12;
		int num33 = perm[num29 + num14 + perm[num30 + num15 + perm[num31 + num16]]] % 12;
		int num34 = perm[num29 + num17 + perm[num30 + num18 + perm[num31 + num19]]] % 12;
		int num35 = perm[num29 + 1 + perm[num30 + 1 + perm[num31 + 1]]] % 12;
		float num36 = 0.6f - num11 * num11 - num12 * num12 - num13 * num13;
		float num37;
		if (num36 < 0f)
		{
			num37 = 0f;
		}
		else
		{
			num36 *= num36;
			num37 = num36 * num36 * dot(grad3[num32], num11, num12, num13);
		}
		float num38 = 0.6f - num20 * num20 - num21 * num21 - num22 * num22;
		float num39;
		if (num38 < 0f)
		{
			num39 = 0f;
		}
		else
		{
			num38 *= num38;
			num39 = num38 * num38 * dot(grad3[num33], num20, num21, num22);
		}
		float num40 = 0.6f - num23 * num23 - num24 * num24 - num25 * num25;
		float num41;
		if (num40 < 0f)
		{
			num41 = 0f;
		}
		else
		{
			num40 *= num40;
			num41 = num40 * num40 * dot(grad3[num34], num23, num24, num25);
		}
		float num42 = 0.6f - num26 * num26 - num27 * num27 - num28 * num28;
		float num43;
		if (num42 < 0f)
		{
			num43 = 0f;
		}
		else
		{
			num42 *= num42;
			num43 = num42 * num42 * dot(grad3[num35], num26, num27, num28);
		}
		return 32f * (num37 + num39 + num41 + num43);
	}

	public static float noise(float xin, float yin)
	{
		float num = (float)(0.5 * (Math.Sqrt(3.0) - 1.0));
		float num2 = (xin + yin) * num;
		int num3 = fastfloor(xin + num2);
		int num4 = fastfloor(yin + num2);
		float num5 = (float)((3.0 - Math.Sqrt(3.0)) / 6.0);
		float num6 = (float)(num3 + num4) * num5;
		float num7 = (float)num3 - num6;
		float num8 = (float)num4 - num6;
		float num9 = xin - num7;
		float num10 = yin - num8;
		int num11;
		int num12;
		if (num9 > num10)
		{
			num11 = 1;
			num12 = 0;
		}
		else
		{
			num11 = 0;
			num12 = 1;
		}
		float num13 = num9 - (float)num11 + num5;
		float num14 = num10 - (float)num12 + num5;
		float num15 = num9 - 1f + 2f * num5;
		float num16 = num10 - 1f + 2f * num5;
		int num17 = num3 & 0xFF;
		int num18 = num4 & 0xFF;
		int num19 = perm[num17 + perm[num18]] % 12;
		int num20 = perm[num17 + num11 + perm[num18 + num12]] % 12;
		int num21 = perm[num17 + 1 + perm[num18 + 1]] % 12;
		float num22 = 0.5f - num9 * num9 - num10 * num10;
		float num23;
		if (num22 < 0f)
		{
			num23 = 0f;
		}
		else
		{
			num22 *= num22;
			num23 = num22 * num22 * dot(grad3[num19], num9, num10);
		}
		float num24 = 0.5f - num13 * num13 - num14 * num14;
		float num25;
		if (num24 < 0f)
		{
			num25 = 0f;
		}
		else
		{
			num24 *= num24;
			num25 = num24 * num24 * dot(grad3[num20], num13, num14);
		}
		float num26 = 0.5f - num15 * num15 - num16 * num16;
		float num27;
		if (num26 < 0f)
		{
			num27 = 0f;
		}
		else
		{
			num26 *= num26;
			num27 = num26 * num26 * dot(grad3[num21], num15, num16);
		}
		float num28 = 70f * (num23 + num25 + num27);
		return (num28 + 1f) * 0.5f;
	}
}
