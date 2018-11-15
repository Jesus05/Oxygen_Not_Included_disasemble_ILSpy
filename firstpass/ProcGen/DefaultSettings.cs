using Klei;
using System.Collections.Generic;

namespace ProcGen
{
	public class DefaultSettings : YamlIO<DefaultSettings>
	{
		public BaseLocation baseData
		{
			get;
			private set;
		}

		public Dictionary<string, object> data
		{
			get;
			private set;
		}

		public List<string> defaultMoveTags
		{
			get;
			private set;
		}

		public List<string> overworldAddTags
		{
			get;
			private set;
		}

		public DefaultSettings()
		{
			data = new Dictionary<string, object>();
		}
	}
}
