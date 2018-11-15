using System;

namespace YamlDotNet.Core
{
	[Serializable]
	public class Version
	{
		public int Major
		{
			get;
			private set;
		}

		public int Minor
		{
			get;
			private set;
		}

		public Version(int major, int minor)
		{
			Major = major;
			Minor = minor;
		}

		public override bool Equals(object obj)
		{
			Version version = obj as Version;
			return version != null && Major == version.Major && Minor == version.Minor;
		}

		public override int GetHashCode()
		{
			return Major.GetHashCode() ^ Minor.GetHashCode();
		}
	}
}
