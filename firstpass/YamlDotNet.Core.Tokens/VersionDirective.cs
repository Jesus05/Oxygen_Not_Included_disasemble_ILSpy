using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class VersionDirective : Token
	{
		private readonly Version version;

		public Version Version => version;

		public VersionDirective(Version version)
			: this(version, Mark.Empty, Mark.Empty)
		{
		}

		public VersionDirective(Version version, Mark start, Mark end)
			: base(start, end)
		{
			this.version = version;
		}

		public override bool Equals(object obj)
		{
			VersionDirective versionDirective = obj as VersionDirective;
			return versionDirective != null && version.Equals(versionDirective.version);
		}

		public override int GetHashCode()
		{
			return version.GetHashCode();
		}
	}
}
