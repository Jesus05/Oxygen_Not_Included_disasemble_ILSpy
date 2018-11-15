namespace LibNoiseDotNet.Graphics.Tools.Noise.Primitive
{
	public class SimplexPerlin : ImprovedPerlin, IModule4D, IModule3D, IModule2D, IModule
	{
		protected static float F2 = 0.3660254f;

		protected static float G2 = 0.211324871f;

		protected static float G22 = G2 * 2f - 1f;

		protected static float F3 = 0.333333343f;

		protected static float G3 = 0.166666672f;

		protected static float F4 = 0.309017f;

		protected static float G4 = 0.1381966f;

		protected static float G42 = G4 * 2f;

		protected static float G43 = G4 * 3f;

		protected static float G44 = G4 * 4f - 1f;

		protected static int[][] _grad3 = new int[12][]
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

		protected static int[][] _grad4 = new int[32][]
		{
			new int[4]
			{
				0,
				1,
				1,
				1
			},
			new int[4]
			{
				0,
				1,
				1,
				-1
			},
			new int[4]
			{
				0,
				1,
				-1,
				1
			},
			new int[4]
			{
				0,
				1,
				-1,
				-1
			},
			new int[4]
			{
				0,
				-1,
				1,
				1
			},
			new int[4]
			{
				0,
				-1,
				1,
				-1
			},
			new int[4]
			{
				0,
				-1,
				-1,
				1
			},
			new int[4]
			{
				0,
				-1,
				-1,
				-1
			},
			new int[4]
			{
				1,
				0,
				1,
				1
			},
			new int[4]
			{
				1,
				0,
				1,
				-1
			},
			new int[4]
			{
				1,
				0,
				-1,
				1
			},
			new int[4]
			{
				1,
				0,
				-1,
				-1
			},
			new int[4]
			{
				-1,
				0,
				1,
				1
			},
			new int[4]
			{
				-1,
				0,
				1,
				-1
			},
			new int[4]
			{
				-1,
				0,
				-1,
				1
			},
			new int[4]
			{
				-1,
				0,
				-1,
				-1
			},
			new int[4]
			{
				1,
				1,
				0,
				1
			},
			new int[4]
			{
				1,
				1,
				0,
				-1
			},
			new int[4]
			{
				1,
				-1,
				0,
				1
			},
			new int[4]
			{
				1,
				-1,
				0,
				-1
			},
			new int[4]
			{
				-1,
				1,
				0,
				1
			},
			new int[4]
			{
				-1,
				1,
				0,
				-1
			},
			new int[4]
			{
				-1,
				-1,
				0,
				1
			},
			new int[4]
			{
				-1,
				-1,
				0,
				-1
			},
			new int[4]
			{
				1,
				1,
				1,
				0
			},
			new int[4]
			{
				1,
				1,
				-1,
				0
			},
			new int[4]
			{
				1,
				-1,
				1,
				0
			},
			new int[4]
			{
				1,
				-1,
				-1,
				0
			},
			new int[4]
			{
				-1,
				1,
				1,
				0
			},
			new int[4]
			{
				-1,
				1,
				-1,
				0
			},
			new int[4]
			{
				-1,
				-1,
				1,
				0
			},
			new int[4]
			{
				-1,
				-1,
				-1,
				0
			}
		};

		protected static int[][] _simplex = new int[64][]
		{
			new int[4]
			{
				0,
				1,
				2,
				3
			},
			new int[4]
			{
				0,
				1,
				3,
				2
			},
			new int[4],
			new int[4]
			{
				0,
				2,
				3,
				1
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				1,
				2,
				3,
				0
			},
			new int[4]
			{
				0,
				2,
				1,
				3
			},
			new int[4],
			new int[4]
			{
				0,
				3,
				1,
				2
			},
			new int[4]
			{
				0,
				3,
				2,
				1
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				1,
				3,
				2,
				0
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				1,
				2,
				0,
				3
			},
			new int[4],
			new int[4]
			{
				1,
				3,
				0,
				2
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				2,
				3,
				0,
				1
			},
			new int[4]
			{
				2,
				3,
				1,
				0
			},
			new int[4]
			{
				1,
				0,
				2,
				3
			},
			new int[4]
			{
				1,
				0,
				3,
				2
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				2,
				0,
				3,
				1
			},
			new int[4],
			new int[4]
			{
				2,
				1,
				3,
				0
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				2,
				0,
				1,
				3
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				3,
				0,
				1,
				2
			},
			new int[4]
			{
				3,
				0,
				2,
				1
			},
			new int[4],
			new int[4]
			{
				3,
				1,
				2,
				0
			},
			new int[4]
			{
				2,
				1,
				0,
				3
			},
			new int[4],
			new int[4],
			new int[4],
			new int[4]
			{
				3,
				1,
				0,
				2
			},
			new int[4],
			new int[4]
			{
				3,
				2,
				0,
				1
			},
			new int[4]
			{
				3,
				2,
				1,
				0
			}
		};

		public SimplexPerlin()
			: base(0, NoiseQuality.Standard)
		{
		}

		public SimplexPerlin(int seed, NoiseQuality quality)
			: base(seed, quality)
		{
		}

		public float GetValue(float x, float y, float z, float w)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = (x + y + z + w) * F4;
			int num7 = Libnoise.FastFloor(x + num6);
			int num8 = Libnoise.FastFloor(y + num6);
			int num9 = Libnoise.FastFloor(z + num6);
			int num10 = Libnoise.FastFloor(w + num6);
			float num11 = (float)(num7 + num8 + num9 + num10) * G4;
			float num12 = x - ((float)num7 - num11);
			float num13 = y - ((float)num8 - num11);
			float num14 = z - ((float)num9 - num11);
			float num15 = w - ((float)num10 - num11);
			int num16 = 0;
			if (num12 > num13)
			{
				num16 = 32;
			}
			if (num12 > num14)
			{
				num16 |= 0x10;
			}
			if (num13 > num14)
			{
				num16 |= 8;
			}
			if (num12 > num15)
			{
				num16 |= 4;
			}
			if (num13 > num15)
			{
				num16 |= 2;
			}
			if (num14 > num15)
			{
				num16 |= 1;
			}
			int[] array = _simplex[num16];
			int num17 = (array[0] >= 3) ? 1 : 0;
			int num18 = (array[1] >= 3) ? 1 : 0;
			int num19 = (array[2] >= 3) ? 1 : 0;
			int num20 = (array[3] >= 3) ? 1 : 0;
			int num21 = (array[0] >= 2) ? 1 : 0;
			int num22 = (array[1] >= 2) ? 1 : 0;
			int num23 = (array[2] >= 2) ? 1 : 0;
			int num24 = (array[3] >= 2) ? 1 : 0;
			int num25 = (array[0] >= 1) ? 1 : 0;
			int num26 = (array[1] >= 1) ? 1 : 0;
			int num27 = (array[2] >= 1) ? 1 : 0;
			int num28 = (array[3] >= 1) ? 1 : 0;
			float num29 = num12 - (float)num17 + G4;
			float num30 = num13 - (float)num18 + G4;
			float num31 = num14 - (float)num19 + G4;
			float num32 = num15 - (float)num20 + G4;
			float num33 = num12 - (float)num21 + G42;
			float num34 = num13 - (float)num22 + G42;
			float num35 = num14 - (float)num23 + G42;
			float num36 = num15 - (float)num24 + G42;
			float num37 = num12 - (float)num25 + G43;
			float num38 = num13 - (float)num26 + G43;
			float num39 = num14 - (float)num27 + G43;
			float num40 = num15 - (float)num28 + G43;
			float num41 = num12 + G44;
			float num42 = num13 + G44;
			float num43 = num14 + G44;
			float num44 = num15 + G44;
			int num45 = num7 & 0xFF;
			int num46 = num8 & 0xFF;
			int num47 = num9 & 0xFF;
			int num48 = num10 & 0xFF;
			float num49 = 0.6f - num12 * num12 - num13 * num13 - num14 * num14 - num15 * num15;
			if (num49 > 0f)
			{
				num49 *= num49;
				int num50 = _random[num45 + _random[num46 + _random[num47 + _random[num48]]]] % 32;
				num = num49 * num49 * Dot(_grad4[num50], num12, num13, num14, num15);
			}
			float num51 = 0.6f - num29 * num29 - num30 * num30 - num31 * num31 - num32 * num32;
			if (num51 > 0f)
			{
				num51 *= num51;
				int num52 = _random[num45 + num17 + _random[num46 + num18 + _random[num47 + num19 + _random[num48 + num20]]]] % 32;
				num2 = num51 * num51 * Dot(_grad4[num52], num29, num30, num31, num32);
			}
			float num53 = 0.6f - num33 * num33 - num34 * num34 - num35 * num35 - num36 * num36;
			if (num53 > 0f)
			{
				num53 *= num53;
				int num54 = _random[num45 + num21 + _random[num46 + num22 + _random[num47 + num23 + _random[num48 + num24]]]] % 32;
				num3 = num53 * num53 * Dot(_grad4[num54], num33, num34, num35, num36);
			}
			float num55 = 0.6f - num37 * num37 - num38 * num38 - num39 * num39 - num40 * num40;
			if (num55 > 0f)
			{
				num55 *= num55;
				int num56 = _random[num45 + num25 + _random[num46 + num26 + _random[num47 + num27 + _random[num48 + num28]]]] % 32;
				num4 = num55 * num55 * Dot(_grad4[num56], num37, num38, num39, num40);
			}
			float num57 = 0.6f - num41 * num41 - num42 * num42 - num43 * num43 - num44 * num44;
			if (num57 > 0f)
			{
				num57 *= num57;
				int num58 = _random[num45 + 1 + _random[num46 + 1 + _random[num47 + 1 + _random[num48 + 1]]]] % 32;
				num5 = num57 * num57 * Dot(_grad4[num58], num41, num42, num43, num44);
			}
			return 27f * (num + num2 + num3 + num4 + num5);
		}

		protected float Dot(int[] g, float x, float y, float z, float t)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z + (float)g[3] * t;
		}

		public new float GetValue(float x, float y, float z)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = (x + y + z) * F3;
			int num6 = Libnoise.FastFloor(x + num5);
			int num7 = Libnoise.FastFloor(y + num5);
			int num8 = Libnoise.FastFloor(z + num5);
			float num9 = (float)(num6 + num7 + num8) * G3;
			float num10 = x - ((float)num6 - num9);
			float num11 = y - ((float)num7 - num9);
			float num12 = z - ((float)num8 - num9);
			int num13;
			int num14;
			int num15;
			int num16;
			int num17;
			int num18;
			if (num10 >= num11)
			{
				if (num11 >= num12)
				{
					num13 = 1;
					num14 = 0;
					num15 = 0;
					num16 = 1;
					num17 = 1;
					num18 = 0;
				}
				else if (num10 >= num12)
				{
					num13 = 1;
					num14 = 0;
					num15 = 0;
					num16 = 1;
					num17 = 0;
					num18 = 1;
				}
				else
				{
					num13 = 0;
					num14 = 0;
					num15 = 1;
					num16 = 1;
					num17 = 0;
					num18 = 1;
				}
			}
			else if (num11 < num12)
			{
				num13 = 0;
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 1;
				num18 = 1;
			}
			else if (num10 < num12)
			{
				num13 = 0;
				num14 = 1;
				num15 = 0;
				num16 = 0;
				num17 = 1;
				num18 = 1;
			}
			else
			{
				num13 = 0;
				num14 = 1;
				num15 = 0;
				num16 = 1;
				num17 = 1;
				num18 = 0;
			}
			float num19 = num10 - (float)num13 + G3;
			float num20 = num11 - (float)num14 + G3;
			float num21 = num12 - (float)num15 + G3;
			float num22 = num10 - (float)num16 + F3;
			float num23 = num11 - (float)num17 + F3;
			float num24 = num12 - (float)num18 + F3;
			float num25 = num10 - 0.5f;
			float num26 = num11 - 0.5f;
			float num27 = num12 - 0.5f;
			int num28 = num6 & 0xFF;
			int num29 = num7 & 0xFF;
			int num30 = num8 & 0xFF;
			float num31 = 0.6f - num10 * num10 - num11 * num11 - num12 * num12;
			if (num31 > 0f)
			{
				num31 *= num31;
				int num32 = _random[num28 + _random[num29 + _random[num30]]] % 12;
				num = num31 * num31 * Dot(_grad3[num32], num10, num11, num12);
			}
			float num33 = 0.6f - num19 * num19 - num20 * num20 - num21 * num21;
			if (num33 > 0f)
			{
				num33 *= num33;
				int num34 = _random[num28 + num13 + _random[num29 + num14 + _random[num30 + num15]]] % 12;
				num2 = num33 * num33 * Dot(_grad3[num34], num19, num20, num21);
			}
			float num35 = 0.6f - num22 * num22 - num23 * num23 - num24 * num24;
			if (num35 > 0f)
			{
				num35 *= num35;
				int num36 = _random[num28 + num16 + _random[num29 + num17 + _random[num30 + num18]]] % 12;
				num3 = num35 * num35 * Dot(_grad3[num36], num22, num23, num24);
			}
			float num37 = 0.6f - num25 * num25 - num26 * num26 - num27 * num27;
			if (num37 > 0f)
			{
				num37 *= num37;
				int num38 = _random[num28 + 1 + _random[num29 + 1 + _random[num30 + 1]]] % 12;
				num4 = num37 * num37 * Dot(_grad3[num38], num25, num26, num27);
			}
			return 32f * (num + num2 + num3 + num4);
		}

		protected float Dot(int[] g, float x, float y, float z)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z;
		}

		public new float GetValue(float x, float y)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = (x + y) * F2;
			int num5 = Libnoise.FastFloor(x + num4);
			int num6 = Libnoise.FastFloor(y + num4);
			float num7 = (float)(num5 + num6) * G2;
			float num8 = x - ((float)num5 - num7);
			float num9 = y - ((float)num6 - num7);
			int num10;
			int num11;
			if (num8 > num9)
			{
				num10 = 1;
				num11 = 0;
			}
			else
			{
				num10 = 0;
				num11 = 1;
			}
			float num12 = num8 - (float)num10 + G2;
			float num13 = num9 - (float)num11 + G2;
			float num14 = num8 + G22;
			float num15 = num9 + G22;
			int num16 = num5 & 0xFF;
			int num17 = num6 & 0xFF;
			float num18 = 0.5f - num8 * num8 - num9 * num9;
			if (num18 > 0f)
			{
				num18 *= num18;
				int num19 = _random[num16 + _random[num17]] % 12;
				num = num18 * num18 * Dot(_grad3[num19], num8, num9);
			}
			float num20 = 0.5f - num12 * num12 - num13 * num13;
			if (num20 > 0f)
			{
				num20 *= num20;
				int num21 = _random[num16 + num10 + _random[num17 + num11]] % 12;
				num2 = num20 * num20 * Dot(_grad3[num21], num12, num13);
			}
			float num22 = 0.5f - num14 * num14 - num15 * num15;
			if (num22 > 0f)
			{
				num22 *= num22;
				int num23 = _random[num16 + 1 + _random[num17 + 1]] % 12;
				num3 = num22 * num22 * Dot(_grad3[num23], num14, num15);
			}
			return 70f * (num + num2 + num3);
		}

		protected float Dot(int[] g, float x, float y)
		{
			return (float)g[0] * x + (float)g[1] * y;
		}
	}
}
