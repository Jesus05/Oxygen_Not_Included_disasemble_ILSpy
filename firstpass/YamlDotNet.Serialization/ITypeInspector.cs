using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization
{
	public interface ITypeInspector
	{
		IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container);

		IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched);
	}
}
