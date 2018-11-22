using KSerialization;
using Satsuma;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Arc
	{
		[Serialize]
		public string type = "";

		[Serialize]
		public TagSet tags;

		public Satsuma.Arc arc
		{
			get;
			private set;
		}

		public Arc()
		{
		}

		public Arc(string type)
		{
			this.type = type;
		}

		public Arc(Satsuma.Arc arc, string type)
		{
			this.arc = arc;
			this.type = type;
		}
	}
}
