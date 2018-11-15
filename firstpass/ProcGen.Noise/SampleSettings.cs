using System;

namespace ProcGen.Noise
{
	public class SampleSettings : NoiseBase
	{
		public float zoom
		{
			get;
			set;
		}

		public bool normalise
		{
			get;
			set;
		}

		public bool seamless
		{
			get;
			set;
		}

		public Vector2f lowerBound
		{
			get;
			set;
		}

		public Vector2f upperBound
		{
			get;
			set;
		}

		public SampleSettings()
		{
			zoom = 0.1f;
			lowerBound = new Vector2f(2, 2);
			upperBound = new Vector2f(4, 4);
			seamless = false;
			normalise = false;
		}

		public override Type GetObjectType()
		{
			return typeof(SampleSettings);
		}
	}
}
