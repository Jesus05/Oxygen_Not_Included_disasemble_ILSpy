using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public interface IEmitter
	{
		void Emit(ParsingEvent @event);
	}
}
