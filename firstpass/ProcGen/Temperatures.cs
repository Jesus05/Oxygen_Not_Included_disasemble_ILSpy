using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class Temperatures : YamlIO<Temperatures>
	{
		public Dictionary<Temperature.Range, Temperature> ranges
		{
			get;
			private set;
		}

		public Temperatures()
		{
			ranges = new Dictionary<Temperature.Range, Temperature>();
		}
	}
}
