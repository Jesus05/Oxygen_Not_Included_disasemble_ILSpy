using System;
using System.Globalization;

namespace Satsuma.Drawing
{
	public struct PointD : IEquatable<PointD>
	{
		public double X
		{
			get;
			private set;
		}

		public double Y
		{
			get;
			private set;
		}

		public PointD(double x, double y)
		{
			this = default(PointD);
			X = x;
			Y = y;
		}

		public bool Equals(PointD other)
		{
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (obj is PointD)
			{
				return Equals((PointD)obj);
			}
			return false;
		}

		public static bool operator ==(PointD a, PointD b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PointD a, PointD b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() * 17 + Y.GetHashCode();
		}

		public string ToString(IFormatProvider provider)
		{
			return string.Format(provider, "({0} {1})", X, Y);
		}

		public override string ToString()
		{
			return ToString(CultureInfo.CurrentCulture);
		}

		public static PointD operator +(PointD a, PointD b)
		{
			return new PointD(a.X + b.X, a.Y + b.Y);
		}

		public static PointD Add(PointD a, PointD b)
		{
			return a + b;
		}

		public double Distance(PointD other)
		{
			return Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
		}
	}
}
