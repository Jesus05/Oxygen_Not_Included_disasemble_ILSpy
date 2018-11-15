using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Helpers;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class DictionaryNodeDeserializer : INodeDeserializer
	{
		private readonly IObjectFactory _objectFactory;

		public DictionaryNodeDeserializer(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			Type implementedGenericInterface = ReflectionUtility.GetImplementedGenericInterface(expectedType, typeof(IDictionary<, >));
			Type tKey;
			Type tValue;
			IDictionary dictionary;
			if (implementedGenericInterface != null)
			{
				Type[] genericArguments = implementedGenericInterface.GetGenericArguments();
				tKey = genericArguments[0];
				tValue = genericArguments[1];
				value = _objectFactory.Create(expectedType);
				dictionary = (value as IDictionary);
				if (dictionary == null)
				{
					dictionary = new GenericDictionaryToNonGenericAdapter(value, implementedGenericInterface);
				}
			}
			else
			{
				if (!typeof(IDictionary).IsAssignableFrom(expectedType))
				{
					value = null;
					return false;
				}
				tKey = typeof(object);
				tValue = typeof(object);
				value = _objectFactory.Create(expectedType);
				dictionary = (IDictionary)value;
			}
			DeserializeHelper(tKey, tValue, parser, nestedObjectDeserializer, dictionary);
			return true;
		}

		private static void DeserializeHelper(Type tKey, Type tValue, IParser parser, Func<IParser, Type, object> nestedObjectDeserializer, IDictionary result)
		{
			parser.Expect<MappingStart>();
			while (!parser.Accept<MappingEnd>())
			{
				object key = nestedObjectDeserializer(parser, tKey);
				IValuePromise valuePromise = key as IValuePromise;
				object value = nestedObjectDeserializer(parser, tValue);
				IValuePromise valuePromise2 = value as IValuePromise;
				if (valuePromise == null)
				{
					if (valuePromise2 == null)
					{
						result[key] = value;
					}
					else
					{
						valuePromise2.ValueAvailable += delegate(object v)
						{
							result[key] = v;
						};
					}
				}
				else if (valuePromise2 == null)
				{
					valuePromise.ValueAvailable += delegate(object v)
					{
						result[v] = value;
					};
				}
				else
				{
					bool hasFirstPart = false;
					valuePromise.ValueAvailable += delegate(object v)
					{
						if (hasFirstPart)
						{
							result[v] = value;
						}
						else
						{
							key = v;
							hasFirstPart = true;
						}
					};
					valuePromise2.ValueAvailable += delegate(object v)
					{
						if (hasFirstPart)
						{
							result[key] = v;
						}
						else
						{
							value = v;
							hasFirstPart = true;
						}
					};
				}
			}
			parser.Expect<MappingEnd>();
		}
	}
}
