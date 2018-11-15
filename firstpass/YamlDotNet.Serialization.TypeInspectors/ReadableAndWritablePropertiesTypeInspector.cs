using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public sealed class ReadableAndWritablePropertiesTypeInspector : TypeInspectorSkeleton
	{
		private readonly ITypeInspector _innerTypeDescriptor;

		public ReadableAndWritablePropertiesTypeInspector(ITypeInspector innerTypeDescriptor)
		{
			_innerTypeDescriptor = innerTypeDescriptor;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			return from p in _innerTypeDescriptor.GetProperties(type, container)
			where p.CanWrite
			select p;
		}
	}
}
