using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public struct GradientPoint : IEquatable<GradientPoint>
	{
		public IColor Color;

		public float Position;

		private int _hashcode;

		public GradientPoint(float position, IColor color)
		{
			Color = color;
			Position = position;
			_hashcode = ((int)Position ^ Color.GetHashCode());
		}

		public bool Equals(GradientPoint other)
		{
			return Position == other.Position;
		}

		public override int GetHashCode()
		{
			return _hashcode;
		}
	}
}
