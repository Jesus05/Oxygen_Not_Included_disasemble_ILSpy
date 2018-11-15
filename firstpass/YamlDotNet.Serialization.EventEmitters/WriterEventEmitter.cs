using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.EventEmitters
{
	public sealed class WriterEventEmitter : IEventEmitter
	{
		void IEventEmitter.Emit(AliasEventInfo eventInfo, IEmitter emitter)
		{
			emitter.Emit(new AnchorAlias(eventInfo.Alias));
		}

		void IEventEmitter.Emit(ScalarEventInfo eventInfo, IEmitter emitter)
		{
			emitter.Emit(new Scalar(eventInfo.Anchor, eventInfo.Tag, eventInfo.RenderedValue, eventInfo.Style, eventInfo.IsPlainImplicit, eventInfo.IsQuotedImplicit));
		}

		void IEventEmitter.Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
		{
			emitter.Emit(new MappingStart(eventInfo.Anchor, eventInfo.Tag, eventInfo.IsImplicit, eventInfo.Style));
		}

		void IEventEmitter.Emit(MappingEndEventInfo eventInfo, IEmitter emitter)
		{
			emitter.Emit(new MappingEnd());
		}

		void IEventEmitter.Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
		{
			emitter.Emit(new SequenceStart(eventInfo.Anchor, eventInfo.Tag, eventInfo.IsImplicit, eventInfo.Style));
		}

		void IEventEmitter.Emit(SequenceEndEventInfo eventInfo, IEmitter emitter)
		{
			emitter.Emit(new SequenceEnd());
		}
	}
}
