using System.Collections.Generic;

namespace ProcGen
{
	public class Feature
	{
		public string type
		{
			get;
			set;
		}

		public List<string> tags
		{
			get;
			private set;
		}

		public List<string> excludesTags
		{
			get;
			private set;
		}

		public Feature()
		{
			tags = new List<string>();
			excludesTags = new List<string>();
		}
	}
}
