using System;

namespace YamlDotNet.Serialization.TypeResolvers
{
	public sealed class DynamicTypeResolver : ITypeResolver
	{
		public Type Resolve(Type staticType, object actualValue)
		{
			return (actualValue == null) ? staticType : actualValue.GetType();
		}
	}
}
