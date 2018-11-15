using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.EventEmitters
{
	public abstract class ChainedEventEmitter : IEventEmitter
	{
		protected readonly IEventEmitter nextEmitter;

		protected ChainedEventEmitter(IEventEmitter nextEmitter)
		{
			if (nextEmitter == null)
			{
				throw new ArgumentNullException("nextEmitter");
			}
			this.nextEmitter = nextEmitter;
		}

		public virtual void Emit(AliasEventInfo eventInfo, IEmitter emitter)
		{
			nextEmitter.Emit(eventInfo, emitter);
		}

		public virtual void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
		{
			nextEmitter.Emit(eventInfo, emitter);
		}

		public virtual void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
		{
			nextEmitter.Emit(eventInfo, emitter);
		}

		public virtual void Emit(MappingEndEventInfo eventInfo, IEmitter emitter)
		{
			nextEmitter.Emit(eventInfo, emitter);
		}

		public virtual void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
		{
			nextEmitter.Emit(eventInfo, emitter);
		}

		public virtual void Emit(SequenceEndEventInfo eventInfo, IEmitter emitter)
		{
			nextEmitter.Emit(eventInfo, emitter);
		}
	}
}
