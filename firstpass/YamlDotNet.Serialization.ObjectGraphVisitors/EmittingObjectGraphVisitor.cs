using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class EmittingObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
	{
		private readonly IEventEmitter eventEmitter;

		public EmittingObjectGraphVisitor(IEventEmitter eventEmitter)
		{
			this.eventEmitter = eventEmitter;
		}

		bool IObjectGraphVisitor<IEmitter>.Enter(IObjectDescriptor value, IEmitter context)
		{
			return true;
		}

		bool IObjectGraphVisitor<IEmitter>.EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return true;
		}

		bool IObjectGraphVisitor<IEmitter>.EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return true;
		}

		void IObjectGraphVisitor<IEmitter>.VisitScalar(IObjectDescriptor scalar, IEmitter context)
		{
			eventEmitter.Emit(new ScalarEventInfo(scalar), context);
		}

		void IObjectGraphVisitor<IEmitter>.VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, IEmitter context)
		{
			eventEmitter.Emit(new MappingStartEventInfo(mapping), context);
		}

		void IObjectGraphVisitor<IEmitter>.VisitMappingEnd(IObjectDescriptor mapping, IEmitter context)
		{
			eventEmitter.Emit(new MappingEndEventInfo(mapping), context);
		}

		void IObjectGraphVisitor<IEmitter>.VisitSequenceStart(IObjectDescriptor sequence, Type elementType, IEmitter context)
		{
			eventEmitter.Emit(new SequenceStartEventInfo(sequence), context);
		}

		void IObjectGraphVisitor<IEmitter>.VisitSequenceEnd(IObjectDescriptor sequence, IEmitter context)
		{
			eventEmitter.Emit(new SequenceEndEventInfo(sequence), context);
		}
	}
}
