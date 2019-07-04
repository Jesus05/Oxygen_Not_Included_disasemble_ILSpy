using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class TypeConverterNodeDeserializer : INodeDeserializer
	{
		private readonly IEnumerable<IYamlTypeConverter> converters;

		public TypeConverterNodeDeserializer(IEnumerable<IYamlTypeConverter> converters)
		{
			if (converters == null)
			{
				throw new ArgumentNullException("converters");
			}
			this.converters = converters;
		}

		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			IYamlTypeConverter yamlTypeConverter = converters.FirstOrDefault((IYamlTypeConverter c) => c.Accepts(expectedType));
			if (yamlTypeConverter != null)
			{
				value = yamlTypeConverter.ReadYaml(parser, expectedType);
				return true;
			}
			value = null;
			return false;
		}
	}
}
