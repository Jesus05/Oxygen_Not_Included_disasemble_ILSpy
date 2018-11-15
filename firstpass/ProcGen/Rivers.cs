using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class Rivers : YamlIO<Rivers>
	{
		public Dictionary<string, River> rivers
		{
			get;
			private set;
		}

		public Rivers()
		{
			rivers = new Dictionary<string, River>();
		}
	}
}
