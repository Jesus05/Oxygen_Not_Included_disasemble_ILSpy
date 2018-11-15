using System;

namespace YamlDotNet.Serialization
{
	public interface IObjectGraphVisitor<TContext>
	{
		bool Enter(IObjectDescriptor value, TContext context);

		bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, TContext context);

		bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, TContext context);

		void VisitScalar(IObjectDescriptor scalar, TContext context);

		void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, TContext context);

		void VisitMappingEnd(IObjectDescriptor mapping, TContext context);

		void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, TContext context);

		void VisitSequenceEnd(IObjectDescriptor sequence, TContext context);
	}
}
