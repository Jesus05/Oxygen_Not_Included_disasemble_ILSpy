using Klei;

namespace ProcGen
{
	public class SubWorldFile : YamlIO<SubWorldFile>
	{
		public string name
		{
			get;
			private set;
		}

		public SubWorld zone
		{
			get;
			private set;
		}

		public SubWorldFile()
		{
			zone = new SubWorld();
		}
	}
}
