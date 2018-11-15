using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	public interface IScanner
	{
		Mark CurrentPosition
		{
			get;
		}

		Token Current
		{
			get;
		}

		bool MoveNext();

		bool MoveNextWithoutConsuming();

		void ConsumeCurrent();
	}
}
