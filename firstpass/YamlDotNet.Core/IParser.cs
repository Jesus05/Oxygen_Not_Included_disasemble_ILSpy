using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public interface IParser
	{
		ParsingEvent Current
		{
			get;
		}

		bool MoveNext();
	}
}
