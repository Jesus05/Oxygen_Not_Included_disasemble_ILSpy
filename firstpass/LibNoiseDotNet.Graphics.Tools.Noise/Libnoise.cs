using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise
{
	public static class Libnoise
	{
		public const string VERSION = "1.0.0 B";

		public const float PI = 3.14159274f;

		public const float SQRT_2 = 1.41421354f;

		public const float SQRT_3 = 1.73205078f;

		public const float SQRT_5 = 2.236068f;

		public const float DEG2RAD = 0.0174532924f;

		public const float RAD2DEG = 57.29578f;

		public static void LatLonToXYZ(float lat, float lon, ref float x, ref float y, ref float z)
		{
			float num = (float)Math.Cos((double)(0.0174532924f * lat));
			x = num * (float)Math.Cos((double)(0.0174532924f * lon));
			y = (float)Math.Sin((double)(0.0174532924f * lat));
			z = num * (float)Math.Sin((double)(0.0174532924f * lon));
		}

		public static byte Lerp(byte n0, byte n1, float a)
		{
			float num = (float)(int)n0 / 255f;
			float num2 = (float)(int)n1 / 255f;
			return (byte)((num + a * (num2 - num)) * 255f);
		}

		public static float Lerp(float n0, float n1, float a)
		{
			return n0 + a * (n1 - n0);
		}

		public static float Cerp(float n0, float n1, float n2, float n3, float a)
		{
			float num = n3 - n2 - (n0 - n1);
			float num2 = n0 - n1 - num;
			float num3 = n2 - n0;
			return num * a * a * a + num2 * a * a + num3 * a + n1;
		}

		public static float SCurve3(float a)
		{
			return a * a * (3f - 2f * a);
		}

		public static float SCurve5(float a)
		{
			return a * a * a * (a * (a * 6f - 15f) + 10f);
		}

		public static int Clamp(int value, int lowerBound, int upperBound)
		{
			if (value < lowerBound)
			{
				return lowerBound;
			}
			if (value > upperBound)
			{
				return upperBound;
			}
			return value;
		}

		public static float Clamp(float value, float lowerBound, float upperBound)
		{
			if (value < lowerBound)
			{
				return lowerBound;
			}
			if (value > upperBound)
			{
				return upperBound;
			}
			return value;
		}

		public static double Clamp(double value, double lowerBound, double upperBound)
		{
			if (value < lowerBound)
			{
				return lowerBound;
			}
			if (value > upperBound)
			{
				return upperBound;
			}
			return value;
		}

		public static int Clamp01(int value)
		{
			return Clamp(value, 0, 1);
		}

		public static float Clamp01(float value)
		{
			return Clamp(value, 0f, 1f);
		}

		public static double Clamp01(double value)
		{
			return Clamp(value, 0.0, 1.0);
		}

		public static void SwapValues<T>(ref T a, ref T b)
		{
			T val = a;
			a = b;
			b = val;
		}

		public static void SwapValues(ref double a, ref double b)
		{
			Libnoise.SwapValues<double>(ref a, ref b);
		}

		public static void SwapValues(ref int a, ref int b)
		{
			Libnoise.SwapValues<int>(ref a, ref b);
		}

		public static void SwapValues(ref float a, ref float b)
		{
			Libnoise.SwapValues<float>(ref a, ref b);
		}

		public static double ToInt32Range(double value)
		{
			if (value >= 1073741824.0)
			{
				return 2.0 * Math.IEEERemainder(value, 1073741824.0) - 1073741824.0;
			}
			if (value <= -1073741824.0)
			{
				return 2.0 * Math.IEEERemainder(value, 1073741824.0) + 1073741824.0;
			}
			return value;
		}

		public static byte[] UnpackBigUint32(int value, ref byte[] buffer)
		{
			if (buffer.Length < 4)
			{
				Array.Resize(ref buffer, 4);
			}
			buffer[0] = (byte)(value >> 24);
			buffer[1] = (byte)(value >> 16);
			buffer[2] = (byte)(value >> 8);
			buffer[3] = (byte)value;
			return buffer;
		}

		public static byte[] UnpackBigFloat(float value, ref byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public static byte[] UnpackBigUint16(short value, ref byte[] buffer)
		{
			if (buffer.Length < 2)
			{
				Array.Resize(ref buffer, 2);
			}
			buffer[0] = (byte)(value >> 8);
			buffer[1] = (byte)value;
			return buffer;
		}

		public static byte[] UnpackLittleUint16(short value, ref byte[] buffer)
		{
			if (buffer.Length < 2)
			{
				Array.Resize(ref buffer, 2);
			}
			buffer[0] = (byte)(value & 0xFF);
			buffer[1] = (byte)((value & 0xFF00) >> 8);
			return buffer;
		}

		public static byte[] UnpackLittleUint32(int value, ref byte[] buffer)
		{
			if (buffer.Length < 4)
			{
				Array.Resize(ref buffer, 4);
			}
			buffer[0] = (byte)(value & 0xFF);
			buffer[1] = (byte)((value & 0xFF00) >> 8);
			buffer[2] = (byte)((value & 0xFF0000) >> 16);
			buffer[3] = (byte)((value & 4278190080u) >> 24);
			return buffer;
		}

		public static byte[] UnpackLittleFloat(float value, ref byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public static int FastFloor(double x)
		{
			return (!(x >= 0.0)) ? ((int)x - 1) : ((int)x);
		}

		public static int FastFloor(float x)
		{
			return (!(x >= 0f)) ? ((int)x - 1) : ((int)x);
		}
	}
}
