using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	public static class Constants
	{
		public static readonly TagDirective[] DefaultTagDirectives = new TagDirective[2]
		{
			new TagDirective("!", "!"),
			new TagDirective("!!", "tag:yaml.org,2002:")
		};

		public const int MajorVersion = 1;

		public const int MinorVersion = 1;

		public const char HandleCharacter = '!';

		public const string DefaultHandle = "!";
	}
}
