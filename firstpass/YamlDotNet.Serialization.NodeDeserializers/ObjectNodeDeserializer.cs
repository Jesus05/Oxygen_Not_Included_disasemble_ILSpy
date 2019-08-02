using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class ObjectNodeDeserializer : INodeDeserializer
	{
		private readonly IObjectFactory _objectFactory;

		private readonly ITypeInspector _typeDescriptor;

		private readonly bool _ignoreUnmatched;

		private readonly Action<string> _unmatchedLogFn;

		public ObjectNodeDeserializer(IObjectFactory objectFactory, ITypeInspector typeDescriptor, bool ignoreUnmatched, Action<string> unmatchedLogFn)
		{
			_objectFactory = objectFactory;
			_typeDescriptor = typeDescriptor;
			_ignoreUnmatched = ignoreUnmatched;
			_unmatchedLogFn = unmatchedLogFn;
		}

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			MappingStart mappingStart = parser.Allow<MappingStart>();
			if (mappingStart == null)
			{
				value = null;
				return false;
			}
			value = _objectFactory.Create(expectedType);
			while (!parser.Accept<MappingEnd>())
			{
				Scalar scalar = parser.Expect<Scalar>();
				IPropertyDescriptor property = _typeDescriptor.GetProperty(expectedType, null, scalar.Value, _ignoreUnmatched);
				if (property == null)
				{
					if (_unmatchedLogFn != null)
					{
						_unmatchedLogFn(string.Format("{2} Found a property '{0}' on a type '{1}', but that type doesn't have that property!", scalar.Value, expectedType.FullName, parser.Current.Start));
					}
					parser.SkipThisAndNestedEvents();
				}
				else
				{
					object obj = nestedObjectDeserializer(parser, property.Type);
					IValuePromise valuePromise = obj as IValuePromise;
					if (valuePromise == null)
					{
						object value2 = TypeConverter.ChangeType(obj, property.Type);
						property.Write(value, value2);
					}
					else
					{
						object valueRef = value;
						valuePromise.ValueAvailable += delegate(object v)
						{
							object value3 = TypeConverter.ChangeType(v, property.Type);
							property.Write(valueRef, value3);
						};
					}
				}
			}
			parser.Expect<MappingEnd>();
			return true;
		}
	}
}
