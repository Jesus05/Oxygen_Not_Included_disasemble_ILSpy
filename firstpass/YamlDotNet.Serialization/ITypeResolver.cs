using System;

namespace YamlDotNet.Serialization
{
	public interface ITypeResolver
	{
		Type Resolve(Type staticType, object actualValue);
	}
}
