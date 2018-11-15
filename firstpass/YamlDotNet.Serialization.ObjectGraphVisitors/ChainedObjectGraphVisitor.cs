using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public abstract class ChainedObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
	{
		private readonly IObjectGraphVisitor<IEmitter> nextVisitor;

		protected ChainedObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
		{
			this.nextVisitor = nextVisitor;
		}

		public virtual bool Enter(IObjectDescriptor value, IEmitter context)
		{
			return nextVisitor.Enter(value, context);
		}

		public virtual bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return nextVisitor.EnterMapping(key, value, context);
		}

		public virtual bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return nextVisitor.EnterMapping(key, value, context);
		}

		public virtual void VisitScalar(IObjectDescriptor scalar, IEmitter context)
		{
			nextVisitor.VisitScalar(scalar, context);
		}

		public virtual void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, IEmitter context)
		{
			nextVisitor.VisitMappingStart(mapping, keyType, valueType, context);
		}

		public virtual void VisitMappingEnd(IObjectDescriptor mapping, IEmitter context)
		{
			nextVisitor.VisitMappingEnd(mapping, context);
		}

		public virtual void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, IEmitter context)
		{
			nextVisitor.VisitSequenceStart(sequence, elementType, context);
		}

		public virtual void VisitSequenceEnd(IObjectDescriptor sequence, IEmitter context)
		{
			nextVisitor.VisitSequenceEnd(sequence, context);
		}
	}
}
