using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Filter
{
	public class Voronoi : FilterModule, IModule3D, IModule
	{
		public const float DEFAULT_DISPLACEMENT = 1f;

		protected float _displacement = 1f;

		protected bool _distance = false;

		public float Displacement
		{
			get
			{
				return _displacement;
			}
			set
			{
				_displacement = value;
			}
		}

		public bool Distance
		{
			get
			{
				return _distance;
			}
			set
			{
				_distance = value;
			}
		}

		public float GetValue(float x, float y, float z)
		{
			x *= _frequency;
			y *= _frequency;
			z *= _frequency;
			int num = (!(x > 0f)) ? ((int)x - 1) : ((int)x);
			int num2 = (!(y > 0f)) ? ((int)y - 1) : ((int)y);
			int num3 = (!(z > 0f)) ? ((int)z - 1) : ((int)z);
			float num4 = 2.14748365E+09f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			for (int i = num3 - 2; i <= num3 + 2; i++)
			{
				for (int j = num2 - 2; j <= num2 + 2; j++)
				{
					for (int k = num - 2; k <= num + 2; k++)
					{
						float num8 = (float)k + _source3D.GetValue((float)k, (float)j, (float)i);
						float num9 = (float)j + _source3D.GetValue((float)k, (float)j, (float)i);
						float num10 = (float)i + _source3D.GetValue((float)k, (float)j, (float)i);
						float num11 = num8 - x;
						float num12 = num9 - y;
						float num13 = num10 - z;
						float num14 = num11 * num11 + num12 * num12 + num13 * num13;
						if (num14 < num4)
						{
							num4 = num14;
							num5 = num8;
							num6 = num9;
							num7 = num10;
						}
					}
				}
			}
			float num18;
			if (_distance)
			{
				float num15 = num5 - x;
				float num16 = num6 - y;
				float num17 = num7 - z;
				num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16 + num17 * num17)) * 1.73205078f - 1f;
			}
			else
			{
				num18 = 0f;
			}
			return num18 + _displacement * _source3D.GetValue((float)(int)Math.Floor((double)num5), (float)(int)Math.Floor((double)num6), (float)(int)Math.Floor((double)num7));
		}
	}
}
