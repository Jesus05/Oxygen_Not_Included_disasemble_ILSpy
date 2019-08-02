using System;

namespace ProcGen
{
	[Serializable]
	public class MobReference
	{
		public string type
		{
			get;
			private set;
		}

		public MinMax count
		{
			get;
			private set;
		}
	}
}
