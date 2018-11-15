using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public interface IEventEmitter
	{
		void Emit(AliasEventInfo eventInfo, IEmitter emitter);

		void Emit(ScalarEventInfo eventInfo, IEmitter emitter);

		void Emit(MappingStartEventInfo eventInfo, IEmitter emitter);

		void Emit(MappingEndEventInfo eventInfo, IEmitter emitter);

		void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter);

		void Emit(SequenceEndEventInfo eventInfo, IEmitter emitter);
	}
}
