using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HUSL
{
	public class ColorConverter
	{
		protected static double[][] M = new double[3][]
		{
			new double[3]
			{
				3.2409699419045208,
				-1.537383177570093,
				-0.498610760293
			},
			new double[3]
			{
				-0.96924363628087,
				1.87596750150772,
				0.041555057407175
			},
			new double[3]
			{
				0.055630079696993,
				-0.20397695888897,
				1.0569715142428779
			}
		};

		protected static double[][] MInv = new double[3][]
		{
			new double[3]
			{
				0.41239079926595,
				0.35758433938387,
				0.18048078840183
			},
			new double[3]
			{
				0.21263900587151,
				0.71516867876775,
				0.072192315360733
			},
			new double[3]
			{
				0.019330818715591,
				0.11919477979462,
				0.95053215224966
			}
		};

		protected static double RefX = 0.95045592705167;

		protected static double RefY = 1.0;

		protected static double RefZ = 1.089057750759878;

		protected static double RefU = 0.19783000664283;

		protected static double RefV = 0.46831999493879;

		protected static double Kappa = 903.2962962;

		protected static double Epsilon = 0.0088564516;

		protected static IList<double[]> GetBounds(double L)
		{
			List<double[]> list = new List<double[]>();
			double num = Math.Pow(L + 16.0, 3.0) / 1560896.0;
			double num2 = (!(num > Epsilon)) ? (L / Kappa) : num;
			for (int i = 0; i < 3; i++)
			{
				double num3 = M[i][0];
				double num4 = M[i][1];
				double num5 = M[i][2];
				for (int j = 0; j < 2; j++)
				{
					double num6 = (284517.0 * num3 - 94839.0 * num5) * num2;
					double num7 = (838422.0 * num5 + 769860.0 * num4 + 731718.0 * num3) * L * num2 - (double)(769860 * j) * L;
					double num8 = (632260.0 * num5 - 126452.0 * num4) * num2 + (double)(126452 * j);
					list.Add(new double[2]
					{
						num6 / num8,
						num7 / num8
					});
				}
			}
			return list;
		}

		protected static double IntersectLineLine(IList<double> lineA, IList<double> lineB)
		{
			return (lineA[1] - lineB[1]) / (lineB[0] - lineA[0]);
		}

		protected static double DistanceFromPole(IList<double> point)
		{
			return Math.Sqrt(Math.Pow(point[0], 2.0) + Math.Pow(point[1], 2.0));
		}

		protected static bool LengthOfRayUntilIntersect(double theta, IList<double> line, out double length)
		{
			length = line[1] / (Math.Sin(theta) - line[0] * Math.Cos(theta));
			return length >= 0.0;
		}

		protected static double MaxSafeChromaForL(double L)
		{
			IList<double[]> bounds = GetBounds(L);
			double num = 1.7976931348623157E+308;
			for (int i = 0; i < 2; i++)
			{
				double num2 = bounds[i][0];
				double num3 = bounds[i][1];
				double[] lineA = new double[2]
				{
					num2,
					num3
				};
				double num4 = IntersectLineLine(lineA, new double[2]
				{
					-1.0 / num2,
					0.0
				});
				double val = DistanceFromPole(new double[2]
				{
					num4,
					num3 + num4 * num2
				});
				num = Math.Min(num, val);
			}
			return num;
		}

		protected static double MaxChromaForLH(double L, double H)
		{
			double theta = H / 360.0 * 3.1415926535897931 * 2.0;
			IList<double[]> bounds = GetBounds(L);
			double num = 1.7976931348623157E+308;
			foreach (double[] item in bounds)
			{
				if (LengthOfRayUntilIntersect(theta, item, out double length))
				{
					num = Math.Min(num, length);
				}
			}
			return num;
		}

		protected static double DotProduct(IList<double> a, IList<double> b)
		{
			double num = 0.0;
			for (int i = 0; i < a.Count; i++)
			{
				num += a[i] * b[i];
			}
			return num;
		}

		protected static double Round(double value, int places)
		{
			double num = Math.Pow(10.0, (double)places);
			return Math.Round(value * num) / num;
		}

		protected static double FromLinear(double c)
		{
			if (c <= 0.0031308)
			{
				return 12.92 * c;
			}
			return 1.055 * Math.Pow(c, 0.41666666666666669) - 0.055;
		}

		protected static double ToLinear(double c)
		{
			if (c > 0.04045)
			{
				return Math.Pow((c + 0.055) / 1.055, 2.4);
			}
			return c / 12.92;
		}

		protected static IList<int> RGBPrepare(IList<double> tuple)
		{
			for (int i = 0; i < tuple.Count; i++)
			{
				tuple[i] = Round(tuple[i], 3);
			}
			for (int j = 0; j < tuple.Count; j++)
			{
				double num = tuple[j];
				if (num < -0.0001 || num > 1.0001)
				{
					throw new Exception("Illegal rgb value: " + num);
				}
			}
			int[] array = new int[tuple.Count];
			for (int k = 0; k < tuple.Count; k++)
			{
				array[k] = (int)Math.Round(tuple[k] * 255.0);
			}
			return array;
		}

		protected static double YToL(double Y)
		{
			if (Y <= Epsilon)
			{
				return Y / RefY * Kappa;
			}
			return 116.0 * Math.Pow(Y / RefY, 0.33333333333333331) - 16.0;
		}

		protected static double LToY(double L)
		{
			if (L <= 8.0)
			{
				return RefY * L / Kappa;
			}
			return RefY * Math.Pow((L + 16.0) / 116.0, 3.0);
		}

		public static IList<double> XYZToRGB(IList<double> tuple)
		{
			return new double[3]
			{
				FromLinear(DotProduct(M[0], tuple)),
				FromLinear(DotProduct(M[1], tuple)),
				FromLinear(DotProduct(M[2], tuple))
			};
		}

		public static IList<double> RGBToXYZ(IList<double> tuple)
		{
			double[] b = new double[3]
			{
				ToLinear(tuple[0]),
				ToLinear(tuple[1]),
				ToLinear(tuple[2])
			};
			return new double[3]
			{
				DotProduct(MInv[0], b),
				DotProduct(MInv[1], b),
				DotProduct(MInv[2], b)
			};
		}

		public static IList<double> XYZToLUV(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			double num4 = 4.0 * num / (num + 15.0 * num2 + 3.0 * num3);
			double num5 = 9.0 * num2 / (num + 15.0 * num2 + 3.0 * num3);
			double num6 = YToL(num2);
			if (num6 == 0.0)
			{
				return new double[3];
			}
			double num7 = 13.0 * num6 * (num4 - RefU);
			double num8 = 13.0 * num6 * (num5 - RefV);
			return new double[3]
			{
				num6,
				num7,
				num8
			};
		}

		public static IList<double> LUVToXYZ(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			if (num == 0.0)
			{
				return new double[3];
			}
			double num4 = num2 / (13.0 * num) + RefU;
			double num5 = num3 / (13.0 * num) + RefV;
			double num6 = LToY(num);
			double num7 = 0.0 - 9.0 * num6 * num4 / ((num4 - 4.0) * num5 - num4 * num5);
			double num8 = (9.0 * num6 - 15.0 * num5 * num6 - num5 * num7) / (3.0 * num5);
			return new double[3]
			{
				num7,
				num6,
				num8
			};
		}

		public static IList<double> LUVToLCH(IList<double> tuple)
		{
			double num = tuple[0];
			double x = tuple[1];
			double num2 = tuple[2];
			double num3 = Math.Pow(Math.Pow(x, 2.0) + Math.Pow(num2, 2.0), 0.5);
			double num4 = Math.Atan2(num2, x);
			double num5 = num4 * 180.0 / 3.1415926535897931;
			if (num5 < 0.0)
			{
				num5 = 360.0 + num5;
			}
			return new double[3]
			{
				num,
				num3,
				num5
			};
		}

		public static IList<double> LCHToLUV(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			double num4 = num3 / 360.0 * 2.0 * 3.1415926535897931;
			double num5 = Math.Cos(num4) * num2;
			double num6 = Math.Sin(num4) * num2;
			return new double[3]
			{
				num,
				num5,
				num6
			};
		}

		public static IList<double> HUSLToLCH(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			if (num3 > 99.9999999)
			{
				return new double[3]
				{
					100.0,
					0.0,
					num
				};
			}
			if (num3 < 1E-08)
			{
				return new double[3]
				{
					0.0,
					0.0,
					num
				};
			}
			double num4 = MaxChromaForLH(num3, num);
			double num5 = num4 / 100.0 * num2;
			return new double[3]
			{
				num3,
				num5,
				num
			};
		}

		public static IList<double> LCHToHUSL(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			if (num > 99.9999999)
			{
				return new double[3]
				{
					num3,
					0.0,
					100.0
				};
			}
			if (num < 1E-08)
			{
				return new double[3]
				{
					num3,
					0.0,
					0.0
				};
			}
			double num4 = MaxChromaForLH(num, num3);
			double num5 = num2 / num4 * 100.0;
			return new double[3]
			{
				num3,
				num5,
				num
			};
		}

		public static IList<double> HUSLPToLCH(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			if (num3 > 99.9999999)
			{
				return new double[3]
				{
					100.0,
					0.0,
					num
				};
			}
			if (num3 < 1E-08)
			{
				return new double[3]
				{
					0.0,
					0.0,
					num
				};
			}
			double num4 = MaxSafeChromaForL(num3);
			double num5 = num4 / 100.0 * num2;
			return new double[3]
			{
				num3,
				num5,
				num
			};
		}

		public static IList<double> LCHToHUSLP(IList<double> tuple)
		{
			double num = tuple[0];
			double num2 = tuple[1];
			double num3 = tuple[2];
			if (num > 99.9999999)
			{
				return new double[3]
				{
					num3,
					0.0,
					100.0
				};
			}
			if (num < 1E-08)
			{
				return new double[3]
				{
					num3,
					0.0,
					0.0
				};
			}
			double num4 = MaxSafeChromaForL(num);
			double num5 = num2 / num4 * 100.0;
			return new double[3]
			{
				num3,
				num5,
				num
			};
		}

		public static string RGBToHex(IList<double> tuple)
		{
			IList<int> list = RGBPrepare(tuple);
			return string.Format("#{0}{1}{2}", list[0].ToString("x2"), list[1].ToString("x2"), list[2].ToString("x2"));
		}

		public static IList<double> HexToRGB(string hex)
		{
			return new double[3]
			{
				(double)int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber) / 255.0,
				(double)int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber) / 255.0,
				(double)int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber) / 255.0
			};
		}

		public static IList<double> LCHToRGB(IList<double> tuple)
		{
			return XYZToRGB(LUVToXYZ(LCHToLUV(tuple)));
		}

		public static IList<double> RGBToLCH(IList<double> tuple)
		{
			return LUVToLCH(XYZToLUV(RGBToXYZ(tuple)));
		}

		public static IList<double> HUSLToRGB(IList<double> tuple)
		{
			return LCHToRGB(HUSLToLCH(tuple));
		}

		public static IList<double> RGBToHUSL(IList<double> tuple)
		{
			return LCHToHUSL(RGBToLCH(tuple));
		}

		public static IList<double> HUSLPToRGB(IList<double> tuple)
		{
			return LCHToRGB(HUSLPToLCH(tuple));
		}

		public static IList<double> RGBToHUSLP(IList<double> tuple)
		{
			return LCHToHUSLP(RGBToLCH(tuple));
		}

		public static string HUSLToHex(IList<double> tuple)
		{
			return RGBToHex(HUSLToRGB(tuple));
		}

		public static string HUSLPToHex(IList<double> tuple)
		{
			return RGBToHex(HUSLPToRGB(tuple));
		}

		public static IList<double> HexToHUSL(string s)
		{
			return RGBToHUSL(HexToRGB(s));
		}

		public static IList<double> HexToHUSLP(string s)
		{
			return RGBToHUSLP(HexToRGB(s));
		}

		public static Color HUSLToColor(float h, float s, float l)
		{
			double[] collection = new double[3]
			{
				(double)h,
				(double)s,
				(double)l
			};
			IList<double> list = HUSLToRGB(new List<double>(collection));
			return new Color((float)list[0], (float)list[1], (float)list[2]);
		}

		public static Color HUSLPToColor(float h, float s, float l)
		{
			double[] collection = new double[3]
			{
				(double)h,
				(double)s,
				(double)l
			};
			IList<double> list = HUSLPToRGB(new List<double>(collection));
			return new Color((float)list[0], (float)list[1], (float)list[2]);
		}
	}
}
