using Klei;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace KMod
{
	[JsonObject(MemberSerialization.Fields)]
	[DebuggerDisplay("{title}")]
	public struct Label
	{
		public enum DistributionPlatform
		{
			Local,
			Steam,
			Epic,
			Rail,
			Dev
		}

		public DistributionPlatform distribution_platform;

		public string id;

		public ulong version;

		public string title;

		[JsonIgnore]
		private string distribution_platform_name
		{
			get
			{
				return distribution_platform.ToString();
			}
		}

		[JsonIgnore]
		public string install_path
		{
			get
			{
				return FSUtil.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), distribution_platform_name), id));
			}
		}

		public override string ToString()
		{
			return title;
		}

		public bool Match(Label rhs)
		{
			return id == rhs.id && distribution_platform == rhs.distribution_platform;
		}
	}
}
