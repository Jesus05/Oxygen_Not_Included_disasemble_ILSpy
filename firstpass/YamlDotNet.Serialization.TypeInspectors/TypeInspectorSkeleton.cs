using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace YamlDotNet.Serialization.TypeInspectors
{
	public abstract class TypeInspectorSkeleton : ITypeInspector
	{
		public abstract IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container);

		public IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched)
		{
			IEnumerable<IPropertyDescriptor> enumerable = from p in GetProperties(type, container)
			where p.Name == name
			select p;
			using (IEnumerator<IPropertyDescriptor> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					IPropertyDescriptor current = enumerator.Current;
					if (enumerator.MoveNext())
					{
						throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Multiple properties with the name/alias '{0}' already exists on type '{1}', maybe you're misusing YamlAlias or maybe you are using the wrong naming convention? The matching properties are: {2}", name, type.FullName, string.Join(", ", (from p in enumerable
						select p.Name).ToArray())));
					}
					return current;
				}
				if (!ignoreUnmatched)
				{
					throw new SerializationException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' not found on type '{1}'.", name, type.FullName));
				}
				return null;
			}
		}
	}
}
