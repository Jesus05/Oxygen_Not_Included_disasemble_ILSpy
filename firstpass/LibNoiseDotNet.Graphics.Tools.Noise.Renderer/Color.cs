using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class Color : IEquatable<Color>, IColor
	{
		public delegate byte GrayscaleStrategy(IColor color);

		protected byte _red = 0;

		protected byte _green = 0;

		protected byte _blue = 0;

		protected byte _alpha = byte.MaxValue;

		protected int _hashcode;

		private static Random _rnd = new Random(666);

		public byte Red
		{
			get
			{
				return _red;
			}
			set
			{
				_red = value;
			}
		}

		public byte Green
		{
			get
			{
				return _green;
			}
			set
			{
				_green = value;
			}
		}

		public byte Blue
		{
			get
			{
				return _blue;
			}
			set
			{
				_blue = value;
			}
		}

		public byte Alpha
		{
			get
			{
				return _alpha;
			}
			set
			{
				_alpha = value;
			}
		}

		public static Color BLACK => new Color(0, 0, 0, byte.MaxValue);

		public static Color WHITE => new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public static Color RED => new Color(byte.MaxValue, 0, 0, byte.MaxValue);

		public static Color GREEN => new Color(0, byte.MaxValue, 0, byte.MaxValue);

		public static Color BLUE => new Color(0, 0, byte.MaxValue, byte.MaxValue);

		public static Color TRANSPARENT => new Color(0, 0, 0, 0);

		public Color()
		{
			_hashcode = ((_red + _green + _blue) ^ _rnd.Next());
		}

		public Color(byte r, byte g, byte b, byte a)
			: this()
		{
			_red = r;
			_green = g;
			_blue = b;
			_alpha = a;
		}

		public Color(byte r, byte g, byte b)
			: this()
		{
			_red = r;
			_green = g;
			_blue = b;
			_alpha = byte.MaxValue;
		}

		public bool Equals(Color other)
		{
			return _red == other.Red && _green == other.Green && _blue == other.Blue && _alpha == other.Alpha;
		}

		public static IColor Lerp(IColor color0, IColor color1, float t, bool withAlphaChannel)
		{
			IColor color2 = (IColor)Activator.CreateInstance(color0.GetType());
			color2.Red = Libnoise.Lerp(color0.Red, color1.Red, t);
			color2.Green = Libnoise.Lerp(color0.Green, color1.Green, t);
			color2.Blue = Libnoise.Lerp(color0.Blue, color1.Blue, t);
			color2.Alpha = (byte)((!withAlphaChannel) ? 255 : Libnoise.Lerp(color0.Alpha, color1.Alpha, t));
			return color2;
		}

		public static IColor Lerp(IColor color0, IColor color1, float t)
		{
			return Lerp(color0, color1, t, true);
		}

		public static IColor Lerp32(IColor color0, IColor color1, float t)
		{
			return Lerp(color0, color1, t, true);
		}

		public static IColor Lerp24(IColor color0, IColor color1, float t)
		{
			return Lerp(color0, color1, t, false);
		}

		public static IColor Grayscale(IColor color)
		{
			IColor color2 = (IColor)Activator.CreateInstance(color.GetType());
			IColor color3 = color2;
			byte b2 = color2.Blue = GrayscaleLuminosityStrategy(color);
			b2 = (color2.Green = b2);
			color3.Red = b2;
			color2.Alpha = byte.MaxValue;
			return color2;
		}

		public static IColor Grayscale(IColor color, GrayscaleStrategy Strategy)
		{
			IColor color2 = (IColor)Activator.CreateInstance(color.GetType());
			byte b = Strategy(color);
			IColor color3 = color2;
			byte b3 = color2.Blue = b;
			b3 = (color2.Green = b3);
			color3.Red = b3;
			color2.Alpha = byte.MaxValue;
			return color2;
		}

		public static byte GrayscaleLightnessStrategy(IColor color)
		{
			return (byte)((Math.Max(color.Red, Math.Max(color.Green, color.Blue)) + Math.Min(color.Red, Math.Max(color.Green, color.Blue))) / 2);
		}

		public static byte GrayscaleAverageStrategy(IColor color)
		{
			return (byte)((color.Red + color.Green + color.Blue) / 3);
		}

		public static byte GrayscaleLuminosityStrategy(IColor color)
		{
			return (byte)(0.21f * (float)(int)color.Red + 0.71f * (float)(int)color.Green + 0.07f * (float)(int)color.Blue);
		}

		public override string ToString()
		{
			return $"Color({Red},{Green},{Blue},{Alpha})";
		}

		public override bool Equals(object other)
		{
			if (!(other is IColor))
			{
				return false;
			}
			return _red == ((IColor)other).Red && _green == ((IColor)other).Green && _blue == ((IColor)other).Blue && _alpha == ((IColor)other).Alpha;
		}

		public override int GetHashCode()
		{
			return _hashcode;
		}

		public static bool operator ==(Color a, IColor b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Color a, IColor b)
		{
			return !a.Equals(b);
		}

		public static bool operator >(Color a, IColor b)
		{
			return a._red > b.Red && a._green > b.Green && a._blue > b.Blue && a._alpha > b.Alpha;
		}

		public static bool operator <(Color a, IColor b)
		{
			return a._red < b.Red && a._green < b.Green && a._blue < b.Blue && a._alpha < b.Alpha;
		}

		public static bool operator >=(Color a, IColor b)
		{
			return a > b || a == b;
		}

		public static bool operator <=(Color a, IColor b)
		{
			return a < b || a == b;
		}
	}
}
