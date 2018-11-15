using System;

namespace ProcGen.Noise
{
	public class NoiseBase
	{
		public string name
		{
			get;
			set;
		}

		public Vector2f pos
		{
			get;
			set;
		}

		public virtual Type GetObjectType()
		{
			return null;
		}
	}
}
