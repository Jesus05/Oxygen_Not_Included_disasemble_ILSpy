using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public struct ControlPoint : IEquatable<ControlPoint>
	{
		public float Input;

		public float Output;

		public ControlPoint(float input, float output)
		{
			Input = input;
			Output = output;
		}

		public bool Equals(ControlPoint other)
		{
			return Input == other.Input;
		}
	}
}
