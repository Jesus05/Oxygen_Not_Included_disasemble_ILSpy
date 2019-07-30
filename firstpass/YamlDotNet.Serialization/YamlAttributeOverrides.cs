using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Helpers;

namespace YamlDotNet.Serialization
{
	public sealed class YamlAttributeOverrides
	{
		private struct AttributeKey
		{
			public readonly Type AttributeType;

			public readonly string PropertyName;

			public AttributeKey(Type attributeType, string propertyName)
			{
				AttributeType = attributeType;
				PropertyName = propertyName;
			}

			public override bool Equals(object obj)
			{
				AttributeKey attributeKey = (AttributeKey)obj;
				return AttributeType.Equals(attributeKey.AttributeType) && PropertyName.Equals(attributeKey.PropertyName);
			}

			public override int GetHashCode()
			{
				return HashCode.CombineHashCodes(AttributeType.GetHashCode(), PropertyName.GetHashCode());
			}
		}

		private sealed class AttributeMapping
		{
			public readonly Type RegisteredType;

			public readonly Attribute Attribute;

			public AttributeMapping(Type registeredType, Attribute attribute)
			{
				RegisteredType = registeredType;
				Attribute = attribute;
			}

			public override bool Equals(object obj)
			{
				AttributeMapping attributeMapping = obj as AttributeMapping;
				return attributeMapping != null && RegisteredType.Equals(attributeMapping.RegisteredType) && Attribute.Equals(attributeMapping.Attribute);
			}

			public override int GetHashCode()
			{
				return HashCode.CombineHashCodes(RegisteredType.GetHashCode(), Attribute.GetHashCode());
			}

			public int Matches(Type matchType)
			{
				int num = 0;
				for (Type type = matchType; type != null; type = type.BaseType())
				{
					num++;
					if (type == RegisteredType)
					{
						return num;
					}
				}
				if (matchType.GetInterfaces().Contains(RegisteredType))
				{
					return num;
				}
				return 0;
			}
		}

		private readonly Dictionary<AttributeKey, List<AttributeMapping>> overrides = new Dictionary<AttributeKey, List<AttributeMapping>>();

		public T GetAttribute<T>(Type type, string member) where T : Attribute
		{
			if (overrides.TryGetValue(new AttributeKey(typeof(T), member), out List<AttributeMapping> value))
			{
				int num = 0;
				AttributeMapping attributeMapping = null;
				foreach (AttributeMapping item in value)
				{
					int num2 = item.Matches(type);
					if (num2 > num)
					{
						num = num2;
						attributeMapping = item;
					}
				}
				if (num > 0)
				{
					return (T)attributeMapping.Attribute;
				}
			}
			return (T)null;
		}

		public void Add(Type type, string member, Attribute attribute)
		{
			AttributeMapping item = new AttributeMapping(type, attribute);
			AttributeKey key = new AttributeKey(attribute.GetType(), member);
			if (!overrides.TryGetValue(key, out List<AttributeMapping> value))
			{
				value = new List<AttributeMapping>();
				overrides.Add(key, value);
			}
			else if (value.Contains(item))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Attribute ({2}) already set for Type {0}, Member {1}", type.FullName, member, attribute));
			}
			value.Add(item);
		}

		public void Add<TClass>(Expression<Func<TClass, object>> propertyAccessor, Attribute attribute)
		{
			PropertyInfo propertyInfo = propertyAccessor.AsProperty();
			Add(typeof(TClass), propertyInfo.Name, attribute);
		}

		public YamlAttributeOverrides Clone()
		{
			YamlAttributeOverrides yamlAttributeOverrides = new YamlAttributeOverrides();
			foreach (KeyValuePair<AttributeKey, List<AttributeMapping>> @override in overrides)
			{
				foreach (AttributeMapping item in @override.Value)
				{
					YamlAttributeOverrides yamlAttributeOverrides2 = yamlAttributeOverrides;
					Type registeredType = item.RegisteredType;
					AttributeKey key = @override.Key;
					yamlAttributeOverrides2.Add(registeredType, key.PropertyName, item.Attribute);
				}
			}
			return yamlAttributeOverrides;
		}
	}
}
